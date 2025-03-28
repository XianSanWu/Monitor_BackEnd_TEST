﻿using FluentValidation;
using FluentValidation.Results;
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
        [Tags("WorkflowSteps")]  //分組(可多標籤)        
        [HttpPost("SearchList")]
        public async Task<ResultResponse<WorkflowStepsResponse>> QuerySearchList(WorkflowStepsSearchListRequest searchReq, CancellationToken cancellationToken)
        {
            #region 參數宣告
            var result = new WorkflowStepsResponse();
            ValidationResult searchRequestValidationResult;
            #endregion

            #region 參數驗證
            searchRequestValidationResult = await _searchListRequestValidator.ValidateAsync(searchReq, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("WorkflowStepsSearchListRequest 參數：{@WorkflowStepsSearchListRequest}", searchReq);
            _logger.LogInformation("WorkflowStepsSearchListRequest 驗證：{ValidationResult}", searchRequestValidationResult);

            if (!string.IsNullOrWhiteSpace(searchRequestValidationResult.ToString()))
            {
                return FailResult<WorkflowStepsResponse>($"參數檢核未通過：{searchRequestValidationResult}");
            }
            #endregion

            #region 流程

            result = await _workflowStepsService.QuerySearchList(searchReq, _config, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);

            #endregion

        }
    }
}
