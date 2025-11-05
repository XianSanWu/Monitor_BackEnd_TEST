using static Models.Entities.Requests.WorkflowStepsEntityRequest;
using static Models.Entities.Responses.WorkflowEntityResponse;

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
        Task<WorkflowStepsEntitySearchListResponse> QueryWorkflowStepsSearchLastList(WorkflowStepsSearchListEntityRequest searchReq, CancellationToken cancellationToken = default);

        /// <summary>
        /// 工作進度查詢DB
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<WorkflowStepsEntitySearchListResponse> QueryWorkflowStepsSearchList(WorkflowStepsSearchListEntityRequest searchReq, CancellationToken cancellationToken = default);

    }
}
