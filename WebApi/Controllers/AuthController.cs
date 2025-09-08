using Services.Interfaces;
using WebAPi.Controllers;

namespace WebApi.Controllers
{
    public partial class AuthController(
        IAuthService authService,
        ITokenService tokenService,
        IPermissionService permissionService,
        IConfiguration configuration
        )
        : BaseController()
    {
        #region DI
        private readonly IAuthService _authService = authService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IConfiguration _config = configuration;
        private readonly IPermissionService _permissionService = permissionService;

        #endregion
    }
}