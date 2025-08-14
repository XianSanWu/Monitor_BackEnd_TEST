﻿
namespace Models.Entities
{
    /// <summary>
    /// 使用者資料表
    /// </summary>
    public class UserEntity
    {
        /// <summary>
        /// 使用者主鍵
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 使用者主鍵 Uuid
        /// </summary>
        public string Uuid { get; set; } = string.Empty;

        /// <summary>
        /// 使用者名稱（唯一）
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 使用者權限 BitMask（加速用）
        /// </summary>
        public int FeatureMask { get; set; }

        /// <summary>
        /// 使用者 是否刪除
        /// </summary>
        public bool IsUse { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateAt { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime UpdateAt { get; set; }
    }

}
