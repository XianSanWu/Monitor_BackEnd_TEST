using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Utilities.Utilities;

namespace Services.Implementations
{
    public class TokenService(IConfiguration configuration) : ITokenService
    {
        private readonly IConfiguration _config = configuration;
        private string Key => _config["EncryptionSettings:AESKey"] ?? string.Empty;
        private string Iv => _config["EncryptionSettings:AESIV"] ?? string.Empty;
        private string JwtKey => _config["Jwt:Key"] ?? string.Empty;
        private string JwtIssuer => _config["Jwt:Issuer"] ?? string.Empty;
        private string JwtAudience => _config["Jwt:Audience"] ?? string.Empty;

        public string GenerateToken(string userId, int featureMask)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", userId),
                new Claim("FeatureMask", featureMask.ToString())
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(CryptoUtil.Decrypt(Base64Util.Decode(JwtKey), Key, Iv)));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: JwtIssuer,
                audience: JwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
