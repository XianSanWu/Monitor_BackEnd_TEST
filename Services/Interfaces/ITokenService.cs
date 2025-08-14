
namespace Services.Interfaces
{
    /// <summary>
    /// Token 產生服務，用於產生 JWT Token。
    /// </summary>
    public interface ITokenService
    {
        string GenerateToken(string userId, int featureMask);
    }
}
