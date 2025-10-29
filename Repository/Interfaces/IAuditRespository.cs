using Models.Dto.Requests;
using Models.Entities.Requests;
using static Models.Dto.Responses.AuditResponse;

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
        Task<bool> SaveAuditLogAsync(AuditEntityRequest log, CancellationToken cancellationToken = default);

        /// <summary>
        /// 查詢稽核軌跡
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AuditSearchListResponse> QueryAuditLogAsync(AuditSearchListEntityRequest req, CancellationToken cancellationToken = default);

    }
}
