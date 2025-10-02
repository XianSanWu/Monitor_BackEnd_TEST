using Models.Dto.Requests;

namespace Repository.Interfaces
{
    public interface IAuditRespository : IRepository
    {
        Task<bool> SaveAuditLogAsync(AuditRequest log, CancellationToken cancellationToken = default);

    }
}
