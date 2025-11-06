using Models.Common;

namespace Models.Entities.Requests
{
    public class PermissionEntityRequest
    {
        #region 查詢用
        public class PermissionEntitySearchListRequest : BaseSearchModel
        {
            public PermissionEntitySearchListFieldModelRequest? FieldModel { get; set; }

        }
        
        public class PermissionEntitySearchListFieldModelRequest
        {
            public bool? IsUse { get; set; }
        }

        #endregion

        #region 更新用
        public class PermissionEntityUpdateRequest
        {
            public List<PermissionEntityUpdateFieldRequest>? FieldRequest { get; set; }
            //public List<PermissionUpdateConditionRequest>? ConditionRequest { get; set; }
        }

        // 更新欄位
        public class PermissionEntityUpdateFieldRequest
        {
            public string Uuid { get; set; } = string.Empty;
            public string? ParentUuid { get; set; }
            public string? Icon { get; set; }
            public string? ModuleName { get; set; }
            public string? FeatureName { get; set; }
            public string? Title { get; set; }
            public string? Action { get; set; }
            public string? Link { get; set; }
            public int BitValue { get; set; }
            public int? Sort { get; set; }
            public bool? IsUse { get; set; }
            public bool? IsVisible { get; set; }
            public DateTime? UpdateAt { get; set; }
        }

        // 更新條件
        public class PermissionEntityUpdateConditionRequest : BaseConditionModel
        {
            public FieldWithMetadataModel? Uuid { get; set; } = new FieldWithMetadataModel();
        }
        #endregion

    }
}
