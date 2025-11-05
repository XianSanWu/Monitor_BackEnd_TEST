
using Models.Common;

namespace Models.Dto.Responses
{
    public class PermissionResponse : BaseModel
    {
        public class PermissionSearchListResponse
        {

            #region Properties
            /// <summary>  查詢結果[清單] </summary>
            //public List<PermissionSearchResponse>? SearchItem { get; set; }
            #endregion

            public class PermissionSearchResponse
            {
                /// <summary>
                /// 主鍵
                /// </summary>
                public int Id { get; set; }

                /// <summary>
                /// 唯一識別 ID（預設 new GUID）
                /// </summary>
                public string? Uuid { get; set; }

                /// <summary>
                /// 父功能 ID（對應本表 Uuid，NULL 表示最上層）
                /// </summary>
                public string? ParentUuid { get; set; }

                /// <summary>
                /// 本層圖示
                /// </summary>
                public string? Icon { get; set; }

                /// <summary>
                /// 所屬模組（分類用途）
                /// </summary>
                public string? ModuleName { get; set; }

                /// <summary>
                /// 功能代號
                /// </summary>
                public string? FeatureName { get; set; }

                /// <summary>
                /// 顯示名稱（例如選單名稱）
                /// </summary>
                public string? Title { get; set; }

                /// <summary>
                /// 操作類型（Read, Create, Update...）
                /// </summary>
                public string? Action { get; set; }

                /// <summary>
                /// 對應路由
                /// </summary>
                public string? Link { get; set; }

                /// <summary>
                /// 權限位值
                /// </summary>
                public int BitValue { get; set; } = 0;

                /// <summary>
                /// 排序
                /// </summary>
                public int Sort { get; set; } = 1;

                /// <summary>
                /// 是否啟用
                /// </summary>
                public bool IsUse { get; set; } = true;

                /// <summary>
                /// 是否可見
                /// </summary>
                public bool IsVisible { get; set; } = true;

            }

        }

    }
}
