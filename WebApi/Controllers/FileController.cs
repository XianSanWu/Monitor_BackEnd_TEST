using Services.Interfaces;

namespace WebAPi.Controllers
{
    public partial class FileController(
       IFileService fileService,
       IZipService zipService
       )
       : BaseController()
    {
        #region DI
        private readonly IFileService _fileService = fileService;
        private readonly IZipService _zipService = zipService;
        #endregion
    }
}