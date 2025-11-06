using AutoMapper;
using Dapper;
using Repository.Interfaces;
using static Models.Entities.Requests.MailHunterEntityRequest;
using static Models.Entities.Responses.ProjectMailCountEntityResponse;

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
        public async Task<ProjectMailCountEntitySearchListResponse> GetProjectMailCountList(MailHunterEntitySearchListRequest searchReq, CancellationToken cancellationToken = default)
        {
            #region 參數宣告

            var result = new ProjectMailCountEntitySearchListResponse();

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            QueryProjectMailCountSql(searchReq);

            var _pagingSql = await GetPagingSql(searchReq.Page, _unitOfWork, _sqlParams).ConfigureAwait(false);
            result.Page = searchReq.Page;
            result.SearchItem = (await _unitOfWork.Connection.QueryAsync<ProjectMailCountEntity>(_pagingSql, _sqlParams).ConfigureAwait(false)).ToList();
            result.Page.TotalCount = (await _unitOfWork.Connection.QueryAsync<int?>(GetTotalCountSql(), _sqlParams).ConfigureAwait(false)).FirstOrDefault() ?? 0;

            return result;

            #endregion
        }
    }
}
