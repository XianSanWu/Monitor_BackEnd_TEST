

namespace Services.Interfaces
{
    /// <summary>
    /// JWT Token。
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Token 產生服務，用於產生 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="featureMask"></param>
        /// <returns></returns>
        Task<string> GenerateTokenAsync(string userId, int featureMask);

        /// <summary>
        /// 儲存User Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> InsertUserTokenAsync(string userId, string token, CancellationToken cancellationToken);
    }
}
