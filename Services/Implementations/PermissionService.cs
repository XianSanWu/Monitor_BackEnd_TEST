using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Dto.Responses;
using Models.Enums;
using Repository.Implementations;
using Repository.Interfaces;
using Services.Interfaces;
using static Models.Dto.Requests.PermissionRequest;
using Repository.Implementations.PermissionRespository;
using static Models.Dto.Responses.PermissionResponse;
using static Models.Dto.Responses.UserResponse;
using static Models.Dto.Requests.UserRequest;
using Repository.UnitOfWorkExtension;
using AutoMapper.Features;
using System;

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
        public async Task<bool> SaveFeaturePermissionsAsync(PermissionUpdateRequest updateReq, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = false;
            #endregion

            #region 流程
            var dbType = DBConnectionEnum.Cdp;
#if TEST
            dbType = DBConnectionEnum.DefaultConnection;
#endif

            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);

            var repo = _repositoryFactory.Create<IPermissionRespository>(_scopeAccessor);

            return await repo.SaveFeaturePermissionsAsync(updateReq, cancellationToken);
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
            var dbType = DBConnectionEnum.Cdp;
#if TEST
            dbType = DBConnectionEnum.DefaultConnection;
#endif

            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);

            var repo = _repositoryFactory.Create<IPermissionRespository>(_scopeAccessor);

            return await repo.GetUserListAsync(searchReq, cancellationToken);
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
#if TEST
            dbType = DBConnectionEnum.DefaultConnection;
#endif

            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);

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
        public async Task<List<PermissionSearchListResponse>> GetUserPermissionsAsync(UserSearchListRequest searchReq, CancellationToken cancellationToken)
        {
            #region 參數宣告
            var result = new List<PermissionSearchListResponse>();
            #endregion

            #region 流程
            var dbType = DBConnectionEnum.Cdp;
#if TEST
            dbType = DBConnectionEnum.DefaultConnection;
#endif

            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);

            var repo = _repositoryFactory.Create<IPermissionRespository>(_scopeAccessor);

            result = await repo.GetUserPermissionsAsync(searchReq, cancellationToken).ConfigureAwait(false);

            return result;
            #endregion
        }

        /// <summary>
        /// 讓前端登入後依據 JWT 中的 UserId 拉取完整權限清單（module, feature, action）Menu
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<PermissionSearchListResponse>> GetUserPermissionsMenuAsync(UserSearchListRequest searchReq, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = new List<PermissionSearchListResponse>();
            #endregion


            #region 流程
            var dbType = DBConnectionEnum.Cdp;
#if TEST
            dbType = DBConnectionEnum.DefaultConnection;
#endif

            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);

            var repo = _repositoryFactory.Create<IPermissionRespository>(_scopeAccessor);

            result = await repo.GetUserPermissionsMenuAsync(searchReq, cancellationToken).ConfigureAwait(false);

            return result;
            #endregion
        }

        /// <summary>
        /// 取得所有權限清單列表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<PermissionSearchListResponse>> GetPermissionListAsync(PermissionSearchListRequest searchReq, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = new List<PermissionSearchListResponse>();
            #endregion

            #region 流程
            var dbType = DBConnectionEnum.Cdp;
#if TEST
            dbType = DBConnectionEnum.DefaultConnection;
#endif

            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);

            var repo = _repositoryFactory.Create<IPermissionRespository>(_scopeAccessor);

            result = await repo.GetPermissionListAsync(searchReq, cancellationToken).ConfigureAwait(false);

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
#if TEST
            dbType = DBConnectionEnum.DefaultConnection;
#endif

            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);

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
#if TEST
            dbType = DBConnectionEnum.DefaultConnection;
#endif

            using var uow = _uowFactory.UseUnitOfWork(_scopeAccessor, dbType);

            var repo = _repositoryFactory.Create<IPermissionRespository>(_scopeAccessor);

            result = await repo.GetUserByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);

            return result;
            #endregion
        }

    }
}
