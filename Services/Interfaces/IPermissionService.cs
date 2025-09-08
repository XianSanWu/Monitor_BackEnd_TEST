using Models.Dto.Responses;
using static Models.Dto.Requests.PermissionRequest;
using static Models.Dto.Requests.UserRequest;
using static Models.Dto.Responses.PermissionResponse;
using static Models.Dto.Responses.UserResponse;

namespace Services.Interfaces
{
    public interface IPermissionService
    {
        /// <summary>
        /// 儲存全部權限
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SaveFeaturePermissionsAsync(PermissionUpdateRequest updateReq, CancellationToken cancellationToken);

        /// <summary>
        /// 檢查需更新使用者是否存在
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        Task<bool> CheckUpdateUserAsync(UserUpdateRequest updateReq, CancellationToken cancellationToken);

        /// <summary>
        /// 儲存使用者
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        Task<bool> SaveUserAsync(UserUpdateRequest updateReq, CancellationToken cancellationToken);

        /// <summary>
        /// 啟用/停用使用者
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        Task<bool> IsUseUserAsync(UserUpdateRequest updateReq, CancellationToken cancellationToken);

        /// <summary>
        /// 取得所有使用者清單列表
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        Task<UserResponse> GetUserListAsync(UserSearchListRequest searchReq, CancellationToken cancellationToken);



        // 權限服務介面定義

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
        Task<List<PermissionSearchListResponse>> GetUserPermissionsAsync(UserSearchListRequest searchReq, CancellationToken cancellationToken);


        /// <summary>
        /// 讓前端登入後依據 JWT 中的 UserId 拉取完整權限清單（module, feature, action） Menu
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<PermissionSearchListResponse>> GetUserPermissionsMenuAsync(UserSearchListRequest searchReq, CancellationToken cancellationToken);
        

        /// <summary>
        /// 取得所有權限清單列表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<PermissionSearchListResponse>> GetPermissionListAsync(PermissionSearchListRequest searchReq, CancellationToken cancellationToken);

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
