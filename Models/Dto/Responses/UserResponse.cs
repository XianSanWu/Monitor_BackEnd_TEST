using Models.Common;

namespace Models.Dto.Responses
{
    public class UserResponse : BaseModel
    {
        #region Properties
        /// <summary>  查詢結果[清單] </summary>
        public List<UserSearchListResponse>? SearchItem { get; set; }
        #endregion

        public class UserSearchListResponse
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
