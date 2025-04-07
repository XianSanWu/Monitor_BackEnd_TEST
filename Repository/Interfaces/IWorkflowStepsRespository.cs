using Microsoft.Extensions.Configuration;
using Models.Dto.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;

namespace Repository.Interfaces
{
    public interface IWorkflowStepsRespository
    {
        /// <summary>
        /// 【旅程】工作進度查詢
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<WorkflowStepsSearchListResponse> QueryJourneySearchList(WorkflowStepsSearchListRequest searchReq, CancellationToken cancellationToken = default);

        /// <summary>
        /// 【群發】工作進度查詢
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<WorkflowStepsSearchListResponse> QueryGroupSendSearchList(WorkflowStepsSearchListRequest searchReq, CancellationToken cancellationToken = default);
    }
}
