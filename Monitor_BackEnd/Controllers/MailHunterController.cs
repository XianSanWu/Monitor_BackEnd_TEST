using AutoMapper;
using Services.Interfaces;

namespace WebAPi.Controllers
{
    public partial class MailHunterController(
       IConfiguration config,
       IMapper mapper,
       ILogger<MailHunterController> logger,
       IMailHunterService mailHunterService
       )
       : BaseController(config, mapper)
    {
        #region DI
        private readonly ILogger<MailHunterController> _logger = logger;
        private readonly IMailHunterService _mailHunterService = mailHunterService;
        #endregion
    }
}