
namespace Models.Entities
{
    public class ProjectMailCountEnyity
    {
        /// <summary> 專案發送的年份 </summary>
        public int Year { get; set; }

        /// <summary> 專案發送的月份 </summary>
        public string? Month { get; set; }

        /// <summary> 每月的專案數量 </summary>
        public int ProjectCount { get; set; }

        /// <summary> 每月專案涉及的用戶總數 </summary>
        public int ProjectOriginTotalUser { get; set; }

        /// <summary> 專案發送的月份排序 </summary>
        public int MonthSort { get; set; }

    }
}
