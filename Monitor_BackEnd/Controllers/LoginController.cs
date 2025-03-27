using AutoMapper;
using Models.Dto.Requests.Validation;
using Services.Interfaces;
using WebAPi.Controllers;

namespace WebApi.Controllers
{
    public partial class LoginController(
        IConfiguration config,
        IMapper mapper,
        ILogger<LoginController> logger,
        LoginRequestValidator loginRequestValidator,
        IAuthService authService
           ) : BaseController(config, mapper)
    {
        #region DI
        private readonly ILogger<LoginController> _logger = logger;
        private readonly LoginRequestValidator _loginRequestValidator = loginRequestValidator;
        private readonly IAuthService _authService = authService;
        #endregion

    }
}