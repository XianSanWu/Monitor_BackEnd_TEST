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
using AutoMapper.Features;
using System;
using static Models.Dto.Responses.PermissionResponse;
using k8s.KubeConfigModels;
using static Models.Dto.Responses.UserResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Models.Dto.Requests.UserRequest;

namespace Services.Implementations
{
    public class PermissionService(
        IPermissionRespository permissionRespository,
        ILogger<PermissionService> logger,
        IConfiguration config,
        IMapper mapper
            ) : IPermissionService
    {
        private readonly ILogger<PermissionService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IConfiguration _config = config;
        private readonly IPermissionRespository _permissionRespository = permissionRespository;

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
            var CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.Cdp);
#if TEST
            CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.DefaultConnection);
#endif
            using (IDbHelper dbHelper = CDP_dbHelper)
            {
                IPermissionRespository _pRp = new PermissionRespository(dbHelper.UnitOfWork, mapper);
                result = await _pRp.SaveFeaturePermissionsAsync(updateReq, cancellationToken).ConfigureAwait(false);
            }

            return result;
            #endregion
        }

        /// <summary>
        /// 檢查需更新使用者是否存在
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        public async Task<bool> CheckUpdateUserAsync(UserUpdateRequest updateReq, CancellationToken cancellationToken)
        {
            #region 參數宣告

            var result = false;
            #endregion

            #region 流程
            var CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.Cdp);
#if TEST
            CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.DefaultConnection);
#endif
            using (IDbHelper dbHelper = CDP_dbHelper)
            {
                IPermissionRespository _pRp = new PermissionRespository(dbHelper.UnitOfWork, mapper);
                result = await _pRp.CheckUpdateUserAsync(updateReq, cancellationToken).ConfigureAwait(false);
            }

            return result;
            #endregion
        }

        /// <summary>
        /// 儲存使用者
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        public async Task<bool> SaveUserAsync(UserUpdateRequest updateReq, CancellationToken cancellationToken)
        {
            #region 參數宣告
            
            var result = false;
            #endregion

            if (updateReq.FieldRequest != null)
            {
                updateReq.FieldRequest.UpdateAt = DateTime.Now;
            }

            #region 流程
            var CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.Cdp);
#if TEST
            CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.DefaultConnection);
#endif
            using (IDbHelper dbHelper = CDP_dbHelper)
            {
                IPermissionRespository _pRp = new PermissionRespository(dbHelper.UnitOfWork, mapper);
                result = await _pRp.SaveUserAsync(updateReq, cancellationToken).ConfigureAwait(false);
            }

            return result;
            #endregion
        }

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

            if (updateReq.FieldRequest != null)
            {
                updateReq.FieldRequest.UpdateAt = DateTime.Now;
            }

            #region 流程
            var CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.Cdp);
#if TEST
            CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.DefaultConnection);
#endif
            using (IDbHelper dbHelper = CDP_dbHelper)
            {
                IPermissionRespository _pRp = new PermissionRespository(dbHelper.UnitOfWork, mapper);
                result = await _pRp.IsUseUserAsync(updateReq, cancellationToken).ConfigureAwait(false);
            }

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
            var CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.Cdp);
#if TEST
            CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.DefaultConnection);
#endif
            using (IDbHelper dbHelper = CDP_dbHelper)
            {
                IPermissionRespository _pRp = new PermissionRespository(dbHelper.UnitOfWork, mapper);
                result = await _pRp.GetUserListAsync(searchReq, cancellationToken).ConfigureAwait(false);
            }

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
            var CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.Cdp);
#if TEST
            CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.DefaultConnection);
#endif
            using (IDbHelper dbHelper = CDP_dbHelper)
            {
                IPermissionRespository _pRp = new PermissionRespository(dbHelper.UnitOfWork, mapper);
                result = await _pRp.GetBitValue(module, feature, action, cancellationToken).ConfigureAwait(false);
            }

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
            var CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.Cdp);
#if TEST
            CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.DefaultConnection);
#endif
            using (IDbHelper dbHelper = CDP_dbHelper)
            {
                IPermissionRespository _pRp = new PermissionRespository(dbHelper.UnitOfWork, mapper);
                result = await _pRp.GetUserPermissionsAsync(searchReq, cancellationToken).ConfigureAwait(false);
            }
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
            var CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.Cdp);
#if TEST
            CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.DefaultConnection);
#endif
            using (IDbHelper dbHelper = CDP_dbHelper)
            {
                IPermissionRespository _pRp = new PermissionRespository(dbHelper.UnitOfWork, mapper);
                result = await _pRp.GetPermissionListAsync(searchReq, cancellationToken).ConfigureAwait(false);
            }
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
            var CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.Cdp);
#if TEST
            CDP_dbHelper = new DbHelper(_config, DBConnectionEnum.DefaultConnection);
#endif
            using (IDbHelper dbHelper = CDP_dbHelper)
            {
                IPermissionRespository _pRp = new PermissionRespository(dbHelper.UnitOfWork, mapper);
                result = await _pRp.GetUserAsync(userName, cancellationToken).ConfigureAwait(false);
            }
            return result;
            #endregion
        }



    }
}
