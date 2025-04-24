using AutoMapper;
using Services.Interfaces;

namespace WebAPi.Controllers
{
    public partial class MsmqController(
       IConfiguration config,
       IMapper mapper,
       ILogger<MsmqController> logger,
       IMsmqService msmqService
       )
       : BaseController(config, mapper)
    {
        #region DI
        private readonly ILogger<MsmqController> _logger = logger;
        private readonly IMsmqService _msmqService = msmqService;
        #endregion
    }
}