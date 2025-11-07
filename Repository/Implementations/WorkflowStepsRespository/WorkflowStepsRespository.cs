using AutoMapper;
using Dapper;
using Repository.Interfaces;
using static Models.Entities.Requests.WorkflowStepsEntityRequest;
using static Models.Entities.Responses.WorkflowEntityResponse;

namespace Repository.Implementations.WorkflowStepsRespository
{
    public partial class WorkflowStepsRespository(
        IUnitOfWork unitOfWork,
        IMapper mapper) : BaseRepository(unitOfWork, mapper), IWorkflowStepsRespository, IRepository
    {
        /// <summary>
        /// 工作進度查詢DB (最後一筆)
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<WorkflowStepsEntitySearchListResponse> QueryWorkflowStepsSearchLastList(WorkflowStepsEntitySearchListRequest req, CancellationToken cancellationToken = default)
        {
            #region 參數宣告

            var result = new WorkflowStepsEntitySearchListResponse();

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            QueryWorkflowLastSql(req);

            result.Page = req.Page;
            result.SearchItem = new List<WorkflowEntity>();

            var _pagingSql = await GetPagingSql(req.Page, _unitOfWork, _sqlParams).ConfigureAwait(false);
            var queryWorkflowEntity = (await _unitOfWork.Connection.QueryAsync<WorkflowEntity>(_pagingSql, _sqlParams).ConfigureAwait(false)).ToList();
            result.SearchItem = _mapper.Map<List<WorkflowEntity>>(queryWorkflowEntity);
            result.Page.TotalCount = (await _unitOfWork.Connection.QueryAsync<int?>(GetTotalCountSql(), _sqlParams).ConfigureAwait(false)).FirstOrDefault() ?? 0;

            return result;

            #endregion
        }
        

        /// <summary>
        /// 工作進度查詢DB
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<WorkflowStepsEntitySearchListResponse> QueryWorkflowStepsSearchList(WorkflowStepsEntitySearchListRequest req, CancellationToken cancellationToken = default)
        {
            #region 參數宣告

            var result = new WorkflowStepsEntitySearchListResponse();

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            QueryWorkflowSql(req);

            result.Page = req.Page;
            result.SearchItem = new List<WorkflowEntity>();

            var _pagingSql = await GetPagingSql(req.Page, _unitOfWork, _sqlParams).ConfigureAwait(false);
            var queryWorkflowEntity = (await _unitOfWork.Connection.QueryAsync<WorkflowEntity>(_pagingSql, _sqlParams).ConfigureAwait(false)).ToList();
            result.SearchItem = _mapper.Map<List<WorkflowEntity>>(queryWorkflowEntity);
            result.Page.TotalCount = (await _unitOfWork.Connection.QueryAsync<int?>(GetTotalCountSql(), _sqlParams).ConfigureAwait(false)).FirstOrDefault() ?? 0;

            return result;

            #endregion
        }
    }

}
