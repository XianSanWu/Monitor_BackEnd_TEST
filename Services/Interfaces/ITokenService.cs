

using static Models.Dto.Responses.AuthResponse;

namespace Services.Interfaces
{
    /// <summary>
    /// JWT Token。
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// 取得 UserTokenByRefreshToken
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AuthSearchResponse> GetUserTokenByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Token 產生服務，用於產生 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="featureMask"></param>
        /// <returns></returns>
        Task<string> GenerateTokenAsync(string userId, int featureMask);

        /// <summary>
        /// 產生 RefreshToken (長隨機字串)
        /// </summary>
        Task<string> GenerateRefreshTokenAsync();

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
        Task<string> InsertUserTokenAsync(string userId, string accessToken, DateTime accessTokenExpiresAt, string refreshToken, DateTime refreshTokenExpiresAt, CancellationToken cancellationToken);
    }
}
