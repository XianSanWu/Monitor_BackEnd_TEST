using Dapper;
using Models.Entities;
using Repository.Implementations;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Utilities;
using YamlDotNet.Core;
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

            // 獲取模型的有效欄位（使用反射）//輸入框
            var columnsWithValues = Reflection.GetValidColumnsWithValues(searchReq.FieldModel);

            foreach (var column in columnsWithValues)
            {
                if (column.Value != null)
                {
                    _sqlStr.Append($" AND {column.Key} LIKE @{column.Key} ");
                    _sqlParams.Add($"@{column.Key}", $"%{column.Value}%");
                }
                //Console.WriteLine($"Key: {column.Key}, Value: {column.Value}");
            }


            // 獲取模型的有效欄位（使用反射）//grid
            // 根據模型類型來查找有效欄位，只有模型中的屬性會被考慮，避免sql注入風險
            var validColumns = Reflection.GetValidColumns<WorkflowEntity>();

            //設定grid欄位模糊查詢
            if (searchReq.FilterModel != null)
            {
                foreach (var filter in searchReq.FilterModel)
                {
                    if (string.IsNullOrWhiteSpace(filter.Value) || !validColumns.Contains(filter.Key, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    _sqlStr.Append($" AND {filter.Key} LIKE @{filter.Key} ");
                    _sqlParams.Add($"@{filter.Key}", $"%{filter.Value}%");
                }
            }
            
            //設定SQL排序
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


            return true;
        }

    }
}
