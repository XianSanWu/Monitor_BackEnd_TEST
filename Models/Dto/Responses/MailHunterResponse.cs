using Models.Common;
using System.ComponentModel.DataAnnotations;
namespace Models.Dto.Responses
{
    public class MailHunterResponse
    {
        public class MailHunterSearchListResponse : BaseModel
        {
            public List<MailHunterSearchListDetailResponse> SearchItem { get; set; } = new();
        }

        public class MailHunterSearchListDetailResponse
        {
            /// <summary> 專案發送的年份 </summary>
            [Display(Name = "專案發送的年份")]
            public int Year { get; set; }

            /// <summary> 專案發送的月份 </summary>
            [Display(Name = "專案發送的月份")]
            public string? Month { get; set; }

            /// <summary> 每月的專案數量 </summary>
            [Display(Name = "每月的專案數量")]
            public int ProjectCount { get; set; }

            /// <summary> 每月專案涉及的用戶總數 </summary>
            [Display(Name = "每月專案涉及的用戶總數")]
            public int ProjectOriginTotalUser { get; set; }
        }

    }
}
