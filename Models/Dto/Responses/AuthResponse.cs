using Models.Common;

namespace Models.Dto.Responses
{
    public class AuthResponse
    {
        public class AuthSearchListResponse : BaseModel
        {
            #region Properties
            /// <summary>  查詢結果[清單] </summary>
            //public List<AuthSearchResponse>? SearchItem { get; set; }
            #endregion
        }

        public class AuthSearchResponse
        {
            public bool IsLogin { get; set; }
            public string UserId { get; set; } = string.Empty;
            public string TokenUuid { get; set; } = string.Empty;
            public string AccessToken { get; set; } = string.Empty;

            public DateTime AccessTokenExpiresAt { get; set; }

            public string RefreshToken { get; set; } = string.Empty;
            public DateTime RefreshTokenExpiresAt { get; set; }
        }
    }
}
