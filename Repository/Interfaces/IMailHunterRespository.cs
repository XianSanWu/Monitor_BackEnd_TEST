using static Models.Dto.Responses.MailHunterResponse;
using static Models.Entities.Requests.MailHunterEntityRequest;

namespace Repository.Interfaces
{
    public interface IMailHunterRespository : IRepository
    {
        /// <summary>
        /// 查詢專案發送數量
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MailHunterSearchListResponse> GetProjectMailCountList(MailHunterSearchListEntityRequest searchReq, CancellationToken cancellationToken = default);

    }
}

