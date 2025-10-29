using Models.Common;

namespace Models.Entities.Requests
{
    public class PermissionEntityRequest
    {
        #region 查詢用
        public class PermissionSearchListEntityRequest : BaseSearchModel
        {
            public PermissionSearchListFieldModelEntityRequest? FieldModel { get; set; }

        }
        
        public class PermissionSearchListFieldModelEntityRequest
        {
            public bool? IsUse { get; set; }
        }

        #endregion

        #region 更新用
        public class PermissionUpdateEntityRequest
        {
            public List<PermissionUpdateFieldEntityRequest>? FieldRequest { get; set; }
            //public List<PermissionUpdateConditionRequest>? ConditionRequest { get; set; }
        }

        // 更新欄位
        public class PermissionUpdateFieldEntityRequest
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
        public class PermissionUpdateConditionEntityRequest : BaseConditionModel
        {
            public FieldWithMetadataModel? Uuid { get; set; } = new FieldWithMetadataModel();
        }
        #endregion

    }
}
