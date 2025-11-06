using static Models.Entities.Requests.AuditEntityRequest;
using static Models.Entities.Responses.AuditEntityResponse;

namespace Repository.Interfaces
{
    public interface IAuditRespository : IRepository
    {
        /// <summary>
        /// 存取稽核軌跡
        /// </summary>
        /// <param name="log"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SaveAuditLogAsync(AuditEntityCommomRequest log, CancellationToken cancellationToken = default);

        /// <summary>
        /// 查詢稽核軌跡
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AuditEntitySearchListResponse> QueryAuditLogAsync(AuditSearchListEntityRequest req, CancellationToken cancellationToken = default);

    }
}
