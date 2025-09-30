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
        private int JwtRefreshTokenExpiresAt => _config.GetValue<int>("Jwt:RefreshTokenExpiresAt");
        private int JwtAccessTokenExpiresAt => _config.GetValue<int>("Jwt:AccessTokenExpiresAt");


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
                var user = await _permissionService.GetUserByUserNameAsync(LoginReq.UserName ?? string.Empty, cancellationToken).ConfigureAwait(false);

                // 設定 AccessToken 過期時間 (單純給人看用的)
                var accessTokenExpiresAt = DateTime.Now.AddMinutes(JwtAccessTokenExpiresAt);

                // 產生 AccessToken (JWT)
                var accessToken = await _tokenService.GenerateTokenAsync(user.Uuid, user.FeatureMask).ConfigureAwait(false);

                // 產生 RefreshToken
                var refreshToken = await _tokenService.GenerateRefreshTokenAsync().ConfigureAwait(false);

                // 設定 RefreshToken 過期時間 
                var refreshTokenExpiresAt = DateTime.Now.AddMinutes(JwtRefreshTokenExpiresAt);

                //var token = await _tokenService.GenerateTokenAsync(user.Uuid, user.FeatureMask).ConfigureAwait(false);
                var tokenUuid = await _tokenService.InsertUserTokenAsync(
                                                        user.Uuid,
                                                        accessToken,
                                                        accessTokenExpiresAt,
                                                        refreshToken,
                                                        refreshTokenExpiresAt,
                                                        cancellationToken).ConfigureAwait(false);

                result.IsLogin = true;
                result.TokenUuid = tokenUuid;
                result.AccessToken = accessToken;
                result.AccessTokenExpiresAt = accessTokenExpiresAt;
                result.RefreshToken = refreshToken;
                result.RefreshTokenExpiresAt = refreshTokenExpiresAt;

                return result;
            }

            result.IsLogin = false;
            return result;
        }

        /// <summary>
        /// 取得 UserTokenByRefreshToken
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AuthResponse> GetUserTokenByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            return await _tokenService.GetUserTokenByRefreshTokenAsync(refreshToken, cancellationToken).ConfigureAwait(false);
        }
    }
}
