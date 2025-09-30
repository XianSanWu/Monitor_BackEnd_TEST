using AutoMapper;
using Dapper;
using Models.Entities;
using Repository.Interfaces;
using static Models.Dto.Requests.MailHunterRequest;
using static Models.Dto.Responses.MailHunterResponse;

namespace Repository.Implementations.MailHunterRespository
{
    public partial class MailHunterRespository(IUnitOfWork unitOfWork, IMapper mapper)
       : BaseRepository(unitOfWork, mapper), IMailHunterRespository, IRepository
    {

        /// <summary>
        /// 查詢專案發送數量
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MailHunterSearchListResponse> GetProjectMailCountList(MailHunterSearchListRequest searchReq, CancellationToken cancellationToken = default)
        {
            #region 參數宣告

            var result = new MailHunterSearchListResponse();

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            QueryProjectMailCountSql(searchReq);

            result.Page = searchReq.Page;
            result.SearchItem = new List<MailHunterSearchListDetailResponse>();

            var _pagingSql = await GetPagingSql(searchReq.Page, _unitOfWork, _sqlParams).ConfigureAwait(false);
            var queryWorkflowEntity = (await _unitOfWork.Connection.QueryAsync<ProjectMailCountEnyity>(_pagingSql, _sqlParams).ConfigureAwait(false)).ToList();
            result.SearchItem = _mapper.Map<List<MailHunterSearchListDetailResponse>>(queryWorkflowEntity);
            result.Page.TotalCount = (await _unitOfWork.Connection.QueryAsync<int?>(GetTotalCountSql(), _sqlParams).ConfigureAwait(false)).FirstOrDefault() ?? 0;

            return result;

            #endregion
        }
    }
}
