using static Models.Dto.Requests.MailHunterRequest;
using static Models.Dto.Responses.MailHunterResponse;

namespace Services.Interfaces
{
    public interface IMailHunterService
    {

        /// <summary>
        /// 查詢專案發送數量
        /// </summary>
        /// <param name="req"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MailHunterSearchListResponse> GetProjectMailCountList(MailHunterSearchListRequest req, CancellationToken cancellationToken = default);

    }
}
