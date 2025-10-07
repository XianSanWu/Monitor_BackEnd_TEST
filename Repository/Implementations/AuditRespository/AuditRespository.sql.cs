using Dapper;
using Models.Dto.Requests;
using System.Text;

namespace Repository.Implementations.AuditLogRespository
{
    public partial class AuditRespository
    {
        private void SaveAuditLog(AuditRequest log)
        {
            _sqlStr = new StringBuilder();

            _sqlStr.Append(@"
                INSERT INTO Audit
                (UserId, FrontUrl, FrontActionName, BackActionName, HttpMethod, HttpStatusCode, RequestPath, Parameters, IpAddress, CreateAt)
                VALUES (@UserId, @FrontUrl, @FrontActionName, @BackActionName, @HttpMethod, @HttpStatusCode, @RequestPath, @Parameters, @IpAddress, @CreateAt);
            ");

            _sqlParams = new DynamicParameters();
            _sqlParams.Add("@UserId", log.UserId);
            _sqlParams.Add("@FrontUrl", log.FrontUrl);
            _sqlParams.Add("@FrontActionName", log.FrontActionName);
            _sqlParams.Add("@BackActionName", log.BackActionName);
            _sqlParams.Add("@HttpMethod", log.HttpMethod);
            _sqlParams.Add("@HttpStatusCode", log.HttpStatusCode);
            _sqlParams.Add("@RequestPath", log.RequestPath);
            _sqlParams.Add("@Parameters", log.Parameters);
            _sqlParams.Add("@IpAddress", log.IpAddress);
            _sqlParams.Add("@CreateAt", log.CreateAt);
        }
    }
}
