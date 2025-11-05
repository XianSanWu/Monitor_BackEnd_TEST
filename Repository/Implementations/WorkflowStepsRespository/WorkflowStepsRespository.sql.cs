using Dapper;
using Repository.Interfaces;
using System.Text;
using Utilities.Utilities;
using static Models.Entities.Requests.WorkflowStepsEntityRequest;
using static Models.Entities.Responses.WorkflowEntityResponse;

namespace Repository.Implementations.WorkflowStepsRespository
{
    public partial class WorkflowStepsRespository : BaseRepository, IWorkflowStepsRespository
    {
        private static readonly string[] sourceArray = ["SendUuid", "SendUuidSort"];

        /// <summary>
        /// 工作進度查詢DB (最後一筆)
        /// </summary>
        /// <param name="searchReq"></param>
        private void QueryWorkflowLastSql(WorkflowStepsSearchListEntityRequest searchReq)
        {
            _sqlStr = new StringBuilder();
            _sqlStr?.Append(@"
SELECT wf.*
FROM Workflow wf WITH (NOLOCK)
JOIN (
    SELECT SendUuidSort, MAX(BatchIdSort) AS MaxBatchIdSort
    FROM Workflow WITH (NOLOCK)
    GROUP BY SendUuidSort
) maxTable
  ON wf.SendUuidSort = maxTable.SendUuidSort
 AND wf.BatchIdSort = maxTable.MaxBatchIdSort
");

            _sqlParams = new DynamicParameters();

            #region  處理 FieldModel 輸入框 (模糊查詢)
            var columnsWithValues = Reflection.GetValidColumnsWithValues(searchReq.FieldModel);

            foreach (var column in columnsWithValues)
            {
                var column_key = column.Key;
                if (sourceArray.Any(item => item.Equals(column.Key, StringComparison.OrdinalIgnoreCase)))
                {
                    column_key = $"wf.{column.Key}";
                }

                AppendFilterConditionEquals(column_key, column.Value, null); // 不需要驗證欄位是否有效，因為已從 model 取得
            }
            #endregion

            #region  處理 FilterModel Grid (模糊查詢)
            var validColumns = Reflection.GetValidColumns<WorkflowEntity>();

            if (searchReq.FilterModel != null)
            {
                foreach (var filter in searchReq.FilterModel)
                {
                    var filter_key = filter.Key;
                    if (sourceArray.Any(item => item.Equals(filter.Key, StringComparison.OrdinalIgnoreCase)))
                    {
                        filter_key = $"wf.{filter.Key}";
                    }
                    AppendFilterCondition(filter_key, filter.Value, validColumns);
                }
            }
            #endregion


            #region  設定SQL排序
            if (searchReq.SortModel != null &&
                !string.IsNullOrWhiteSpace(searchReq.SortModel.Key) &&
                !string.IsNullOrWhiteSpace(searchReq.SortModel.Value) &&
                validColumns.Contains(searchReq.SortModel.Key, StringComparer.OrdinalIgnoreCase)
                )
            {
                var SortKey = searchReq.SortModel.Key;
                if (sourceArray.Any(item => item.Equals(searchReq.SortModel.Key, StringComparison.OrdinalIgnoreCase)))
                {
                    SortKey = $"wf.{searchReq.SortModel.Key}";
                }

                _sqlOrderByStr = $" ORDER BY {SortKey} {searchReq.SortModel.Value} ";
            }
            else
            {
                _sqlOrderByStr = $" ORDER BY CreateAt DESC ";
            }
            #endregion

        }


        /// <summary>
        /// 工作進度查詢DB
        /// </summary>
        /// <param name="searchReq"></param>
        private void QueryWorkflowSql(WorkflowStepsSearchListEntityRequest searchReq)
        {
            _sqlStr = new StringBuilder();
            _sqlStr?.Append(@" SELECT * FROM Workflow WITH (NOLOCK) WHERE 1=1 ");

            _sqlParams = new DynamicParameters();

            #region  處理 FieldModel 輸入框 (模糊查詢)
            var columnsWithValues = Reflection.GetValidColumnsWithValues(searchReq.FieldModel);

            foreach (var column in columnsWithValues)
            {
                AppendFilterConditionEquals(column.Key, column.Value, null); // 不需要驗證欄位是否有效，因為已從 model 取得
            }
            #endregion

            #region  處理 FilterModel Grid (模糊查詢)
            var validColumns = Reflection.GetValidColumns<WorkflowEntity>();

            if (searchReq.FilterModel != null)
            {
                foreach (var filter in searchReq.FilterModel)
                {
                    AppendFilterCondition(filter.Key, filter.Value, validColumns);
                }
            }
            #endregion

            #region  初版模糊查詢
            //// 獲取模型的有效欄位（使用反射）//輸入框
            //var columnsWithValues = Reflection.GetValidColumnsWithValues(searchReq.FieldModel);

            //foreach (var column in columnsWithValues)
            //{
            //    if (column.Value != null)
            //    {
            //        if (column.Key.EndsWith("At", StringComparison.OrdinalIgnoreCase))
            //        {
            //            // 假設此欄位在資料庫中是 DATETIME 類型，進行模糊查詢
            //            _sqlStr.Append($" AND CONVERT(VARCHAR, {column.Key}, 121) LIKE @{column.Key} ");
            //            _sqlParams.Add($"@{column.Key}", $"%{column.Value}%");
            //        }
            //        else
            //        {
            //            _sqlStr.Append($" AND {column.Key} LIKE @{column.Key} ");
            //            _sqlParams.Add($"@{column.Key}", $"%{column.Value}%");
            //        }
            //    }
            //    //Console.WriteLine($"Key: {column.Key}, Value: {column.Value}");
            //}


            //// 獲取模型的有效欄位（使用反射）//grid
            //// 根據模型類型來查找有效欄位，只有模型中的屬性會被考慮，避免sql注入風險
            //var validColumns = Reflection.GetValidColumns<WorkflowEntity>();

            ////設定grid欄位模糊查詢
            //if (searchReq.FilterModel != null)
            //{
            //    foreach (var filter in searchReq.FilterModel)
            //    {
            //        if (string.IsNullOrWhiteSpace(filter.Key) || string.IsNullOrWhiteSpace(filter.Value) || !validColumns.Contains(filter.Key, StringComparer.OrdinalIgnoreCase))
            //        {
            //            continue;
            //        }

            //        // 如果 filter.Key 以 "At" 結尾，進行 datetime 處理
            //        if (filter.Key.EndsWith("At", StringComparison.OrdinalIgnoreCase))
            //        {
            //            // 假設此欄位在資料庫中是 DATETIME 類型，進行模糊查詢
            //            _sqlStr.Append($" AND CONVERT(VARCHAR, {filter.Key}, 121) LIKE @{filter.Key} ");
            //            _sqlParams.Add($"@{filter.Key}", $"%{filter.Value}%");
            //        }
            //        else
            //        {
            //            // 否則，正常處理字串模糊查詢
            //            _sqlStr.Append($" AND {filter.Key} LIKE @{filter.Key} ");
            //            _sqlParams.Add($"@{filter.Key}", $"%{filter.Value}%");
            //        }
            //    }
            //}
            #endregion

            #region  設定SQL排序
            if (searchReq.SortModel != null &&
                !string.IsNullOrWhiteSpace(searchReq.SortModel.Key) &&
                !string.IsNullOrWhiteSpace(searchReq.SortModel.Value) &&
                validColumns.Contains(searchReq.SortModel.Key, StringComparer.OrdinalIgnoreCase)
                )
            {
                _sqlOrderByStr = $" ORDER BY {searchReq.SortModel.Key} {searchReq.SortModel.Value} ";
            }
            else
            {
                _sqlOrderByStr = $" ORDER BY CreateAt DESC ";
            }
            #endregion

        }

    }
}
