using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.Enums;
using Repository.Implementations;
using Repository.Interfaces;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Utilities.Utilities;
using Repository.Implementations.TokenRespository;

namespace Services.Implementations
{
    public class TokenService(IConfiguration configuration,
        IMapper mapper
        ) : ITokenService
    {
        private readonly IConfiguration _config = configuration;
        private readonly IMapper _mapper = mapper;
        private string Key => _config["EncryptionSettings:AESKey"] ?? string.Empty;
        private string Iv => _config["EncryptionSettings:AESIV"] ?? string.Empty;
        private string JwtKey => _config["Jwt:Key"] ?? string.Empty;
        private string JwtIssuer => _config["Jwt:Issuer"] ?? string.Empty;
        private string JwtAudience => _config["Jwt:Audience"] ?? string.Empty;
        private int JwtExpiresTime => _config.GetValue<int>("Jwt:ExpiresTime");

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
                // new Claim("UserPermissions", NewtonsoftJsonHelper.SerializeObject(userPermissions)),
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
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        /// <summary>
        /// 儲存User Token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<string> InsertUserTokenAsync(string userId, string token, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = string.Empty;
            #endregion

            #region 流程
            var CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.Cdp);
#if TEST
            CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.DefaultConnection);
#endif
            var expiresAt = DateTime.Now.AddMinutes(JwtExpiresTime);
            using (IDbHelper dbHelper = CDP_dbHelper)
            {
                ITokenRespository _tRp = new TokenRespository(dbHelper.UnitOfWork, mapper);
                result = await _tRp.InsertUserTokenAsync(userId, token, expiresAt, cancellationToken).ConfigureAwait(false);
            }

            return result;
            #endregion
        }

    }
}
