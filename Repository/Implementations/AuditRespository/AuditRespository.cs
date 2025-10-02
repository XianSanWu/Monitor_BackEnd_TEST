using AutoMapper;
using Dapper;
using Models.Dto.Requests;
using Repository.Interfaces;

namespace Repository.Implementations.AuditLogRespository
{
    public partial class AuditRespository(IUnitOfWork unitOfWork, IMapper mapper)
       : BaseRepository(unitOfWork, mapper), IAuditRespository, IRepository
    {
        public async Task<bool> SaveAuditLogAsync(AuditRequest log, CancellationToken cancellationToken = default)
        {
            #region 參數宣告

            var result = false;

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            SaveAuditLog(log);

            // 使用 ExecuteAsync 來執行 SQL 更新，並傳入 cancellationToken
            var affectedRows = await _unitOfWork.Connection.ExecuteAsync(_sqlStr?.ToString() ?? string.Empty, _sqlParams).ConfigureAwait(false);

            // 判斷是否有資料被更新
            result = affectedRows > 0;

            return result;
            #endregion
        }
    }
}
