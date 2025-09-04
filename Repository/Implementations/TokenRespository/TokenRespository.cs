using AutoMapper;
using Dapper;
using Repository.Interfaces;
using System;
using System.Threading;

namespace Repository.Implementations.TokenRespository
{
    public partial class TokenRespository(IUnitOfWork unitOfWork, IMapper mapper)
        : BaseRepository(unitOfWork, mapper), ITokenRespository
    {
        /// <summary>
        /// 儲存User Token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="expiresAt"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> InsertUserTokenAsync(string userId, string token, DateTime expiresAt, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = string.Empty;

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            InsertUserToken(userId, token, expiresAt);

            // 執行 SQL 
            result = await _unitOfWork.Connection.ExecuteScalarAsync<string>(_sqlStr?.ToString() ?? string.Empty, _sqlParams).ConfigureAwait(false);

            if (result == null)
            {
                throw new InvalidOperationException("插入 UserToken 失敗，Uuid 未產生");
            }

            return result;
            #endregion
        }
    }
}
