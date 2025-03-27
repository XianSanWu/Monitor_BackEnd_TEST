using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Dto.Requests.Validation;
using Services.Implementations;
using Services.Interfaces;
using WebApi.Controllers;

namespace WebAPi.Controllers
{
    public partial class WorkflowStepsController(
       IConfiguration config,
       IMapper mapper,
       ILogger<WorkflowStepsController> logger
       //LoginRequestValidator loginRequestValidator,
       //IAuthService authService
       )
       : BaseController(config, mapper)
    {
        #region DI
        private readonly ILogger<WorkflowStepsController> _logger = logger;
        //private readonly LoginRequestValidator _loginRequestValidator = loginRequestValidator;
        //private readonly IAuthService _authService = authService;
        #endregion
    }
}

