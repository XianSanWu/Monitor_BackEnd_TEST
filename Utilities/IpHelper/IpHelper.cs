using Microsoft.AspNetCore.Http;
using System.Net;

namespace Utilities.IpHelper
{
    public static class IpHelper
    {
        public static string GetClientIp(HttpContext context)
        {
            // Cloudflare
            if (context.Request.Headers.TryGetValue("CF-Connecting-IP", out var cfIp))
            {
                if (IPAddress.TryParse(cfIp.ToString(), out _))
                    return cfIp.ToString();
            }

            // X-Forwarded-For
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var xff))
            {
                var ip = xff.ToString().Split(',').FirstOrDefault()?.Trim();
                if (IPAddress.TryParse(ip, out _))
                    return ip;
            }

            // X-Real-IP
            if (context.Request.Headers.TryGetValue("X-Real-IP", out var xRealIp))
            {
                if (IPAddress.TryParse(xRealIp.ToString(), out _))
                    return xRealIp.ToString();
            }

            // Fallback
            return context.Connection.RemoteIpAddress?.ToString() ?? "未知";
        }
    }
}
