using Dapper;
using Models.Dto.Responses;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;
using static Models.Dto.Responses.WorkflowStepsResponse.WorkflowStepsSearchListResponse;

namespace Repository.Implementations.WorkflowStepsRespository
{
    public partial class WorkflowStepsRespository(IUnitOfWork unitOfWork) : BaseRepository(unitOfWork), IWorkflowStepsRespository
    {
        public async Task<WorkflowStepsSearchListResponse> QueryJourneySearchList(WorkflowStepsSearchListRequest searchReq, CancellationToken cancellationToken = default)
        {
            #region 參數宣告

            var result = new WorkflowStepsSearchListResponse();

            #endregion

            #region 流程
            var sql = QueryWorkflowSql(searchReq);
            result.Page = searchReq.Page;
            result.SearchItem = new List<WorkflowStepsSearchResponse>();

            var _pagingSql = await GetPagingSql(searchReq.Page, _unitOfWork, _sqlParams).ConfigureAwait(false);
            result.SearchItem = (await _unitOfWork.Connection.QueryAsync<WorkflowStepsSearchResponse>(_pagingSql, _sqlParams).ConfigureAwait(false)).ToList();
            result.Page.TotalCount = (await _unitOfWork.Connection.QueryAsync<int>(GetTotalCountSql(), _sqlParams).ConfigureAwait(false)).FirstOrDefault();

            return result;

            #endregion
        }
        public Task<WorkflowStepsSearchListResponse> QueryGroupSendSearchList(WorkflowStepsSearchListRequest searchReq, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

}
