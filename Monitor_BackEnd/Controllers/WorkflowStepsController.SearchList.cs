using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Dto.Responses;
using static Models.Dto.Requests.WorkflowStepsRequest;

namespace WebAPi.Controllers
{
    public partial class WorkflowStepsController : BaseController
    {
        /// <summary>
        /// 查詢結果集合(多筆)
        /// </summary>
        /// <param name="searchInfo">前端傳入的查詢條件</param>
        /// <param name="cancellationToken">取消非同步</param>
        /// <returns name="SearchListModel">查詢結果 </returns>
        //[Tags("WorkflowSteps")]  //分組(可多標籤)        
        //[HttpPost("SearchList")]
        //public async Task<ResultResponse<WorkflowStepsResponse>> QuerySearchList(WorkflowStepsSearchListRequest searchReq, CancellationToken cancellationToken)
        //{
            //#region 參數宣告
            //var searchList = new WorkflowStepsSearchListRequest();
            //ValidationResult searchRequestValidationResult;
            //#endregion

            //#region 參數驗證
            //searchRequestValidationResult = await _searchRequestValidator.ValidateAsync(searchInfo).ConfigureAwait(false);

            //_logger.LogInformation("QuerySearchList 引數為 {ValidationResult}", searchRequestValidationResult);

            //if (!String.IsNullOrWhiteSpace(searchRequestValidationResult.ToString()))
            //{
            //    return FailResult<SearchListModel>($"引數檢核未通過：{searchRequestValidationResult}");
            //}
            //#endregion

            //#region 流程

            //// 檢查日誌記錄等級
            //bool shouldLog = LogExtension.ShouldLog();
            //// 判斷是否記錄
            //if (shouldLog)
            //{
            //    _logger.LogInformation("Recording log: This is an information log.");
            //}


            //_searchList = await _demoService.QuerySearchList(searchInfo, _config, ct).ConfigureAwait(false);

            //return SuccessResult(_searchList);

        //    #endregion

        //}
    }
}
