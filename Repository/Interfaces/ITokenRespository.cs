
namespace Repository.Interfaces
{
    public interface ITokenRespository
    {
        /// <summary>
        /// 儲存User Token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="expiresAt"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> InsertUserTokenAsync(string userId, string token,DateTime expiresAt, CancellationToken cancellationToken);
    }
}
