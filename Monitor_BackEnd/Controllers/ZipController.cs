using Services.Interfaces;

namespace WebAPi.Controllers
{
    public partial class ZipController(
       ILogger<ZipController> logger,
       IZipService zipService
       )
       : BaseController()
    {
        #region DI
        private readonly ILogger<ZipController> _logger = logger;
        private readonly IZipService _zipService = zipService;
        #endregion
    }
}