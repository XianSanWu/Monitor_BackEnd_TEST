using Models.Common;

namespace Models.Dto.Responses
{
    public class UserResponse
    {
        public class UserSearchListResponse : BaseModel
        {
            #region Properties
            /// <summary>  查詢結果[清單] </summary>
            public List<UserSearchResponse>? SearchItem { get; set; }
            #endregion

            public class UserSearchResponse
            {
                public int Id { get; set; } = 0;
                public string Uuid { get; set; } = "";
                public string UserName { get; set; } = "";
                public int FeatureMask { get; set; } = 0;
                public bool IsUse { get; set; }
                public DateTime CreateAt { get; set; }

                public DateTime UpdateAt { get; set; }
            }
        }

    }
}
