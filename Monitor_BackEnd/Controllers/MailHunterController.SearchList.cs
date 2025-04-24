using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Dto.Responses;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;

namespace WebAPi.Controllers
{
    public partial class MailHunterController : BaseController
    {
        /// <summary>
        /// 查詢專案發送數量
        /// </summary>
        /// <param name="searchReq">前端傳入的查詢條件</param>
        /// <param name="cancellationToken">取消非同步</param>
        /// <returns name="result">查詢結果 </returns>
        [Tags("WorkflowSteps")]  //分組(可多標籤)        
        [HttpPost("SearchLastList")]
        public async Task<ResultResponse<WorkflowStepsSearchListResponse>> QuerySearchLastList(WorkflowStepsSearchListRequest searchReq, CancellationToken cancellationToken)
        {
            #region 參數宣告
            var result = new WorkflowStepsSearchListResponse();
            #endregion

            #region 流程

            //result = await _mailHunterService.QueryWorkflowStepsSearchLastList(searchReq, _config, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);

            #endregion

        }
    }
}
