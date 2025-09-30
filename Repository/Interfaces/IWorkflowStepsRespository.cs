using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;

namespace Repository.Interfaces
{
    public interface IWorkflowStepsRespository : IRepository
    {

        /// <summary>
        /// 工作進度查詢DB (最後一筆)
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<WorkflowStepsSearchListResponse> QueryWorkflowStepsSearchLastList(WorkflowStepsSearchListRequest searchReq, CancellationToken cancellationToken = default);

        /// <summary>
        /// 工作進度查詢DB
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<WorkflowStepsSearchListResponse> QueryWorkflowStepsSearchList(WorkflowStepsSearchListRequest searchReq, CancellationToken cancellationToken = default);

    }
}
