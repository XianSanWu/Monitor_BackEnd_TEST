using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Dto.Requests;
using Models.Enums;
using Repository.Interfaces;
using Services.Interfaces;
using Repository.UnitOfWorkExtension;

namespace Services.Implementations
{
    public class AuditService(
        ILogger<AuditService> logger,
        IConfiguration config,
        IMapper mapper,
        IUnitOfWorkFactory uowFactory,
        IRepositoryFactory repositoryFactory,
        IUnitOfWorkScopeAccessor scopeAccessor
    ) : IAuditService
    {
        private readonly ILogger<AuditService> _logger = logger;
        private readonly IConfiguration _config = config;
        private readonly IUnitOfWorkFactory _uowFactory = uowFactory;
        private readonly IRepositoryFactory _repositoryFactory = repositoryFactory;
        private readonly IUnitOfWorkScopeAccessor _scopeAccessor = scopeAccessor;

        public async Task<bool> SaveAuditLogAsync(AuditRequest log, CancellationToken cancellationToken = default)
        {
            var dbType = DBConnectionEnum.Cdp;
#if TEST
            dbType = DBConnectionEnum.DefaultConnection;
#endif
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);

            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IAuditRespository>(_scopeAccessor);

            return await repo.SaveAuditLogAsync(log, cancellationToken).ConfigureAwait(false);
        }
    }
}
