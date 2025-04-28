using Microsoft.AspNetCore.Mvc;
using Models.Dto.Responses;
using static Models.Dto.Requests.MsmqQueueRequest;
using static Models.Dto.Responses.MsmqQueueResponse;

namespace WebAPi.Controllers
{
    public partial class MsmqController : BaseController
    {
        /// <summary>
        /// 查詢專案發送數量
        /// </summary>
        /// <param name="searchReq">前端傳入的查詢條件</param>
        /// <param name="cancellationToken">取消非同步</param>
        /// <returns name="result">查詢結果 </returns>
        [Tags("MSMQ")]  //分組(可多標籤)        
        [HttpPost("SearchList")]
        public async Task<ResultResponse<MsmqQueueDetailsResponse>> QuerySearchList(MsmqQueueInfoRequest searchReq, CancellationToken cancellationToken)
        {
            #region 參數宣告
            var result = new MsmqQueueDetailsResponse();
            #endregion

            #region 流程

            result = await _msmqService.GetAllQueueInfo(searchReq, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);

            #endregion

        }

    }
}
