using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utilities.ValidatorsUtil
{
    public static class RegExpUtil
    {
        // 客戶 ID 搜尋 (最多 2 個大寫字母 + 最多 9 個數字)
        public static readonly Regex CustIdSearch = new(@"^[A-Z]{0,2}[0-9]{0,9}$", RegexOptions.Compiled);

        // 整數 (正數)
        public static readonly Regex Int = new(@"^[0-9]*$", RegexOptions.Compiled);

        // 正負整數
        public static readonly Regex IsNumeric = new(@"^-?\d+$", RegexOptions.Compiled);

        // yyyy-MM-dd 格式日期
        public static readonly Regex DateDashAD = new(@"^\d{4}-\d{2}-\d{2}$", RegexOptions.Compiled);

        // 四位數年份 yyyy
        public static readonly Regex YYYY = new(@"^\d{4}$", RegexOptions.Compiled);

        // yyyy-MM 年份 + 月份
        public static readonly Regex YYYYMM = new(@"^\d{4}-\d{2}$", RegexOptions.Compiled);

        // yyyy-MM-dd 或 yyyy/MM/dd 或 yyyy.MM.dd
        public static readonly Regex DateFmt1 = new(@"^\d{4}[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])$", RegexOptions.Compiled);

        // yyyyMMdd 純數字日期
        public static readonly Regex DateFmt2 = new(@"^\d{4}(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])$", RegexOptions.Compiled);

        // MM/dd/yyyy
        public static readonly Regex DateFmt3 = new(@"^(0[1-9]|1[0-2])\/(0[1-9]|[12][0-9]|3[01])\/(19|20)\d\d$", RegexOptions.Compiled);

        // 中文字符檢查
        public static readonly Regex Chinese = new(@"[\u4e00-\u9fa5]", RegexOptions.Compiled);

        // 空白與換行符號
        public static readonly Regex WhiteSpaceAndNewLines = new(@"\s", RegexOptions.Compiled);

        // 中文、英文、數字、底線、連字符
        public static readonly Regex RegexChineseEnglishNumbers = new(@"^[\u4e00-\u9fa5a-zA-Z0-9_-]+$", RegexOptions.Compiled);

        // 英文、數字、底線、連字符
        public static readonly Regex RegexEnglishNumbers = new(@"^[a-zA-Z0-9_-]+$", RegexOptions.Compiled);

        // 英文、數字、特殊符號
        public static readonly Regex RegexSymbolsEnglishNumbers = new(@"^[a-zA-Z0-9_!\-@#&*()+=<>?^,.:;]+$", RegexOptions.Compiled);

        /// <summary>
        /// 驗證輸入是否符合指定的正則表達式
        /// </summary>
        public static bool IsMatch(Regex regex, string? input = "")
        {
            return !string.IsNullOrEmpty(input) && regex.IsMatch(input);
        }
    }
}
