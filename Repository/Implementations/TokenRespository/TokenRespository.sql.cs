using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implementations.TokenRespository
{
    public partial class TokenRespository
    {
        private void InsertUserToken(string userId, string token, DateTime expiresAt)
        {
            #region 參數宣告
            _sqlStr = new StringBuilder();
            _sqlParams = new DynamicParameters();
            #endregion

            _sqlStr.Append(@"
        INSERT INTO UserTokens (UserId, AccessToken, ExpiresAt)
OUTPUT INSERTED.Uuid
VALUES (@UserId, @AccessToken, @ExpiresAt);");

            _sqlParams.Add("@UserId", userId);
            _sqlParams.Add("@AccessToken", token);
            _sqlParams.Add("@ExpiresAt", expiresAt);
        }
    }
}