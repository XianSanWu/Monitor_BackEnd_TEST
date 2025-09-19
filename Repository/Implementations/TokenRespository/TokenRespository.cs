﻿using AutoMapper;
using Dapper;
using Models.Dto.Responses;
using Repository.Interfaces;

namespace Repository.Implementations.TokenRespository
{
    public partial class TokenRespository(IUnitOfWorkScopeAccessor scopeAccessor, IMapper mapper)
        : BaseRepository(scopeAccessor, mapper), ITokenRespository, IRepository
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
            result = await CurrentUow.Connection.QueryFirstOrDefaultAsync<AuthResponse>(_sqlStr?.ToString() ?? string.Empty, _sqlParams).ConfigureAwait(false);

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
            result = await CurrentUow.Connection.ExecuteScalarAsync<string>(_sqlStr?.ToString() ?? string.Empty, _sqlParams).ConfigureAwait(false);

            if (result == null)
            {
                throw new InvalidOperationException("插入 UserToken 失敗，Uuid 未產生");
            }

            return result;
            #endregion
        }
    }
}
