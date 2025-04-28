using Microsoft.AspNetCore.Mvc;
using Models.Dto.Responses;
using static Models.Dto.Requests.MailHunterRequest;
using static Models.Dto.Responses.MailHunterResponse;

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
        [Tags("MailHunter")]  //分組(可多標籤)        
        [HttpPost("ProjectMailCountSearchList")]
        public async Task<ResultResponse<MailHunterSearchListResponse>> GetProjectMailCountList(MailHunterSearchListRequest searchReq, CancellationToken cancellationToken)
        {
            #region 參數宣告
            var result = new MailHunterSearchListResponse();
            #endregion

            #region 流程

            result = await _mailHunterService.GetProjectMailCountList(searchReq, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);

            #endregion

        }
    }
}
