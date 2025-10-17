using Microsoft.AspNetCore.Mvc;
using Models.Dto.Requests;
using Models.Dto.Responses;
using WebAPi.Controllers;
using static Models.Dto.Responses.AuditResponse;

namespace WebApi.Controllers
{
    public partial class AuditController : BaseController
    {
        /// <summary>
        /// 稽核軌跡查詢
        /// </summary>
        /// <param name="req">return bool</param>
        /// <param name="cancellationToken">取消非同步</param>   
        /// <returns></returns>
        [Tags("Audit")]  //分組(可多標籤)        
        [HttpPost("SearchList")]
        public async Task<ResultResponse<AuditSearchListResponse>> QuerySearchList(AuditSearchListRequest req, CancellationToken cancellationToken)
        {
            #region 參數宣告
            var result = new AuditSearchListResponse();
            #endregion

            #region 流程
            result = await _auditService.QueryAuditLogAsync(req, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);
            #endregion
        }

    }
}