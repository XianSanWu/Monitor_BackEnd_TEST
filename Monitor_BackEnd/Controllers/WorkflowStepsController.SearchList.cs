using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Dto.Responses;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;

namespace WebAPi.Controllers
{
    public partial class WorkflowStepsController : BaseController
    {
        /// <summary>
        /// 查詢結果集合(多筆)
        /// </summary>
        /// <param name="searchReq">前端傳入的查詢條件</param>
        /// <param name="cancellationToken">取消非同步</param>
        /// <returns name="result">查詢結果 </returns>
        [Tags("WorkflowSteps")]  //分組(可多標籤)        
        [HttpPost("SearchList")]
        public async Task<ResultResponse<WorkflowStepsSearchListResponse>> QuerySearchList(WorkflowStepsSearchListRequest searchReq, CancellationToken cancellationToken)
        {
            #region 參數宣告
            var result = new WorkflowStepsSearchListResponse();
            //ValidationResult searchRequestValidationResult;
            #endregion

            #region 參數驗證
            //searchRequestValidationResult = await _searchListRequestValidator.ValidateAsync(searchReq, cancellationToken).ConfigureAwait(false);

            //_logger.LogInformation("WorkflowStepsSearchListRequest 參數：{@WorkflowStepsSearchListRequest}", searchReq);
            //_logger.LogInformation("WorkflowStepsSearchListRequest 驗證：{ValidationResult}", searchRequestValidationResult);

            //if (!string.IsNullOrWhiteSpace(searchRequestValidationResult.ToString()))
            //{
            //    return FailResult<WorkflowStepsSearchListResponse>($"參數檢核未通過：{searchRequestValidationResult}");
            //}
            #endregion

            #region 流程

            result = await _workflowStepsService.QueryWorkflowStepsSearchList(searchReq, _config, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);

            #endregion

        }

        /// <summary>
        /// 取得卡夫卡工作量
        /// </summary>
        /// <param name="channel">來源(EDM/SMS/APP)</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("WorkflowSteps")]  //分組(可多標籤)        
        [HttpPost("GetKafkaLag")]
        public async Task<ResultResponse<WorkflowStepsKafkaResponse>> GetKafkaLag(WorkflowStepsKafkaRequest req, CancellationToken cancellationToken)
        {
            var result = new WorkflowStepsKafkaResponse();
            #region 流程
            result = await _workflowStepsService.GetKafkaLag(req, _config, cancellationToken).ConfigureAwait(false);
            
            return SuccessResult(result);
            #endregion
        }


    }
}
