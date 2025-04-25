using AutoMapper;
using Microsoft.Extensions.Configuration;
using Models.Dto.Requests;
using Models.Dto.Responses;
using Models.Enums;
using Repository.Implementations.WorkflowStepsRespository;
using Repository.Implementations;
using Repository.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Dto.Requests.MailHunterRequest;
using static Models.Dto.Responses.MailHunterResponse;
using static Models.Dto.Responses.WorkflowStepsResponse;
using Repository.Implementations.MailHunterRespository;
using Microsoft.Extensions.Logging;

namespace Services.Implementations
{
    public class MailHunterService(
        ILogger<MailHunterService> logger,
        IMapper mapper) : IMailHunterService
    {
        private readonly ILogger<MailHunterService> _logger = logger;

        /// <summary>
        /// 查詢專案發送數量
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<MailHunterSearchListResponse> GetProjectMailCountList(MailHunterSearchListRequest searchReq, IConfiguration _config, CancellationToken cancellationToken = default)
        {
            #region 參數宣告
            //Task allTasks = null; 
            var result = new MailHunterSearchListResponse();
            #endregion

            #region 流程
            var MHU_dbHelper = new DbHelper(_config, DBConnectionEnum.Mail_hunter);
#if DEBUG
            MHU_dbHelper = new DbHelper(_config, DBConnectionEnum.DefaultConnection);
#endif
            using (IDbHelper dbHelper = MHU_dbHelper)
            {
                IMailHunterRespository _mhuRp = new MailHunterRespository(dbHelper.UnitOfWork, mapper);
                result = await _mhuRp.GetProjectMailCountList(searchReq, cancellationToken).ConfigureAwait(false);
            }

            return result;
            #endregion
        }


    }
}
