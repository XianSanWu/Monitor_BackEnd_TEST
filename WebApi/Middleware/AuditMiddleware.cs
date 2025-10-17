using Models.Dto.Requests;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.RegularExpressions;
using Utilities.IpHelper;

namespace WebApi.Middleware
{
    /// <summary>
    /// 稽核軌跡
    /// </summary>
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditMiddleware> _logger;
        private readonly IConfiguration _config;

        public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger, IConfiguration config)
        {
            _next = next;
            _logger = logger;
            _config = config;
        }

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
            string responseBodyText = string.Empty;

            // ====== 讀取 Request Body ======
            if (context.Request.Method == HttpMethods.Get || context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put || context.Request.Method == HttpMethods.Patch)
            {
                if (context.Request.Body.CanSeek)
                {
                    context.Request.EnableBuffering();
                    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                    body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }

                if (!string.IsNullOrEmpty(body))
                    body = Regex.Replace(body, "\"password\":\".*?\"", "\"password\":\"***\"");

                if (body.Length > 3900)
                    body = body[..3900] + "...(truncated)";
            }

            // ====== 取得 UserId ======
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

            // ====== 攔截 Response Body ======
            var originalBodyStream = context.Response.Body;
            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context); // 呼叫下一層 middleware

                // 僅在 Content-Type 為 JSON 時才記錄回傳內容
                if (context.Response.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true)
                {
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                    context.Response.Body.Seek(0, SeekOrigin.Begin);

                    if (responseBodyText.Length > 3900)
                        responseBodyText = responseBodyText[..3900] + "...(truncated)";
                }
            }
            finally
            {
                await responseBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }

            // ====== 組成稽核資料 ======
            var httpStatusCode = (context.Response?.StatusCode ?? 0).ToString();
            var frontUrl = context.Request.Headers["X-FrontUrl"].FirstOrDefault() ?? string.Empty;
            var frontActionName = Uri.UnescapeDataString(context.Request.Headers["X-ActionName"].FirstOrDefault() ?? string.Empty);

            var audit = new AuditRequest
            {
                UserId = userId,
                BackActionName = $"{context.Request.Method} {context.Request.Path}",
                HttpMethod = context.Request.Method,
                RequestPath = context.Request.Path,
                Parameters = body,
                IpAddress = IpHelper.GetClientIp(context),
                FrontUrl = frontUrl,
                FrontActionName = frontActionName,
                HttpStatusCode = httpStatusCode,
                ResponseBody = responseBodyText, 
                CreateAt = DateTime.Now
            };

            // ====== 儲存稽核 ======
            try
            {
                await auditService.SaveAuditLogAsync(audit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save audit log.");
            }
        }
    }
}
