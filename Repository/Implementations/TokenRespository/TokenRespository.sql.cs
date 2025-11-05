using Dapper;
using System.Text;

namespace Repository.Implementations.TokenRespository
{
    public partial class TokenRespository
    {
        private void GetUserTokenByRefreshToken(string refreshToken)
        {
            #region 參數宣告
            _sqlStr = new StringBuilder();
            _sqlParams = new DynamicParameters();
            #endregion

            _sqlStr.Append(@"SELECT TOP 1 *
        FROM UserTokens
        WHERE RefreshToken = @RefreshToken
        ORDER BY CreateAt DESC");

            _sqlParams.Add("@RefreshToken", refreshToken);

        }

        private void InsertUserToken(string userId, string accessToken,DateTime accessTokenExpiresAt, string refreshToken, DateTime refreshTokenExpiresAt)
        {
            #region 參數宣告
            _sqlStr = new StringBuilder();
            _sqlParams = new DynamicParameters();
            #endregion

            _sqlStr.Append(@"
        INSERT INTO UserTokens (UserId, AccessToken, AccessTokenExpiresAt, RefreshToken, RefreshTokenExpiresAt)
OUTPUT INSERTED.Uuid
VALUES (@UserId, @AccessToken, @AccessTokenExpiresAt, @RefreshToken, @RefreshTokenExpiresAt);");

            _sqlParams.Add("@UserId", userId);
            _sqlParams.Add("@AccessToken", accessToken);
            _sqlParams.Add("@AccessTokenExpiresAt", accessTokenExpiresAt);
            _sqlParams.Add("@RefreshToken", refreshToken);
            _sqlParams.Add("@RefreshTokenExpiresAt", refreshTokenExpiresAt);
        }
    }
}