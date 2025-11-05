using Models.Common;

namespace Models.Entities.Responses
{
    /// <summary>
    /// 使用者 Token 紀錄表（支援多裝置登入 / JWT 驗證）
    /// </summary>
    public class UserTokenEntityResponse
    {
        #region 查詢List回傳
        public class UserTokenEntitySearchListResponse : BaseModel
        {
            #region Properties
            /// <summary>  查詢結果[清單] </summary>
            public List<UserTokenEntity>? SearchItem { get; set; }
            #endregion
        }
        #endregion

        public class UserTokenEntity
        {
            /// <summary>
            /// 主鍵
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// 對應的使用者 Id
            /// </summary>
            public int UserId { get; set; }

            /// <summary>
            /// JWT 唯一識別碼（jti）
            /// </summary>
            public string JwtId { get; set; } = string.Empty;

            /// <summary>
            /// 建立時間
            /// </summary>
            public DateTime CreateAt { get; set; }

            /// <summary>
            /// 重新整理權杖
            /// </summary>
            public string RefreshToken { get; set; } = string.Empty;

            /// <summary>
            /// RefreshToken 過期時間
            /// </summary>
            public DateTime RefreshTokenExpiresAt { get; set; }

            public bool IsLogin { get; set; }
            public string TokenUuid { get; set; } = string.Empty;
            public string AccessToken { get; set; } = string.Empty;

            public DateTime AccessTokenExpiresAt { get; set; }

        }
    }
}
