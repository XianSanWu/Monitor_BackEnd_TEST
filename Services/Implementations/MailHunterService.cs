using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Enums;
using Repository.Interfaces;
using Repository.UnitOfWorkExtension;
using Services.Interfaces;
using static Models.Dto.Requests.MailHunterRequest;
using static Models.Dto.Responses.MailHunterResponse;
using static Models.Entities.Requests.MailHunterEntityRequest;

namespace Services.Implementations
{
    public class MailHunterService(
        ILogger<MailHunterService> logger,
        IConfiguration config,
        IMapper mapper,
        IUnitOfWorkFactory uowFactory,
        IRepositoryFactory repositoryFactory,
        IUnitOfWorkScopeAccessor scopeAccessor
        ) : IMailHunterService
    {
        private readonly ILogger<MailHunterService> _logger = logger;
        private readonly IConfiguration _config = config;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWorkFactory _uowFactory = uowFactory;
		private readonly IRepositoryFactory _repositoryFactory = repositoryFactory;
        private readonly IUnitOfWorkScopeAccessor _scopeAccessor = scopeAccessor;

        /// <summary>
        /// 查詢專案發送數量
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<MailHunterSearchListResponse> GetProjectMailCountList(MailHunterSearchListRequest req, CancellationToken cancellationToken = default)
        {
            var entityReq = _mapper.Map<MailHunterEntitySearchListRequest>(req);

            #region 流程
            var dbType = DBConnectionEnum.Mail_hunter;
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);
            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IMailHunterRespository>(_scopeAccessor);
            var entityResp = await repo.GetProjectMailCountList(entityReq, cancellationToken).ConfigureAwait(false);
            var result = _mapper.Map<MailHunterSearchListResponse>(entityResp);

            return result;
            #endregion
        }


    }
}
