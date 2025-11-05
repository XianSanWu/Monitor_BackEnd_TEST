using Models.Dto.Responses;
using static Models.Dto.Requests.AuthRequest;
using static Models.Dto.Responses.AuthResponse;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <param name="LoginReq"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AuthSearchResponse> Login(LoginRequest LoginReq, CancellationToken cancellationToken = default);

        /// <summary>
        /// 取得 UserTokenByRefreshToken
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AuthSearchResponse> GetUserTokenByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    }
}
