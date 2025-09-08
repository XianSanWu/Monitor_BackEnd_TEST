using AutoMapper;
using Azure.Core;
using Dapper;
using k8s.KubeConfigModels;
using Models.Dto.Responses;
using Repository.Interfaces;
using System;
using System.Threading;

namespace Repository.Implementations.TokenRespository
{
    public partial class TokenRespository(IUnitOfWork unitOfWork, IMapper mapper)
        : BaseRepository(unitOfWork, mapper), ITokenRespository
    {

        /// <summary>
        /// 取得 UserTokenByRefreshToken
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AuthResponse> GetUserTokenByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            #region 參數宣告

            var result = new AuthResponse();

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            GetUserTokenByRefreshToken(refreshToken);

            // 執行 SQL 
            result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<AuthResponse>(_sqlStr?.ToString() ?? string.Empty, _sqlParams).ConfigureAwait(false);

            return result;
            #endregion
        }

        /// <summary>
        /// 儲存User Token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="accessToken"></param>
        /// <param name="accessTokenExpiresAt"></param>
        /// <param name="refreshToken"></param>
        /// <param name="refreshTokenExpiresAt"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<string> InsertUserTokenAsync(string userId, string accessToken, DateTime accessTokenExpiresAt, string refreshToken, DateTime refreshTokenExpiresAt, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = string.Empty;

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            InsertUserToken(userId, accessToken, accessTokenExpiresAt, refreshToken, refreshTokenExpiresAt);

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
