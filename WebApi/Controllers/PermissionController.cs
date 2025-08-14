using Services.Interfaces;
using WebAPi.Controllers;

namespace WebApi.Controllers
{
    public partial class PermissionController(
        IPermissionService permissionService
        )
        : BaseController()
    {
        #region DI
        private readonly IPermissionService _permissionService = permissionService;
        #endregion
    }
}