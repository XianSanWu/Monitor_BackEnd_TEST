using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.Enums;
using Repository.Interfaces;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Utilities.Utilities;
using System.Security.Cryptography;
using Models.Dto.Responses;
using Repository.UnitOfWorkExtension;

namespace Services.Implementations
{
    public class TokenService(IConfiguration configuration,
        IMapper mapper,
        IUnitOfWorkFactory uowFactory,
        IRepositoryFactory repositoryFactory,
        IUnitOfWorkScopeAccessor scopeAccessor
        ) : ITokenService
    {
        private readonly IConfiguration _config = configuration;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWorkFactory _uowFactory = uowFactory;
        private readonly IRepositoryFactory _repositoryFactory = repositoryFactory;
        private readonly IUnitOfWorkScopeAccessor _scopeAccessor = scopeAccessor;

        private string Key => _config["EncryptionSettings:AESKey"] ?? string.Empty;
        private string Iv => _config["EncryptionSettings:AESIV"] ?? string.Empty;
        private string JwtKey => _config["Jwt:Key"] ?? string.Empty;
        private string JwtIssuer => _config["Jwt:Issuer"] ?? string.Empty;
        private string JwtAudience => _config["Jwt:Audience"] ?? string.Empty;
        private int JwtAccessTokenExpiresAt => _config.GetValue<int>("Jwt:AccessTokenExpiresAt");

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
            var dbType = DBConnectionEnum.Cdp;
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);
            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<ITokenRespository>(_scopeAccessor);
            result = await repo.GetUserTokenByRefreshTokenAsync(refreshToken, cancellationToken).ConfigureAwait(false);

            return result;
            #endregion
        }

        /// <summary>
        /// 產生 JWT Token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="featureMask"></param>
        /// <returns></returns>
        public Task<string> GenerateTokenAsync(string userId, int featureMask)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", userId),
                new Claim("FeatureMask", featureMask.ToString()),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    CryptoUtil.Decrypt(Base64Util.Decode(JwtKey), Key, Iv)
                )
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: JwtIssuer,
                audience: JwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(JwtAccessTokenExpiresAt),
                signingCredentials: creds
            );

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        /// <summary>
        /// 產生 RefreshToken (長隨機字串)
        /// </summary>
        public Task<string> GenerateRefreshTokenAsync()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Task.FromResult(Convert.ToBase64String(randomNumber));
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
        public async Task<string> InsertUserTokenAsync(string userId, string accessToken, DateTime accessTokenExpiresAt, string refreshToken, DateTime refreshTokenExpiresAt, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = string.Empty;
            #endregion

            #region 流程
            var dbType = DBConnectionEnum.Cdp;
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);
            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<ITokenRespository>(_scopeAccessor);
            result = await repo.InsertUserTokenAsync(userId, accessToken, accessTokenExpiresAt, refreshToken, refreshTokenExpiresAt, cancellationToken).ConfigureAwait(false);

            return result;
            #endregion
        }

    }
}
