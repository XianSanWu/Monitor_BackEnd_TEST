using Microsoft.AspNetCore.Mvc;
using Models.Dto.Responses;
using WebAPi.Controllers;
using static Models.Dto.Requests.PermissionRequest;
using static Models.Dto.Requests.UserRequest;
using static Models.Dto.Responses.PermissionResponse;

namespace WebApi.Controllers
{
    public partial class PermissionController : BaseController
    {
        /// <summary>
        /// 啟用/停用使用者
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission.Action")]  //分組(可多標籤)        
        [HttpPost("IsUseUserAsync")]
        public async Task<ResultResponse<bool>> IsUseUserAsync(UserUpdateRequest updateReq, CancellationToken cancellationToken)
        {

            #region 參數宣告
            var result = false;
            #endregion

            #region 流程
            result = await _permissionService.IsUseUserAsync(updateReq, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);
            #endregion
        }

        /// <summary>
        /// 儲存使用者
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission.Action")]  //分組(可多標籤)        
        [HttpPost("SaveUserAsync")]
        public async Task<ResultResponse<bool>> SaveUserAsync(UserUpdateRequest updateReq, CancellationToken cancellationToken)
        {

            #region 參數宣告
            var result = false;
            #endregion

            #region 流程

            result = await _permissionService.SaveUserAsync(updateReq, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);
            #endregion
        }

        /// <summary>
        /// 檢查需更新使用者是否存在
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission.Action")]  //分組(可多標籤)        
        [HttpPost("CheckUpdateUserAsync")]
        public async Task<ResultResponse<bool>> CheckUpdateUserAsync(UserUpdateRequest updateReq, CancellationToken cancellationToken)
        {

            #region 參數宣告
            var result = false;
            #endregion

            #region 流程

            var checkUser = await _permissionService.CheckUpdateUserAsync(updateReq, cancellationToken).ConfigureAwait(false);

            if (checkUser)
            {
                return FailResult<bool>("已經有此帳號，不可進行新增，請使用編輯作業");
            }
            result = !checkUser;
            return SuccessResult(result);
            #endregion
        }


        /// <summary>
        /// 儲存全部權限
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission.Action")]  //分組(可多標籤)        
        [HttpPost("SaveFeaturePermissionsAsync")]
        public async Task<ResultResponse<bool>> SaveFeaturePermissionsAsync(PermissionUpdateRequest updateReq, CancellationToken cancellationToken)
        {

            #region 參數宣告
            var result = false;
            #endregion

            #region 流程

            result = await _permissionService.SaveFeaturePermissionsAsync(updateReq, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);
            #endregion
        }

    }
}