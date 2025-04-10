﻿using AutoMapper;
using Dapper;
using Models.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;
using static Models.Dto.Responses.WorkflowStepsResponse.WorkflowStepsSearchListResponse;

namespace Repository.Implementations.WorkflowStepsRespository
{
    public partial class WorkflowStepsRespository(IUnitOfWork unitOfWork, IMapper mapper)
        : BaseRepository(unitOfWork, mapper), IWorkflowStepsRespository
    {
        public async Task<WorkflowStepsSearchListResponse> QueryWorkflowStepsSearchList(WorkflowStepsSearchListRequest searchReq, CancellationToken cancellationToken = default)
        {
            #region 參數宣告

            var result = new WorkflowStepsSearchListResponse();

            #endregion

            #region 流程
            var sql = QueryWorkflowSql(searchReq);

            result.Page = searchReq.Page;
            result.SearchItem = new List<WorkflowStepsSearchResponse>();

            var _pagingSql = await GetPagingSql(searchReq.Page, _unitOfWork, _sqlParams).ConfigureAwait(false);
            var queryWorkflowEntity = (await _unitOfWork.Connection.QueryAsync<WorkflowEntity>(_pagingSql, _sqlParams).ConfigureAwait(false)).ToList();
            result.SearchItem = _mapper.Map<List<WorkflowStepsSearchResponse>>(queryWorkflowEntity);
            result.Page.TotalCount = (await _unitOfWork.Connection.QueryAsync<int?>(GetTotalCountSql(), _sqlParams).ConfigureAwait(false)).FirstOrDefault() ?? 0;

            return result;

            #endregion
        }
    }

}
