using Models.Dto.Requests;

namespace Services.Interfaces
{
    public interface IAuditService
    {
        Task<bool> SaveAuditLogAsync(AuditRequest log, CancellationToken cancellationToken = default);
    }
}
