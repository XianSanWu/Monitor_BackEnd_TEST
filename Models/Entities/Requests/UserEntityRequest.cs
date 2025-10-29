using Models.Common;

namespace Models.Entities.Requests
{
    public class UserEntityRequest
    {
        #region 查詢用
        public class UserSearchListEntityRequest : BaseSearchModel
        {
            public UserSearchListFieldModelEntityRequest? FieldModel { get; set; }

        }
        public class UserSearchListFieldModelEntityRequest
        {
            public string? UserId { get; set; }
            public string? TokenUuid { get; set; }
            public string? UserName { get; set; }
            public bool? IsUse { get; set; }

        }
        #endregion

        #region 更新
        public class UserUpdateEntityRequest
        {
            public UserUpdateFieldEntityRequest? FieldRequest { get; set; }
            public List<UserUpdateConditionEntityRequest>? ConditionRequest { get; set; }
        }
        #endregion


        #region 更新[欄位]
        /// <summary> User更新[欄位] </summary>
        public class UserUpdateFieldEntityRequest
        {
            public string? UserName { get; set; }
            public bool? IsUse { get; set; }
            public int?  FeatureMask { get; set; }
            public DateTime? UpdateAt { get; set; }
        }
        #endregion

        #region 更新[條件]
        /// <summary>User更新[條件]</summary>
        public class UserUpdateConditionEntityRequest : BaseConditionModel
        {
            public FieldWithMetadataModel? UserName { get; set; } = new();
            public FieldWithMetadataModel? Uuid { get; set; } = new();
        }
        #endregion
    }
}
