using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.Common
{
    public class BaseSearchModel
    {
        #region Properties
        /// <summary> 分頁資訊 </summary>
        PageBase? Page { get; set; }
        //public PageBase Page = new PageModel(); //用 new 方式 swagger 中不會出現

        /// <summary> CRUD的動作(含權限) </summary>
        public string? ActiveType { get; set; }

        /// <summary> 前端表頭欄位名稱 </summary>
        public string? Field { get; set; }

        /// <summary>  前端表頭排序，後端 SQL order by (asc/desc) </summary>
        public string? Sort { get; set; }
        #endregion
    }
}
