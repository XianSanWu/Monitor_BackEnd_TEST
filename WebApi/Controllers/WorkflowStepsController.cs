using Services.Interfaces;

namespace WebAPi.Controllers
{
    public partial class WorkflowStepsController(
       IWorkflowStepsService workflowStepsService
       )
       : BaseController()
    {
        #region DI
        private readonly IWorkflowStepsService _workflowStepsService = workflowStepsService;
        #endregion
    }
}

