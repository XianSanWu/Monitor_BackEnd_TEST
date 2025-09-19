using AutoMapper;
using Dapper;
using Models.Entities;
using Repository.Interfaces;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;
using static Models.Dto.Responses.WorkflowStepsResponse.WorkflowStepsSearchListResponse;

namespace Repository.Implementations.WorkflowStepsRespository
{
    public partial class WorkflowStepsRespository(
        IUnitOfWorkScopeAccessor scopeAccessor,
    IMapper mapper) : BaseRepository(scopeAccessor, mapper), IWorkflowStepsRespository, IRepository
    {

        /// <summary>
        /// 工作進度查詢DB (最後一筆)
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<WorkflowStepsSearchListResponse> QueryWorkflowStepsSearchLastList(WorkflowStepsSearchListRequest searchReq, CancellationToken cancellationToken = default)
        {
            #region 參數宣告

            var result = new WorkflowStepsSearchListResponse();

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            QueryWorkflowLastSql(searchReq);

            result.Page = searchReq.Page;
            result.SearchItem = new List<WorkflowStepsSearchResponse>();

            var _pagingSql = await GetPagingSql(searchReq.Page, CurrentUow, _sqlParams).ConfigureAwait(false);

            var queryWorkflowEntity = (await CurrentUow.Connection.QueryAsync<WorkflowEntity>(_pagingSql, _sqlParams).ConfigureAwait(false)).ToList();
            result.SearchItem = _mapper.Map<List<WorkflowStepsSearchResponse>>(queryWorkflowEntity);
            result.Page.TotalCount = (await CurrentUow.Connection.QueryAsync<int?>(GetTotalCountSql(), _sqlParams).ConfigureAwait(false)).FirstOrDefault() ?? 0;

            return result;

            #endregion
        }


        /// <summary>
        /// 工作進度查詢DB
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<WorkflowStepsSearchListResponse> QueryWorkflowStepsSearchList(WorkflowStepsSearchListRequest searchReq, CancellationToken cancellationToken = default)
        {
            #region 參數宣告

            var result = new WorkflowStepsSearchListResponse();

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            QueryWorkflowSql(searchReq);

            result.Page = searchReq.Page;
            result.SearchItem = new List<WorkflowStepsSearchResponse>();

            var _pagingSql = await GetPagingSql(searchReq.Page, CurrentUow, _sqlParams).ConfigureAwait(false);
            var queryWorkflowEntity = (await CurrentUow.Connection.QueryAsync<WorkflowEntity>(_pagingSql, _sqlParams).ConfigureAwait(false)).ToList();
            result.SearchItem = _mapper.Map<List<WorkflowStepsSearchResponse>>(queryWorkflowEntity);
            result.Page.TotalCount = (await CurrentUow.Connection.QueryAsync<int?>(GetTotalCountSql(), _sqlParams).ConfigureAwait(false)).FirstOrDefault() ?? 0;

            return result;

            #endregion
        }
    }

}
