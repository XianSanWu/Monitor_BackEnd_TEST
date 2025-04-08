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
        IMapper mapper,
        IConfiguration configuration
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

        public async Task<bool> Login(LoginRequest LoginReq, IConfiguration _config, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);

            // 驗證帳號密碼
            return FakeUsers.TryGetValue(key: (LoginReq.UserName ?? ""), value: out var password) &&
                Base64Util.Decode(LoginReq.Password) == Base64Util.Decode(password);

        }
    }
}
