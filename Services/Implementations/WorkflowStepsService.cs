using Microsoft.Extensions.Configuration;
using Models.Dto.Requests;
using Models.Dto.Responses;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class WorkflowStepsService : IWorkflowStepsService
    {
        public Task<WorkflowStepsResponse> QuerySearchList(WorkflowStepsRequest.WorkflowStepsSearchListRequest searchReq, IConfiguration _config, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
