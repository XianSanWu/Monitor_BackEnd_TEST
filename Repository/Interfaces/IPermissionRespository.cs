using Models.Dto.Responses;
using static Models.Dto.Responses.PermissionResponse;
using static Models.Dto.Responses.UserResponse;
using static Models.Entities.Requests.PermissionEntityRequest;
using static Models.Entities.Requests.UserEntityRequest;

namespace Repository.Interfaces
{
    public interface IPermissionRespository : IRepository
    {
        /// <summary>
        /// 儲存全部權限
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SaveFeaturePermissionsAsync(PermissionUpdateEntityRequest updateReq, CancellationToken cancellationToken);

        /// <summary>
        /// 檢查需更新使用者是否存在
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        Task<bool> CheckUpdateUserAsync(UserUpdateEntityRequest updateReq, CancellationToken cancellationToken);

        /// <summary>
        /// 儲存使用者
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        Task<bool> SaveUserAsync(UserUpdateEntityRequest updateReq, CancellationToken cancellationToken);

        /// <summary>
        /// 啟用/停用使用者
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        Task<bool> IsUseUserAsync(UserUpdateEntityRequest updateReq, CancellationToken cancellationToken);

        /// <summary>
        /// 取得所有使用者清單列表
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
       Task<UserResponse> GetUserListAsync(UserSearchListEntityRequest searchReq, CancellationToken cancellationToken);


        // 權限資料存取介面定義

        /// <summary>
        /// 根據模組、功能與動作查詢 BitValue。
        /// </summary>
        Task<int?> GetBitValue(string module, string feature, string action, CancellationToken cancellationToken = default);

        /// <summary>
        /// 讓前端登入後依據 JWT 中的 UserId 拉取完整權限清單（module, feature, action）
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<PermissionSearchListResponse>> GetUserPermissionsAsync(UserSearchListEntityRequest searchReq, CancellationToken cancellationToken);

        /// <summary>
        /// 讓前端登入後依據 JWT 中的 UserId 拉取完整權限清單（module, feature, action）Memu
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<PermissionSearchListResponse>> GetUserPermissionsMenuAsync(UserSearchListEntityRequest searchReq, CancellationToken cancellationToken);
        
        /// <summary>
        /// 取得所有權限清單列表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<PermissionSearchListResponse>> GetPermissionListAsync(PermissionSearchListEntityRequest searchReq, CancellationToken cancellationToken);

        /// <summary>
        /// 取得單一使用者主檔資訊
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserSearchListResponse> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken);

        /// <summary>
        /// 取得單一使用者主檔資訊
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserSearchListResponse> GetUserByUserIdAsync(string userId, CancellationToken cancellationToken);
        
    }
}
