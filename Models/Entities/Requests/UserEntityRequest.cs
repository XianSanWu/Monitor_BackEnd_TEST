using Models.Common;

namespace Models.Entities.Requests
{
    public class UserEntityRequest
    {
        #region 查詢用
        public class UserEntitySearchListRequest : BaseSearchModel
        {
            public UserEntitySearchListFieldModelRequest? FieldModel { get; set; }

        }
        public class UserEntitySearchListFieldModelRequest
        {
            public string? UserId { get; set; }
            public string? TokenUuid { get; set; }
            public string? UserName { get; set; }
            public bool? IsUse { get; set; }

        }
        #endregion

        #region 更新
        public class UserEntityUpdateRequest
        {
            public UserEntityUpdateFieldRequest? FieldRequest { get; set; }
            public List<UserEntityUpdateConditionRequest>? ConditionRequest { get; set; }
        }
        #endregion


        #region 更新[欄位]
        /// <summary> User更新[欄位] </summary>
        public class UserEntityUpdateFieldRequest
        {
            public string? UserName { get; set; }
            public bool? IsUse { get; set; }
            public int?  FeatureMask { get; set; }
            public DateTime? UpdateAt { get; set; }
        }
        #endregion

        #region 更新[條件]
        /// <summary>User更新[條件]</summary>
        public class UserEntityUpdateConditionRequest : BaseConditionModel
        {
            public FieldWithMetadataModel? UserName { get; set; } = new();
            public FieldWithMetadataModel? Uuid { get; set; } = new();
        }
        #endregion
    }
}
