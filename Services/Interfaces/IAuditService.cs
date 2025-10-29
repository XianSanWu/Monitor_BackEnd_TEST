using Models.Dto.Requests;
using static Models.Dto.Responses.AuditResponse;

namespace Services.Interfaces
{
    public interface IAuditService
    {
        /// <summary>
        /// 存取稽核軌跡
        /// </summary>
        /// <param name="log"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SaveAuditLogAsync(AuditRequest log, CancellationToken cancellationToken = default);

        /// <summary>
        /// 查詢稽核軌跡
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AuditSearchListResponse> QueryAuditLogAsync(AuditSearchListRequest req, CancellationToken cancellationToken = default);
    }
}
