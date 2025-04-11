using Dapper;
using Models.Entities;
using Repository.Interfaces;
using System.Text;
using Utilities.Utilities;
using static Models.Dto.Requests.WorkflowStepsRequest;

namespace Repository.Implementations.WorkflowStepsRespository
{
    public partial class WorkflowStepsRespository : BaseRepository, IWorkflowStepsRespository
    {
        private bool QueryWorkflowSql(WorkflowStepsSearchListRequest searchReq)
        {
            _sqlStr = new StringBuilder();
            _sqlStr.Append(@" SELECT * FROM Workflow WHERE 1=1 ");

            _sqlParams = new DynamicParameters();

            #region  處理 FieldModel 輸入框 (模糊查詢)
            var columnsWithValues = Reflection.GetValidColumnsWithValues(searchReq.FieldModel);

            foreach (var column in columnsWithValues)
            {
                AppendFilterCondition(column.Key, column.Value, null); // 不需要驗證欄位是否有效，因為已從 model 取得
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

            return true;
        }

    }
}
