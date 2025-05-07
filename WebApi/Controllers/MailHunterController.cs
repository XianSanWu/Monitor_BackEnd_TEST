using Services.Interfaces;

namespace WebAPi.Controllers
{
    public partial class MailHunterController(
       ILogger<MailHunterController> logger,
       IMailHunterService mailHunterService,
       IFileService fileService,
       IZipService zipService
       )
       : BaseController()
    {
        #region DI
        private readonly ILogger<MailHunterController> _logger = logger;
        private readonly IMailHunterService _mailHunterService = mailHunterService;
        private readonly IFileService _fileService = fileService;
        private readonly IZipService _zipService = zipService;
        #endregion
    }
}