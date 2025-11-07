using static Models.Dto.Requests.MsmqQueueRequest;
using static Models.Dto.Responses.MsmqQueueResponse;

namespace Services.Interfaces
{
    public interface IMsmqService
    {
        /// <summary>
        /// 取得全部MSMQ佇列訊息
        /// </summary>
        /// <param name="req"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MsmqQueueDetailsResponse> GetAllQueueInfo(MsmqQueueInfoRequest req, CancellationToken cancellationToken = default);

    }
}
