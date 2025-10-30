﻿using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Entities.Requests;
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
        private readonly IUnitOfWorkFactory _uowFactory = uowFactory;
        private readonly IRepositoryFactory _repositoryFactory = repositoryFactory;
        private readonly IUnitOfWorkScopeAccessor _scopeAccessor = scopeAccessor;

        /// <summary>
        /// 查詢專案發送數量
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<MailHunterSearchListResponse> GetProjectMailCountList(MailHunterSearchListRequest req, CancellationToken cancellationToken = default)
        {
            #region 參數宣告
            //Task allTasks = null; 
            var result = new MailHunterSearchListResponse();
            #endregion

            var entityReq = mapper.Map<MailHunterSearchListEntityRequest>(req);

            #region 流程
            var dbType = DBConnectionEnum.Mail_hunter;
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);
            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IMailHunterRespository>(_scopeAccessor);
            result = await repo.GetProjectMailCountList(entityReq, cancellationToken).ConfigureAwait(false);

            return result;
            #endregion
        }


    }
}
