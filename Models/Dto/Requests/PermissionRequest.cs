using Models.Dto.Common;

namespace Models.Dto.Requests
{
    public class PermissionRequest
    {
        #region 查詢用
        public class PermissionSearchListRequest : BaseSearchModel
        {

        }
        #endregion

        #region 更新[欄位]
        /// <summary> 工作流程更新[欄位] </summary>
        public class PermissionUpdateFieldRequest
        {
            /// <summary>主鍵</summary>
            //public int SN { get; set; }
        }
        #endregion

        #region 更新[條件]
        /// <summary>工作流程更新[條件]</summary>
        public class PermissionUpdateConditionRequest : BaseConditionModel
        {
            /// <summary>主鍵</summary>
            //public FieldWithMetadataModel SN { get; set; } = new();
        }
        #endregion

    }
}
