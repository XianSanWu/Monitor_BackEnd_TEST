using Models.Dto.Common;

namespace Models.Dto.Requests
{
    public class UserRequest
    {
        #region 查詢用
        public class UserSearchListRequest : BaseSearchModel
        {
            public UserSearchListFieldModelRequest? FieldModel { get; set; }

        }
        public class UserSearchListFieldModelRequest
        {
            public string? UserName { get; set; }
            public bool? IsUse { get; set; }
        }
        #endregion

        #region 更新
        public class UserUpdateRequest
        {
            public UserUpdateFieldRequest? FieldRequest { get; set; }
            public List<UserUpdateConditionRequest>? ConditionRequest { get; set; }
        }
        #endregion


        #region 更新[欄位]
        /// <summary> User更新[欄位] </summary>
        public class UserUpdateFieldRequest
        {
            public string? UserName { get; set; }
            public bool? IsUse { get; set; }
            public DateTime? UpdateAt { get; set; }
        }
        #endregion

        #region 更新[條件]
        /// <summary>User更新[條件]</summary>
        public class UserUpdateConditionRequest : BaseConditionModel
        {
            public FieldWithMetadataModel UserName { get; set; } = new();
        }
        #endregion
    }
}
