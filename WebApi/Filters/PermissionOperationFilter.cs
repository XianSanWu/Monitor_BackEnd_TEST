using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.Attributes;

namespace WebApi.Filters
{
    public class PermissionOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // 處理 [Authorize] 的顯示
            var hasAuthorize = context.MethodInfo.DeclaringType!.GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
                .Any()
                || context.MethodInfo.GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
                .Any();

            if (hasAuthorize)
            {
                operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    }] = new List<string>()
                }
            };
            }

            // 處理 [PermissionGroupFilter] 的顯示
            var permissionAttr = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<PermissionGroupFilterAttribute>()
                .FirstOrDefault()
                ?? context.MethodInfo.DeclaringType!
                    .GetCustomAttributes(true)
                    .OfType<PermissionGroupFilterAttribute>()
                    .FirstOrDefault();

            if (permissionAttr != null && permissionAttr.Requirements.Any())
            {
                var permissions = permissionAttr.Requirements
                    .Select(r => $"Module={r.Module}; Feature={r.Feature}; Action={r.Action}");

                operation.Description ??= string.Empty;
                operation.Description += $"<br/><b>需要權限:</b> {string.Join(" | ", permissions)}";
            }
        }
    }
}
