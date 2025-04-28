using Services.Interfaces;

namespace WebAPi.Controllers
{
    public partial class MsmqController(
       ILogger<MsmqController> logger,
       IMsmqService msmqService
       )
       : BaseController()
    {
        #region DI
        private readonly ILogger<MsmqController> _logger = logger;
        private readonly IMsmqService _msmqService = msmqService;
        #endregion
    }
}