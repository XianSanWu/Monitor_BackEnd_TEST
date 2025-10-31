using Models.Dto.Requests;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.RegularExpressions;
using Utilities.IpHelper;

namespace WebApi.Middleware
{
    /// <summary>
    /// 稽核軌跡中介層，用於記錄 API 的請求與回應資訊
    /// </summary>
    public class AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger, IConfiguration config)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<AuditMiddleware> _logger = logger;
        private readonly IConfiguration _config = config;

        private const int MaxLogLength = 3900; // 記錄內容最大長度限制

        // 從設定檔讀取要排除的路徑
        private string[] ExcludedPaths =>
            (_config["AuditSettings:ExcludedPaths"] ?? string.Empty)
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        // 從設定檔讀取要排除的 HTTP 方法
        private string[] ExcludedMethods =>
            (_config["AuditSettings:ExcludedMethods"] ?? string.Empty)
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        public async Task InvokeAsync(HttpContext context, IAuditService auditService)
        {
            // ========== 排除不需稽核的請求 ==========
            if (ExcludedPaths.Contains(context.Request.Path.Value, StringComparer.OrdinalIgnoreCase) ||
                ExcludedMethods.Contains(context.Request.Method, StringComparer.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            string body = string.Empty;
            string responseBodyText = string.Empty;

            // ========== 讀取 Request Body ==========
            if ((new[] { HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Patch }).Contains(context.Request.Method, StringComparer.OrdinalIgnoreCase))
            {
                context.Request.EnableBuffering(); // 允許重複讀取 Body
                if (context.Request.Body.CanSeek)
                {
                    context.Request.Body.Position = 0;
                    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                    body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }

                // 移除敏感資料（密碼遮蔽）
                if (!string.IsNullOrEmpty(body))
                    body = Regex.Replace(body, "\"password\":\".*?\"", "\"password\":\"***\"", RegexOptions.IgnoreCase);

                // 限制長度
                if (body.Length > MaxLogLength)
                    body = body[..MaxLogLength] + "...(truncated)";
            }

            // ========== 從 JWT 或 Cookie 取得 UserId ==========
            string userId = string.Empty;

            // 從 Claims 取出 UserId
            if (context.User?.Identity?.IsAuthenticated == true)
                userId = context.User.FindFirst("UserId")?.Value ?? string.Empty;

            // 若未登入則嘗試從 Cookie Token 解析
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
                        _logger.LogWarning(ex, "解析 accessToken 時發生錯誤。");
                    }
                }
            }

            // ========== 判斷是否為檔案下載 ==========
            // 能識別任何形式的 true，如 "true", "TRUE", "True", "1", "yes" 等
            var isFileHeader = context.Request.Headers["IsFile"].FirstOrDefault();
            bool isFile = !string.IsNullOrEmpty(isFileHeader) &&
                          bool.TryParse(isFileHeader, out bool boolResult) && boolResult
                          || string.Equals(isFileHeader, "1", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(isFileHeader, "yes", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(isFileHeader, "y", StringComparison.OrdinalIgnoreCase);

            if (!isFile)
            {
                // ========== 攔截 Response Body ==========
                var originalBodyStream = context.Response.Body;
                await using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                try
                {
                    await _next(context); // 呼叫下一層 Middleware

                    // 僅在回傳 JSON 時才記錄
                    if (context.Response.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                        responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                        context.Response.Body.Seek(0, SeekOrigin.Begin);

                        if (responseBodyText.Length > MaxLogLength)
                            responseBodyText = responseBodyText[..MaxLogLength] + "...(truncated)";
                    }
                }
                finally
                {
                    await responseBody.CopyToAsync(originalBodyStream);
                    context.Response.Body = originalBodyStream;
                }
            }
            else
            {
                // 若為檔案下載，不紀錄回傳內容
                responseBodyText = "File download - response body not logged.";
                await _next(context);
            }

            // ========== 組成稽核紀錄 ==========
            var httpStatusCode = (context.Response?.StatusCode ?? 0).ToString();
            var frontUrl = context.Request.Headers["X-FrontUrl"].FirstOrDefault() ?? string.Empty;
            var frontActionId = context.Request.Headers["X-ActionId"].FirstOrDefault() ?? string.Empty;
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
                FrontActionId = frontActionId,
                HttpStatusCode = httpStatusCode,
                ResponseBody = responseBodyText,
                CreateAt = DateTime.Now
            };

            // ========== 儲存稽核紀錄 ==========
            try
            {
                await auditService.SaveAuditLogAsync(audit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "儲存稽核紀錄時發生錯誤。");
            }
        }
    }
}
