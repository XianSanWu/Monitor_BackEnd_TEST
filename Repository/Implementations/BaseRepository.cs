using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Models.Dto.Common;
using Repository.Interfaces;
using System.Text;

namespace Repository.Implementations
{
    public class BaseRepository(IUnitOfWork unitOfWork, IMapper mapper)
    {

        #region 屬性
        //private readonly string? _connectionString;
        protected StringBuilder? _sqlWithStr;
        protected StringBuilder? _sqlStr;
        protected string? _sqlOrderByStr;
        protected DynamicParameters? _sqlParams;
        protected List<DynamicParameters>? _sqlParamsList;
        //protected SqlConnection SQLConnection;
        //protected SqlTransaction SQLTransaction;
        protected readonly IUnitOfWork _unitOfWork = unitOfWork;
        protected readonly IMapper _mapper = mapper;
        #endregion

        /// <summary>
        /// SQL語法(查詢+分頁)
        /// 需確認SQL語法有ORDER BY語法才不會有問題。
        /// </summary>
        /// <param name="page"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected async Task<string> GetPagingSql(PageBase? page, IUnitOfWork unitOfWork, DynamicParameters? sqlParams)
        {
            #region 參數宣告

            StringBuilder pageSqlStr = new();

            #endregion

            #region 流程

            //驗證是否有ORDER BY 語法
            //if (string.IsNullOrWhiteSpace(_sqlStr) || string.IsNullOrWhiteSpace(_sqlOrderByStr))
            if (string.IsNullOrWhiteSpace(_sqlStr?.ToString()))
            {
                throw new Exception("未傳入SQL語法");
            }
            else if (string.IsNullOrWhiteSpace(_sqlOrderByStr))
            {
                throw new Exception("未傳入SQL ORDER BY語法");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(_sqlOrderByStr) && !_sqlOrderByStr.Contains("ORDER BY", StringComparison.CurrentCultureIgnoreCase))
                {
                    throw new Exception("SQL語法需含 ORDER BY");
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(_sqlWithStr?.ToString()))
                    {
                        pageSqlStr.AppendLine(_sqlWithStr.ToString());
                    }

                    pageSqlStr.AppendLine(_sqlStr.ToString());
                    pageSqlStr.AppendLine(_sqlOrderByStr);

                    if (page != null)
                    {
                        //不給分頁參數時，預設抓前50筆資料，(e.q. 下拉選單、搜尋出來的一頁顯示50筆資料)
                        if ((page.PageSize == 0 || page.PageSize == 1))
                        {
                            page.PageSize = 50;     //對應前端 SearchRequestViewModel.Page.PageSize 
                        }

                        if (page.PageIndex == 0)
                        {
                            page.PageIndex = 1;
                        }

                        page.PageIndex = await GetLastPageIndex(page.PageSize, page.PageIndex, unitOfWork, sqlParams).ConfigureAwait(false);

                        pageSqlStr.AppendLine(string.Format(" OFFSET {0} ROWS ", page.GetStartIndex()));
                        pageSqlStr.AppendLine(string.Format(" FETCH FIRST {0} ROWS ONLY;", page.PageSize));
                    }

                }
            }

            return pageSqlStr.ToString();

