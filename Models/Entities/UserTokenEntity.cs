
namespace Models.Entities
{
    /// <summary>
    /// 使用者 Token 紀錄表（支援多裝置登入 / JWT 驗證）
    /// </summary>
    public class UserTokenEntity
    {
        /// <summary>
        /// 主鍵
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 對應的使用者 Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// JWT 唯一識別碼（jti）
        /// </summary>
        public string JwtId { get; set; } = string.Empty;

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 過期時間
        /// </summary>
        public DateTime ExpiredAt { get; set; }
    }

}
