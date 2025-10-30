using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Dto.Responses;
using Models.Enums;
using Repository.Interfaces;
using Repository.UnitOfWorkExtension;
using Services.Interfaces;
using static Models.Dto.Requests.PermissionRequest;
using static Models.Dto.Requests.UserRequest;
using static Models.Dto.Responses.PermissionResponse;
using static Models.Dto.Responses.UserResponse;
using static Models.Entities.Requests.MailHunterEntityRequest;
using static Models.Entities.Requests.PermissionEntityRequest;
using static Models.Entities.Requests.UserEntityRequest;
using static Models.Entities.Requests.WorkflowStepsEntityRequest;

namespace Services.Implementations
{
    public class PermissionService(
        IPermissionRespository permissionRespository,
        ILogger<PermissionService> logger,
        IConfiguration config,
        IMapper mapper,
        IUnitOfWorkFactory uowFactory,
        IRepositoryFactory repositoryFactory,
        IUnitOfWorkScopeAccessor scopeAccessor
            ) : IPermissionService
    {
        private readonly ILogger<PermissionService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IConfiguration _config = config;
        private readonly IPermissionRespository _permissionRespository = permissionRespository;
        private readonly IUnitOfWorkFactory _uowFactory = uowFactory;
        private readonly IRepositoryFactory _repositoryFactory = repositoryFactory;
        private readonly IUnitOfWorkScopeAccessor _scopeAccessor = scopeAccessor;

        /// <summary>
        /// 儲存全部權限
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SaveFeaturePermissionsAsync(PermissionUpdateRequest req, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = false;
            #endregion

            var entityReq = mapper.Map<PermissionUpdateEntityRequest>(req);

            #region 流程
            var dbType = DBConnectionEnum.Cdp;
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);
            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IPermissionRespository>(_scopeAccessor);
            result = await repo.SaveFeaturePermissionsAsync(entityReq, cancellationToken).ConfigureAwait(false);

            return result;
            #endregion
        }

        /// <summary>
        /// 檢查需更新使用者是否存在
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        public async Task<bool> CheckUpdateUserAsync(UserUpdateRequest req, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = false;
            #endregion

            var entityReq = mapper.Map<UserUpdateEntityRequest>(req);

            #region 流程
            var dbType = DBConnectionEnum.Cdp;
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);
            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IPermissionRespository>(_scopeAccessor);
            result = await repo.CheckUpdateUserAsync(entityReq, cancellationToken).ConfigureAwait(false);
            
            return result;
            #endregion
        }

        /// <summary>
        /// 儲存使用者
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        public async Task<bool> SaveUserAsync(UserUpdateRequest req, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = false;
            #endregion

            if (req.FieldRequest != null)
            {
                req.FieldRequest.UpdateAt = DateTime.Now;
            }

            var entityReq = mapper.Map<UserUpdateEntityRequest>(req);

            #region 流程
            var dbType = DBConnectionEnum.Cdp;
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);
            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IPermissionRespository>(_scopeAccessor);
            result = await repo.SaveUserAsync(entityReq, cancellationToken).ConfigureAwait(false);

            return result;
            #endregion
        }

        /// <summary>
        /// 啟用/停用使用者
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        public async Task<bool> IsUseUserAsync(UserUpdateRequest req, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = false;
            #endregion

            if (req.FieldRequest != null)
            {
                req.FieldRequest.UpdateAt = DateTime.Now;
            }

            var entityReq = mapper.Map<UserUpdateEntityRequest>(req);

            #region 流程
            var dbType = DBConnectionEnum.Cdp;
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);
            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IPermissionRespository>(_scopeAccessor);
            result = await repo.IsUseUserAsync(entityReq, cancellationToken).ConfigureAwait(false);

            return result;
            #endregion
        }

        /// <summary>
        /// 取得所有使用者清單列表
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        public async Task<UserResponse> GetUserListAsync(UserSearchListRequest req, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = new UserResponse();
            #endregion

            var entityReq = mapper.Map<UserSearchListEntityRequest>(req);

            #region 流程
            var dbType = DBConnectionEnum.Cdp;
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);
            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IPermissionRespository>(_scopeAccessor);
            result = await repo.GetUserListAsync(entityReq, cancellationToken).ConfigureAwait(false);

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
            var dbType = DBConnectionEnum.Cdp;
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);
            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IPermissionRespository>(_scopeAccessor);
            result = await repo.GetBitValue(module, feature, action, cancellationToken).ConfigureAwait(false);

            return result;
            #endregion
        }

        /// <summary>
        /// 讓前端登入後依據 JWT 中的 UserId 拉取完整權限清單（module, feature, action）
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<PermissionSearchListResponse>> GetUserPermissionsAsync(UserSearchListRequest req, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = new List<PermissionSearchListResponse>();
            #endregion

            var entityReq = mapper.Map<UserSearchListEntityRequest>(req);

            #region 流程
            var dbType = DBConnectionEnum.Cdp;
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);
            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IPermissionRespository>(_scopeAccessor);
            result = await repo.GetUserPermissionsAsync(entityReq, cancellationToken).ConfigureAwait(false);

            return result;
            #endregion
        }

        /// <summary>
        /// 讓前端登入後依據 JWT 中的 UserId 拉取完整權限清單（module, feature, action）Menu
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<PermissionSearchListResponse>> GetUserPermissionsMenuAsync(UserSearchListRequest req, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = new List<PermissionSearchListResponse>();
            #endregion

            var entityReq = mapper.Map<UserSearchListEntityRequest>(req);

            #region 流程
            var dbType = DBConnectionEnum.Cdp;
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);
            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IPermissionRespository>(_scopeAccessor);
            result = await repo.GetUserPermissionsMenuAsync(entityReq, cancellationToken).ConfigureAwait(false);

            return result;
            #endregion
        }

        /// <summary>
        /// 取得所有權限清單列表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<PermissionSearchListResponse>> GetPermissionListAsync(PermissionSearchListRequest req, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = new List<PermissionSearchListResponse>();
            #endregion

            var entityReq = _mapper.Map<PermissionSearchListEntityRequest>(req);

            #region 流程
            var dbType = DBConnectionEnum.Cdp;
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);
            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IPermissionRespository>(_scopeAccessor);
            result = await repo.GetPermissionListAsync(entityReq, cancellationToken).ConfigureAwait(false);

            return result;
            #endregion
        }

        /// <summary>
        /// 取得單一使用者主檔資訊
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UserSearchListResponse> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = new UserSearchListResponse();
            #endregion

            #region 流程
            var dbType = DBConnectionEnum.Cdp;
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);
            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IPermissionRespository>(_scopeAccessor);
            result = await repo.GetUserByUserNameAsync(userName, cancellationToken).ConfigureAwait(false);

            return result;
            #endregion
        }

        /// <summary>
        /// 取得單一使用者主檔資訊
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UserSearchListResponse> GetUserByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = new UserSearchListResponse();
            #endregion

            #region 流程
            var dbType = DBConnectionEnum.Cdp;
            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);
            // 改成通用 Factory 呼叫
            var repo = _repositoryFactory.Create<IPermissionRespository>(_scopeAccessor);
            result = await repo.GetUserByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);

            return result;
            #endregion
        }

    }
}