            #endregion

        }

        /// <summary>
        /// 計算總筆數
        /// </summary>
        /// <returns></returns>
        protected string GetTotalCountSql()
        {

            #region 參數宣告

            StringBuilder pageSqlStr = new StringBuilder();

            #endregion

            #region 流程

            //驗證是否有ORDER BY 語法
            if (string.IsNullOrWhiteSpace(_sqlStr?.ToString()))
            {
                throw new Exception("未傳入SQL語法");
            }
            else
            {
                pageSqlStr.AppendLine(string.Format("{0} SELECT COUNT(1) FROM ({1}) AS CNT", _sqlWithStr, _sqlStr));
            }

            return pageSqlStr.ToString();

            #endregion

        }


        /// <summary>
        /// 計算總頁數(若嘗試取得頁數超過總頁數，則回傳最後一頁)
        /// https://learn.microsoft.com/zh-tw/dotnet/framework/data/adonet/asynchronous-programming
        /// </summary>
        /// <param name="PageSize">請求查詢的頁數</param>
        /// <param name="PageIndex"></param>
        /// <param name="unitOfWork">資料庫連結字串</param>
        /// <param name="sqlParams">SqlParameter</param>
        /// <returns></returns>
        protected async Task<int> GetLastPageIndex(int PageSize, int PageIndex, IUnitOfWork unitOfWork, DynamicParameters? sqlParams)
        {
            int TotalCount = 0;

            // 不要 new SqlConnection，直接用 unitOfWork.Connection
            var conn = (SqlConnection)unitOfWork.Connection; // cast 你的 DBConnection 為 SqlConnection
            using (var cmd = conn.CreateCommand())
            {
                string SqlStr = string.Format("{0} SELECT COUNT(1) as totalCount FROM ({1}) AS CNT", _sqlWithStr, _sqlStr);
                cmd.CommandText = SqlStr;

                // 指定 Transaction，如果有的話
                if (unitOfWork.Transaction != null)
                    cmd.Transaction = (SqlTransaction)unitOfWork.Transaction;

                // 組 sql parameter
                if (sqlParams != null && sqlParams.ParameterNames.Any())
                {
                    foreach (var name in sqlParams.ParameterNames)
                    {
                        cmd.Parameters.Add(new SqlParameter(name, sqlParams.Get<dynamic>(name)));
                    }
                }

                using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        TotalCount = Convert.ToInt32(reader["totalCount"]);
                    }
                }
            }

            int TotalPage = (int)Math.Ceiling((decimal)TotalCount / PageSize);
            return PageIndex >= TotalPage ? TotalPage : PageIndex;
        }


        /// <summary>
        /// 動態添加欄位條件篩選(必須相等)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="validColumns"></param>
        protected void AppendFilterConditionEquals(string? key, object? value, HashSet<string>? validColumns)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null || string.IsNullOrWhiteSpace(value?.ToString()))
                return;

            if (validColumns != null)
            {
                var keyToCompare = key.Split('.').Last(); // 取最後一段
                if (!validColumns.Contains(keyToCompare, StringComparer.OrdinalIgnoreCase))
                {
                    // 不存在於 validColumns 中
                    return;
                }
            }

            var queryKey = key.Replace(".", "_");

            if (key.EndsWith("At", StringComparison.OrdinalIgnoreCase))
            {
                _sqlStr?.Append($" AND CONVERT(VARCHAR, {key}, 121) LIKE @{queryKey} ");
                _sqlParams?.Add($"@{queryKey}", $"%{value}%");
            }
            else
            {
                _sqlStr?.Append($" AND {key} = @{queryKey} ");
                _sqlParams?.Add($"@{queryKey}", $"{value}");
            }

        }

        /// <summary>
        /// 動態添加欄位條件篩選
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="validColumns"></param>
        protected void AppendFilterCondition(string? key, object? value, HashSet<string>? validColumns)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null || string.IsNullOrWhiteSpace(value?.ToString()))
                return;

            if (validColumns != null)
            {
                var keyToCompare = key.Split('.').Last(); // 取最後一段
                if (!validColumns.Contains(keyToCompare, StringComparer.OrdinalIgnoreCase))
                {
                    // 不存在於 validColumns 中
                    return;
                }
            }

            var queryKey = key.Replace(".", "_");

            if (key.EndsWith("At", StringComparison.OrdinalIgnoreCase))
            {
                _sqlStr?.Append($" AND CONVERT(VARCHAR, {key}, 121) LIKE @{queryKey} ");
            }
            else
            {
                _sqlStr?.Append($" AND {key} LIKE @{queryKey} ");
            }

            _sqlParams?.Add($"@{queryKey}", $"%{value}%");
        }

    }
}
