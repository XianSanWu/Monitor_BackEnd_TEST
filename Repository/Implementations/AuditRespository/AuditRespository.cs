using AutoMapper;
using Dapper;
using Models.Entities.Requests;
using Models.Entities.Responses;
using Repository.Interfaces;
using static Models.Dto.Responses.AuditResponse;
using static Models.Dto.Responses.AuditResponse.AuditSearchListResponse;
using static Models.Entities.Responses.AuditEntityResponse;

namespace Repository.Implementations.AuditLogRespository
{
    public partial class AuditRespository(IUnitOfWork unitOfWork, IMapper mapper)
       : BaseRepository(unitOfWork, mapper), IAuditRespository, IRepository
    {
        /// <summary>
        /// 存取稽核軌跡
        /// </summary>
        /// <param name="log"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SaveAuditLogAsync(AuditEntityRequest log, CancellationToken cancellationToken = default)
        {
            #region 參數宣告

            var result = false;

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            SaveAudit(log);

            // 使用 ExecuteAsync 來執行 SQL 更新，並傳入 cancellationToken
            var affectedRows = await _unitOfWork.Connection.ExecuteAsync(_sqlStr?.ToString() ?? string.Empty, _sqlParams).ConfigureAwait(false);

            // 判斷是否有資料被更新
            result = affectedRows > 0;

            return result;
            #endregion
        }

        /// <summary>
        /// 查詢稽核軌跡
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AuditEntitySearchListResponse> QueryAuditLogAsync(AuditSearchListEntityRequest req, CancellationToken cancellationToken = default)
        {
            #region 參數宣告

            var result = new AuditEntitySearchListResponse();

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            QueryAuditLog(req);

            var _pagingSql = await GetPagingSql(req.Page, _unitOfWork, _sqlParams).ConfigureAwait(false);
            var queryEntity = (await _unitOfWork.Connection.QueryAsync<AuditEntity>(_pagingSql, _sqlParams).ConfigureAwait(false)).ToList();
           
            result.Page = req.Page;
            result.SearchItem = queryEntity;
            result.Page.TotalCount = (await _unitOfWork.Connection.QueryAsync<int?>(GetTotalCountSql(), _sqlParams).ConfigureAwait(false)).FirstOrDefault() ?? 0;

            return result;

            #endregion
        }
    }
}
