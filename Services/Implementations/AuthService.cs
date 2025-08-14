using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Dto.Responses;
using Services.Interfaces;
using Utilities.Utilities;
using static Models.Dto.Requests.AuthRequest;

namespace Services.Implementations
{
    public class AuthService(
        ILogger<AuthService> logger,
        IConfiguration configuration,
        IMapper mapper,
        IPermissionService permissionService,
        ITokenService tokenService
            ) : IAuthService
    {
        private readonly ILogger<AuthService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IConfiguration _config = configuration;
        private readonly IPermissionService _permissionService = permissionService;
        private readonly ITokenService _tokenService = tokenService;

        private Dictionary<string, string> Users => _config.GetSection("Users").Get<Dictionary<string, string>>() ?? [];
        private string Key => _config["EncryptionSettings:AESKey"] ?? string.Empty;
        private string Iv => _config["EncryptionSettings:AESIV"] ?? string.Empty;

        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <param name="LoginReq"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AuthResponse> Login(LoginRequest LoginReq, CancellationToken cancellationToken = default)
        {
            var result = new AuthResponse();

            //await Task.Delay(1000, cancellationToken).ConfigureAwait(false);

            // 驗證帳號密碼
            var isLogined = Users.TryGetValue(key: (LoginReq.UserName ?? string.Empty), value: out var password) &&
                CryptoUtil.Decrypt(Base64Util.Decode(LoginReq.Password), Key, Iv) == CryptoUtil.Decrypt(Base64Util.Decode(password), Key, Iv);

            if (isLogined)
            {
                var user = await _permissionService.GetUserAsync(LoginReq.UserName ?? string.Empty, cancellationToken).ConfigureAwait(false);
                var token = _tokenService.GenerateToken(user.Uuid, user.FeatureMask);
                result.IsLogin = true;
                result.Token = token;
                return result;
            }

            result.IsLogin = false;
            return result;
        }
    }
}
