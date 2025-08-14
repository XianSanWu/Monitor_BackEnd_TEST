using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Services.Interfaces;

namespace WebApi.Attributes
{
    public class PermissionRequirement
    {
        public string Module { get; set; } = string.Empty;
        public string Feature { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;

        public static PermissionRequirement? Parse(string input)
        {
            var parts = input.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var part in parts)
            {
                var kv = part.Split('=', 2);
                if (kv.Length == 2)
                {
                    dict[kv[0].Trim()] = kv[1].Trim();
                }
            }

            if (dict.TryGetValue("Module", out var module) &&
                dict.TryGetValue("Feature", out var feature) &&
                dict.TryGetValue("Action", out var action))
            {
                return new PermissionRequirement
                {
                    Module = module,
                    Feature = feature,
                    Action = action
                };
            }

            return null;
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class PermissionGroupFilterAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly List<PermissionRequirement> _requirements = new();

        public PermissionGroupFilterAttribute(params string[] permissionStrings)
        {
            foreach (var permissionString in permissionStrings)
            {
                var requirement = PermissionRequirement.Parse(permissionString);
                if (requirement != null)
                {
                    _requirements.Add(requirement);
                }
            }
        }

        public IReadOnlyList<PermissionRequirement> Requirements => _requirements;

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var featureMaskClaim = user.Claims.FirstOrDefault(c => string.Equals(c.Type, "FeatureMask", StringComparison.OrdinalIgnoreCase))?.Value;
            if (string.IsNullOrEmpty(featureMaskClaim) || !int.TryParse(featureMaskClaim, out int featureMask))
            {
                context.Result = new ForbidResult();
                return;
            }

            var permissionService = context.HttpContext.RequestServices.GetRequiredService<IPermissionService>();

            foreach (var requirement in _requirements)
            {
                var bitValue = await permissionService.GetBitValue(requirement.Module, requirement.Feature, requirement.Action);
                if (bitValue.HasValue && (featureMask & bitValue.Value) == bitValue.Value)
                {
                    // 有任一權限即放行
                    return;
                }
            }

            context.Result = new ForbidResult();
        }
    }
}
