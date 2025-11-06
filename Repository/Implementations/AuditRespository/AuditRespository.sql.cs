using Dapper;
using System.Text;
using Utilities.Utilities;
using static Models.Entities.Requests.AuditEntityRequest;
using static Models.Entities.Responses.AuditEntityResponse;

namespace Repository.Implementations.AuditLogRespository
{
    public partial class AuditRespository
    {
        private readonly string _Format = "yyyy-MM-dd HH:mm:ss";
        private void SaveAudit(AuditEntityCommomRequest log)
        {
            _sqlStr = new StringBuilder();

            _sqlStr.Append(@"
                INSERT INTO Audit
                (UserId, FrontUrl, FrontActionId, FrontActionName, BackActionName, HttpMethod, HttpStatusCode, RequestPath, Parameters, ResponseBody, IpAddress, CreateAt)
                VALUES (@UserId, @FrontUrl, @FrontActionId, @FrontActionName, @BackActionName, @HttpMethod, @HttpStatusCode, @RequestPath, @Parameters, @ResponseBody, @IpAddress, @CreateAt);
            ");

            _sqlParams = new DynamicParameters();
            _sqlParams.Add("@UserId", log.UserId);
            _sqlParams.Add("@FrontUrl", log.FrontUrl);
            _sqlParams.Add("@FrontActionId", log.FrontActionId);
            _sqlParams.Add("@FrontActionName", log.FrontActionName);
            _sqlParams.Add("@BackActionName", log.BackActionName);
            _sqlParams.Add("@HttpMethod", log.HttpMethod);
            _sqlParams.Add("@HttpStatusCode", log.HttpStatusCode);
            _sqlParams.Add("@RequestPath", log.RequestPath);
            _sqlParams.Add("@Parameters", log.Parameters);
            _sqlParams.Add("@ResponseBody", log.ResponseBody);
            _sqlParams.Add("@IpAddress", log.IpAddress);
            _sqlParams.Add("@CreateAt", log.CreateAt);
        }

        private void QueryAuditLog(AuditSearchListEntityRequest req)
        {
            _sqlStr = new StringBuilder();
            _sqlStr?.Append(@" 
SELECT *
FROM (
    SELECT ad.*, u.UserName
    FROM Audit ad WITH (NOLOCK)
    LEFT JOIN Users u WITH (NOLOCK) ON ad.UserId = u.Uuid
) AS ad
WHERE 1=1
AND ad.CreateAt >= @StartDate  
AND ad.CreateAt <= @EndDate
");

            _sqlParams = new DynamicParameters();

            #region  處理 FieldModel 輸入框 (模糊查詢)
            var columnsWithValues = Reflection.GetValidColumnsWithValues(req.FieldModel);

            foreach (var column in columnsWithValues)
            {
                if ("StartDate".Equals(column.Key, StringComparison.OrdinalIgnoreCase))
                {
                    string? columnValue = column.Value is DateTime dt ? dt.ToString(_Format) : column.Value?.ToString();

                    _sqlParams?.Add($"@StartDate", $"{columnValue}");
                    continue;
                }

                if ("EndDate".Equals(column.Key, StringComparison.OrdinalIgnoreCase))
                {
                    string? columnValue = column.Value is DateTime dt ? dt.ToString(_Format) : column.Value?.ToString();
                    _sqlParams?.Add($"@EndDate", $"{columnValue}");
                    continue;
                }

                AppendFilterConditionEquals($"ad.{column.Key}", column.Value, null); // 不需要驗證欄位是否有效，因為已從 model 取得
            }
            #endregion

            #region  處理 FilterModel Grid (模糊查詢)
            var validColumns = Reflection.GetValidColumns<AuditEntity>();

            if (req.FilterModel != null)
            {
                foreach (var filter in req.FilterModel)
                {
                    AppendFilterCondition($"ad.{filter.Key}", filter.Value, validColumns);
                }
            }
            #endregion

            #region  設定SQL排序
            if (req.SortModel != null &&
                !string.IsNullOrWhiteSpace(req.SortModel.Key) &&
                !string.IsNullOrWhiteSpace(req.SortModel.Value) &&
                validColumns.Contains(req.SortModel.Key, StringComparer.OrdinalIgnoreCase)
                )
            {
                _sqlOrderByStr = $" ORDER BY ad.{req.SortModel.Key} {req.SortModel.Value} ";
            }
            else
            {
                _sqlOrderByStr = $" ORDER BY ad.CreateAt DESC ";
            }
            #endregion
        }
    }
}
