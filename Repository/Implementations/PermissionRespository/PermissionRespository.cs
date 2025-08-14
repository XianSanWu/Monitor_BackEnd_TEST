using AutoMapper;
using Dapper;
using Models.Dto.Responses;
using Models.Entities;
using Repository.Interfaces;
using static Models.Dto.Requests.UserRequest;
using static Models.Dto.Responses.PermissionResponse;
using static Models.Dto.Responses.UserResponse;

namespace Repository.Implementations.PermissionRespository
{
    public partial class PermissionRespository(IUnitOfWork unitOfWork, IMapper mapper)
        : BaseRepository(unitOfWork, mapper), IPermissionRespository
    {
        /// <summary>
        /// 啟用/停用使用者
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        public async Task<bool> IsUseUserAsync(UserUpdateRequest updateReq, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = false;

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            IsUseUser(updateReq);

            // 使用 ExecuteAsync 來執行 SQL 更新，並傳入 cancellationToken
            var affectedRows = await _unitOfWork.Connection.ExecuteAsync(_sqlStr?.ToString() ?? string.Empty, _sqlParams).ConfigureAwait(false);

            // 判斷是否有資料被更新
            result = affectedRows > 0;

            return result;
            #endregion
        }

        /// <summary>
        /// 取得所有使用者清單列表
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        public async Task<UserResponse> GetUserListAsync(UserSearchListRequest searchReq, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = new UserResponse();

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            GetUserList(searchReq);

            result.Page = searchReq.Page;
            result.SearchItem = new List<UserSearchListResponse>();

            var _pagingSql = await GetPagingSql(searchReq.Page, _unitOfWork, _sqlParams).ConfigureAwait(false);
            var queryEntity = (await _unitOfWork.Connection.QueryAsync<UserEntity>(_pagingSql, _sqlParams).ConfigureAwait(false)).ToList();
            result.SearchItem = _mapper.Map<List<UserSearchListResponse>>(queryEntity);
            result.Page.TotalCount = (await _unitOfWork.Connection.QueryAsync<int?>(GetTotalCountSql(), _sqlParams).ConfigureAwait(false)).FirstOrDefault() ?? 0;
            return result;
            #endregion
        }

        /// <summary>
        /// 根據模組、功能與動作查詢 BitValue。
        /// </summary>
        public async Task<int?> GetBitValue(string module, string feature, string action, CancellationToken cancellationToken = default)
        {
            #region 參數宣告

            var result = (int?)null;

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            GetBitValue(module, feature, action);

            result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<int?>((_sqlStr?.ToString() ?? ""), _sqlParams).ConfigureAwait(false);

            return result;
            #endregion
        }

        /// <summary>
        /// 讓前端登入後依據 JWT 中的 UserId 拉取完整權限清單（module, feature, action）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PermissionSearchListResponse> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = new PermissionSearchListResponse();

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            GetPermissions(userId);

            var queryEntity = await _unitOfWork.Connection.QueryAsync<FeaturePermissionEntity>((_sqlStr?.ToString() ?? ""), _sqlParams).ConfigureAwait(false);
            result = _mapper.Map<PermissionSearchListResponse>(queryEntity);

            return result;
            #endregion
        }

        /// <summary>
        /// 取得所有權限清單列表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<PermissionSearchListResponse>> GetPermissionListAsync(CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = new List<PermissionSearchListResponse>();

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            GetPermissions();

            var queryEntity = await _unitOfWork.Connection.QueryAsync<FeaturePermissionEntity>((_sqlStr?.ToString() ?? ""), _sqlParams).ConfigureAwait(false);
            result = _mapper.Map<List<PermissionSearchListResponse>>(queryEntity);

            return result;
            #endregion
        }

        /// <summary>
        /// 取得單一使用者主檔資訊
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UserSearchListResponse> GetUserAsync(string userName, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = new UserSearchListResponse();

            #endregion

            #region 流程

            // 在執行前檢查是否有取消的需求
            cancellationToken.ThrowIfCancellationRequested();

            // 先組合 SQL 語句
            GetUser(userName);

            var queryEntity = await _unitOfWork.Connection.QueryAsync<UserEntity>((_sqlStr?.ToString() ?? ""), _sqlParams).ConfigureAwait(false);
            var mapper = _mapper.Map<List<UserSearchListResponse>>(queryEntity);
            result = mapper.FirstOrDefault() ?? new();

            return result;
            #endregion
        }
    }
}
