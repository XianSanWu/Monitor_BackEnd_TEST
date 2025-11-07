using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;

namespace Services.Interfaces
{
    public interface IWorkflowStepsService
    {
        /// <summary>
        /// 工作進度查詢(最後一筆)
        /// </summary>
        /// <param name="req"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<WorkflowStepsSearchListResponse> QueryWorkflowStepsSearchLastList(WorkflowStepsSearchListRequest req, CancellationToken cancellationToken = default);

        /// <summary>
        /// 工作進度查詢
        /// </summary>
        /// <param name="req"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<WorkflowStepsSearchListResponse> QueryWorkflowStepsSearchList(WorkflowStepsSearchListRequest req, CancellationToken cancellationToken = default);

        /// <summary>
        /// 取得卡夫卡工作量
        /// </summary>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<WorkflowStepsKafkaResponse> GetKafkaLag(WorkflowStepsKafkaRequest req, CancellationToken cancellationToken = default);
    }
}
