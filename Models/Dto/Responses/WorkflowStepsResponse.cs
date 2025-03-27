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
        public class WorkflowStepsSearchListResponse
        {
            #region Properties
            /// <summary>  查詢結果[清單] </summary>
            public List<WorkflowStepsSearchResponse>? WorkflowStepsSearchResponseList { get; set; }
            #endregion

            /// <summary> 查詢結果[欄位] </summary>
            public class WorkflowStepsSearchResponse : BaseModel
            {
                /// <summary> 節點ID </summary>
                public string? NodeId { get; set; }

                /// <summary> 節點名稱 </summary>
                public string? NodeName { get; set; }

                /// <summary> 旅程IG </summary>
                public string? JourneyId { get; set; }

                /// <summary> 旅程名稱 </summary>
                public string? JourneyName { get; set; }

                /// <summary> 來源 </summary>
                public string? Channel { get; set; }

                /// <summary> 處理狀態 </summary>
                public string? Status { get; set; }

                /// <summary> 起始時間 </summary>
                public string? CreateAt { get; set; }

                /// <summary> 更新時間 </summary>
                public string? UpdateAt { get; set; }
            }
            #endregion
        }
    }
}
