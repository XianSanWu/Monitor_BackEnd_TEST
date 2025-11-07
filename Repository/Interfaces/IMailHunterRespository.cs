using static Models.Entities.Requests.MailHunterEntityRequest;
using static Models.Entities.Responses.ProjectMailCountEntityResponse;

namespace Repository.Interfaces
{
    public interface IMailHunterRespository : IRepository
    {
        /// <summary>
        /// 查詢專案發送數量
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ProjectMailCountEntitySearchListResponse> GetProjectMailCountList(MailHunterEntitySearchListRequest req, CancellationToken cancellationToken = default);

    }
}

