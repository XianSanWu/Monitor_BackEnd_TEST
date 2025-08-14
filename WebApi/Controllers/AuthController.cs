using Services.Implementations;
using Services.Interfaces;
using WebAPi.Controllers;

namespace WebApi.Controllers
{
    public partial class AuthController(
        IAuthService authService
        )
        : BaseController()
    {
        #region DI
        private readonly IAuthService _authService = authService;
        #endregion
    }
}