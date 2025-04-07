using Models.Dto.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.Responses
{
    public class WorkflowStepsResponse
    {
        #region 工作流程查詢List回傳
        public class WorkflowStepsSearchListResponse : BaseModel
        {
            #region Properties
            /// <summary>  查詢結果[清單] </summary>
            public List<WorkflowStepsSearchResponse>? SearchItem { get; set; }
            #endregion

            /// <summary> 查詢結果[欄位] </summary>
            public class WorkflowStepsSearchResponse 
            {
                /// <summary>主鍵</summary>
                public int SN { get; set; }

                /// <summary>工作流程唯一識別碼</summary>
                public string? WorkflowUuid { get; set; }

                /// <summary>發送 UUID</summary>
                public string? SendUuid { get; set; }

                /// <summary>批次 ID</summary>
                public string? BatchId { get; set; }

                /// <summary>旅程 ID</summary>
                public string? JourneyId { get; set; }

                /// <summary>旅程名稱</summary>
                public string? JourneyName { get; set; }

                /// <summary>旅程狀態</summary>
                public string? JourneyStatus { get; set; }

                /// <summary>節點 ID</summary>
                public string? NodeId { get; set; }

                /// <summary>節點名稱</summary>
                public string? NodeName { get; set; }

                /// <summary>通道 (Email / SMS)</summary>
                public string? Channel { get; set; }

                /// <summary>通道類型 (EDM / 簡訊)</summary>
                public string? ChannelType { get; set; }

                /// <summary>上傳檔名</summary>
                public string? UploadFileName { get; set; }

                /// <summary>狀態</summary>
                public string? Status { get; set; }

                /// <summary>建立時間</summary>
                public DateTime CreateAt { get; set; }

                /// <summary>更新時間</summary>
                public DateTime UpdateAt { get; set; }

                /// <summary>旅程建立時間</summary>
                public DateTime JourneyCreateAt { get; set; }

                /// <summary>旅程更新時間</summary>
                public DateTime JourneyUpdateAt { get; set; }

                /// <summary>群發建立時間</summary>
                public DateTime GroupSendCreateAt { get; set; }

                /// <summary>群發更新時間</summary>
                public DateTime GroupSendUpdateAt { get; set; }
            }
            #endregion
        }
    }
}
