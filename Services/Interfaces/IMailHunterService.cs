using Microsoft.Extensions.Configuration;
using static Models.Dto.Requests.MailHunterRequest;
using static Models.Dto.Responses.MailHunterResponse;

namespace Services.Interfaces
{
    public interface IMailHunterService
    {

        /// <summary>
        /// 查詢專案發送數量
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MailHunterSearchListResponse> GetProjectMailCountList(MailHunterSearchListRequest searchReq, IConfiguration _config, CancellationToken cancellationToken = default);

    }
}
