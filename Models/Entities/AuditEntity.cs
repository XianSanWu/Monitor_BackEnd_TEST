namespace Models.Entities
{
    /// <summary>
    /// 稽核紀錄表 (AuditLogs)
    /// 用來紀錄 API 的操作軌跡，包含使用者、請求方法、路徑、參數、來源 IP 等資訊。
    /// </summary>
    public class AuditEntity
    {
        /// <summary>
        /// 主鍵，自動遞增
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 使用者 ID（來源於 JWT 或登入帳號）
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 使用者 帳號
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 前端完整 URL，供稽核使用
        /// </summary>
        public string FrontUrl { get; set; } = string.Empty;

        /// <summary>
         ///  同批次操作識別碼
         /// </summary>
        public string FrontActionId { get; set; } = string.Empty;

        /// <summary>
        /// 前端按鈕，供稽核使用
        /// </summary>
        public string FrontActionName { get; set; } = string.Empty;

        /// <summary>
        /// 動作名稱，例如 Controller/Action
        /// </summary>
        public string BackActionName { get; set; } = string.Empty;

        /// <summary>
        /// HTTP 方法 (GET/POST/PUT/DELETE)
        /// </summary>
        public string HttpMethod { get; set; } = string.Empty;

        /// <summary>
        /// HTTP 回應
        /// </summary>
        public string HttpStatusCode { get; set; } = string.Empty;

        /// <summary>
        /// API 請求路徑，例如 /api/orders/123
        /// </summary>
        public string RequestPath { get; set; } = string.Empty;

        /// <summary>
        /// JSON 格式的傳入參數（完整序列化）
        /// </summary>
        public string Parameters { get; set; } = string.Empty;

        /// <summary>
        /// JSON 格式的傳出
        /// </summary>
        public string ResponseBody { get; set; } = string.Empty;

        /// <summary>
        /// 呼叫來源 IP 位址
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// 建立時間（預設為伺服器時間）
        /// </summary>
        public DateTime CreateAt { get; set; }
    }
}
