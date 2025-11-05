using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Dto.Requests;
using Models.Entities.Requests;
using Models.Enums;
using Repository.Interfaces;
using Repository.UnitOfWorkExtension;
using Services.Interfaces;
using static Models.Dto.Responses.AuditResponse;

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

        /// <summary>
        /// 存取稽核軌跡
        /// </summary>
        /// <param name="log"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SaveAuditLogAsync(AuditRequest req, CancellationToken cancellationToken = default)
        {
            var entityReq = mapper.Map<AuditEntityRequest>(req);

            var dbType = DBConnectionEnum.Cdp;
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);
            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IAuditRespository>(_scopeAccessor);

            return await repo.SaveAuditLogAsync(entityReq, cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// 查詢稽核軌跡
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AuditSearchListResponse> QueryAuditLogAsync(AuditSearchListRequest req, CancellationToken cancellationToken = default)
        {
            var entityReq = mapper.Map<AuditSearchListEntityRequest>(req);

            var dbType = DBConnectionEnum.Cdp;
            // 改成通用 Factory 呼叫
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);
            var repo = _repositoryFactory.Create<IAuditRespository>(_scopeAccessor);
            var entityResp = await repo.QueryAuditLogAsync(entityReq, cancellationToken).ConfigureAwait(false);

            var result = mapper.Map<AuditSearchListResponse>(entityResp);

            return result;
        }
    }
}
