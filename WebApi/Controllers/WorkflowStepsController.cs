using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebAPi.Controllers
{
    [Route("[controller]")]
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

