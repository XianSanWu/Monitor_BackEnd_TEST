using static Models.Dto.Requests.MailHunterRequest;
using static Models.Dto.Responses.MailHunterResponse;

namespace Repository.Interfaces
{
    public interface IMailHunterRespository
    {
        /// <summary>
        /// 查詢專案發送數量
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MailHunterSearchListResponse> GetProjectMailCountList(MailHunterSearchListRequest searchReq, CancellationToken cancellationToken = default);

    }
}

