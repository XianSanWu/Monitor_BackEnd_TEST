using Services.Interfaces;
using WebAPi.Controllers;

namespace WebApi.Controllers
{
    public partial class AuditController(
        IAuditService auditService,
        IConfiguration configuration
        )
        : BaseController()
    {
        #region DI
        private readonly IAuditService _auditService = auditService;
        private readonly IConfiguration _config = configuration;

        #endregion
    }
}