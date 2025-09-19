
using Models.Dto.Responses;

namespace Repository.Interfaces
{
    public interface ITokenRespository : IRepository
    {

        /// <summary>
        /// 取得 UserTokenByRefreshToken
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AuthResponse> GetUserTokenByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

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
