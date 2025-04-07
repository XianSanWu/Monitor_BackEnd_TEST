using Dapper;
using Repository.Implementations;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            //設定grid欄位模糊查詢
            if (searchReq.FilterModel != null)
            {
                foreach (var filter in searchReq.FilterModel)
                {
                    _sqlStr.Append($" AND {filter.Key} LIKE @{filter.Key} ");
                    _sqlParams.Add($"@{filter.Key}", $"%{filter.Value}%");
                }
            }

            //設定SQL排序
            if (searchReq.SortModel != null && !string.IsNullOrWhiteSpace(searchReq.SortModel.Key) && !string.IsNullOrWhiteSpace(searchReq.SortModel.Value)) {
                _sqlOrderByStr = $"ORDER BY {searchReq.SortModel.Key} {searchReq.SortModel.Value}";
            }


            return true;
        }

    }
}
