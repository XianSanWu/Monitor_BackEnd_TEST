using AutoMapper;
using Services.Interfaces;
using static Models.Dto.Requests.Validation.WorkflowStepsRequestValidator;

namespace WebAPi.Controllers
{
    public partial class WorkflowStepsController(
       IConfiguration config,
       IMapper mapper,
       ILogger<WorkflowStepsController> logger,
       WorkflowStepsSearchListRequestValidator searchListRequestValidator,
       IWorkflowStepsService workflowStepsService
       )
       : BaseController(config, mapper)
    {
        #region DI
        private readonly ILogger<WorkflowStepsController> _logger = logger;
        private readonly WorkflowStepsSearchListRequestValidator _searchListRequestValidator = searchListRequestValidator;
        private readonly IWorkflowStepsService _workflowStepsService = workflowStepsService;
        #endregion
    }
}

