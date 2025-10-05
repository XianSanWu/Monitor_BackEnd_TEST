using Models.Dto.Requests;
using Services.Interfaces;
using System.Text;
using System.Text.RegularExpressions;
using Utilities.IpHelper;
using System.IdentityModel.Tokens.Jwt;

namespace WebApi.Middleware
{
    public class AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger, IConfiguration config)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<AuditMiddleware> _logger = logger;
        private readonly IConfiguration _config = config;

        private string[] ExcludedPaths =>
            (_config["AuditSettings:ExcludedPaths"] ?? string.Empty)
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        private string[] ExcludedMethods =>
            (_config["AuditSettings:ExcludedMethods"] ?? string.Empty)
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        public async Task InvokeAsync(HttpContext context, IAuditService auditService)
        {
            // 排除條件
            if (ExcludedPaths.Contains(context.Request.Path.Value, StringComparer.OrdinalIgnoreCase) ||
                ExcludedMethods.Contains(context.Request.Method, StringComparer.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            string body = string.Empty;

            // 記錄 GET / POST / PUT / PATCH
            if (context.Request.Method == HttpMethods.Get ||
                context.Request.Method == HttpMethods.Post ||
                context.Request.Method == HttpMethods.Put ||
                context.Request.Method == HttpMethods.Patch)
            {
                if (context.Request.Body.CanSeek)
                {
                    context.Request.EnableBuffering();
                    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                    body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }

                if (!string.IsNullOrEmpty(body))
                {
                    // 遮罩 password
                    body = Regex.Replace(body, "\"password\":\".*?\"", "\"password\":\"***\"");
                }

                // 截斷過大內容
                if (body.Length > 4000)
                {
                    body = string.Concat(body.AsSpan(0, 4000), "...(truncated)");
                }
            }

            // 取得 UserId
            string userId = string.Empty;

            if (context.User?.Identity?.IsAuthenticated == true)
            {
                userId = context.User.FindFirst("UserId")?.Value ?? string.Empty;
            }

            if (string.IsNullOrEmpty(userId))
            {
                var accessToken = context.Request.Cookies["accessToken"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    try
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(accessToken);
                        userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? string.Empty;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to parse accessToken for UserId.");
                    }
                }
            }

            // 取得前端 URL
            // 優先用前端 header，沒有就 fallback 後端組合
            //var frontUrl = context.Request.Headers["X-Front-Url"].FirstOrDefault()
            //               ?? $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
            var frontUrl = context.Request.Headers["X-FrontUrl"].FirstOrDefault() ?? string.Empty;

            var audit = new AuditRequest
            {
                UserId = userId,
                ActionName = $"{context.Request.Method} {context.Request.Path}",
                HttpMethod = context.Request.Method,
                RequestPath = context.Request.Path,
                Parameters = body,
                IpAddress = IpHelper.GetClientIp(context),
                FrontUrl = frontUrl,
                CreateAt = DateTime.Now
            };

            try
            {
                await auditService.SaveAuditLogAsync(audit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save audit log.");
            }

            await _next(context);
        }
    }
}
