using static Models.Dto.Responses.WorkflowStepsResponse;
using static Models.Entities.Requests.WorkflowStepsEntityRequest;

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
        Task<WorkflowStepsSearchListResponse> QueryWorkflowStepsSearchLastList(WorkflowStepsSearchListEntityRequest searchReq, CancellationToken cancellationToken = default);

        /// <summary>
        /// 工作進度查詢DB
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<WorkflowStepsSearchListResponse> QueryWorkflowStepsSearchList(WorkflowStepsSearchListEntityRequest searchReq, CancellationToken cancellationToken = default);

    }
}
