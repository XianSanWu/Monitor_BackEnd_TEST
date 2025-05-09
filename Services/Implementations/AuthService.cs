using AutoMapper;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly ILogger<AuthService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IConfiguration _config = configuration;

        private Dictionary<string, string> Users => _config.GetSection("Users").Get<Dictionary<string, string>>() ?? [];
        private string key => _config["EncryptionSettings:AESKey"] ?? string.Empty;
        private string iv => _config["EncryptionSettings:AESIV"] ?? string.Empty;

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
            return Users.TryGetValue(key: (LoginReq.UserName ?? string.Empty), value: out var password) &&
                CryptoUtil.Decrypt(Base64Util.Decode(LoginReq.Password), key, iv) == CryptoUtil.Decrypt(Base64Util.Decode(password), key, iv);

        }
    }
}
