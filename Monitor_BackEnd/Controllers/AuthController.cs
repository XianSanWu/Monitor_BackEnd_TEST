using AutoMapper;
using Services.Interfaces;
using WebAPi.Controllers;
using static Models.Dto.Requests.Validation.AuthRequestValidator;

namespace WebApi.Controllers
{
    public partial class AuthController(
        IConfiguration config,
        IMapper mapper,
        ILogger<AuthController> logger,
        LoginRequestValidator loginRequestValidator,
        IAuthService authService
        )
        : BaseController(config, mapper)
    {
        #region DI
        private readonly ILogger<AuthController> _logger = logger;
        private readonly LoginRequestValidator _loginRequestValidator = loginRequestValidator;
        private readonly IAuthService _authService = authService;
        #endregion
    }
}