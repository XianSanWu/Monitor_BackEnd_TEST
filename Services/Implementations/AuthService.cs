using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Utilities.Utilities;
using static Models.Dto.Requests.AuthRequest;

namespace Services.Implementations
{
    public class AuthService(
        ILogger<AuthService> logger,
        IConfiguration configuration,
        IMapper mapper
            ) : IAuthService
    {
        private ILogger<AuthService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IConfiguration _config = configuration;

        /// <summary>
        /// 設定兩個帳號
        /// </summary>
        private static readonly Dictionary<string, string> FakeUsers = new()
        {
            { "admin", "YWRtaW5faXQ=" },
            { "user", "dXNlcg==" }
        };

        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <param name="LoginReq"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Login(LoginRequest LoginReq, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken).ConfigureAwait(false);

            // 驗證帳號密碼
            return FakeUsers.TryGetValue(key: (LoginReq.UserName ?? ""), value: out var password) &&
                Base64Util.Decode(LoginReq.Password) == Base64Util.Decode(password);

        }
    }
}
