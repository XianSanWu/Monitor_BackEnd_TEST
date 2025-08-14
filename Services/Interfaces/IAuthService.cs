using Models.Dto.Responses;
using static Models.Dto.Requests.AuthRequest;

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
        Task<AuthResponse> Login(LoginRequest LoginReq, CancellationToken cancellationToken = default);

    }
}
