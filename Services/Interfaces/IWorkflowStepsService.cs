using Microsoft.Extensions.Configuration;
using Models.Dto.Responses;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;

namespace Services.Interfaces
{
    public interface IWorkflowStepsService
    {
        /// <summary>
        /// 工作進度查詢
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<WorkflowStepsSearchListResponse> QueryWorkflowStepsSearchList(WorkflowStepsSearchListRequest searchReq, IConfiguration _config, CancellationToken cancellationToken = default);

       
    }
}
