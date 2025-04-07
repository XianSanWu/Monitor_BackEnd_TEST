using IdentityModel.OidcClient;
using Microsoft.Extensions.Configuration;
using Models.Dto.Requests;
using Models.Dto.Responses;
using Models.Enums;
using Repository.Implementations;
using Repository.Implementations.WorkflowStepsRespository;
using Repository.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;

namespace Services.Implementations
{
    public class WorkflowStepsService : IWorkflowStepsService
    {

        public async Task<WorkflowStepsSearchListResponse> QueryJourneySearchList(WorkflowStepsSearchListRequest searchReq, IConfiguration _config, CancellationToken cancellationToken = default)
        {
            #region 參數宣告
            //Task allTasks = null; 
            var result = new WorkflowStepsSearchListResponse();
            #endregion

            #region 流程
            using (IDbHelper dbHelper = new DbHelper(_config, DBConnectionEnum.DefaultConnection))
            {
                IWorkflowStepsRespository _wfsRp = new WorkflowStepsRespository(dbHelper.UnitOfWork);
                result = await _wfsRp.QueryJourneySearchList(searchReq, cancellationToken).ConfigureAwait(false);
            }

            return result;

            #endregion
        }
        public Task<WorkflowStepsSearchListResponse> QueryGroupSendSearchList(WorkflowStepsSearchListRequest searchReq, IConfiguration _config, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
