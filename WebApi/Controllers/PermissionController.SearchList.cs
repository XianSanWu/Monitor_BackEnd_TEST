using Microsoft.AspNetCore.Mvc;
using Models.Dto.Responses;
using WebAPi.Controllers;
using static Models.Dto.Requests.PermissionRequest;
using static Models.Dto.Requests.UserRequest;
using static Models.Dto.Responses.PermissionResponse.PermissionSearchListResponse;
using static Models.Dto.Responses.UserResponse;

namespace WebApi.Controllers
{
    public partial class PermissionController : BaseController
    {
        /// <summary>
        /// 取得所有使用者清單列表
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission.SearchList")]   //分組(可多標籤)        
        [HttpPost("GetUserListAsync")]
        public async Task<ResultResponse<UserSearchListResponse>> GetUserListAsync(UserSearchListRequest searchReq, CancellationToken cancellationToken)
        {

            #region 參數宣告
            var result = new UserSearchListResponse();
            #endregion

            #region 流程
            result = await _permissionService.GetUserListAsync(searchReq, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);
            #endregion
        }


        /// <summary>
        /// 取得所有權限清單列表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission.SearchList")]   //分組(可多標籤)        
        [HttpPost("GetPermissionListAsync")]
        public async Task<ResultResponse<List<PermissionSearchResponse>>> GetPermissionListAsync(PermissionSearchListRequest searchReq, CancellationToken cancellationToken)
        {

            #region 參數宣告
            var result = new List<PermissionSearchResponse>();
            #endregion

            #region 流程
            result = await _permissionService.GetPermissionListAsync(searchReq, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);
            #endregion
        }

        /// <summary>
        /// 取得個人所有權限清單
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission.SearchList")]  //分組(可多標籤)        
        [HttpPost("GetUserPermissionsAsync")]
        public async Task<ResultResponse<List<PermissionSearchResponse>>> GetUserPermissionsAsync(UserSearchListRequest searchReq, CancellationToken cancellationToken)
        {

            #region 參數宣告
            var result = new List<PermissionSearchResponse>();
            #endregion

            #region 流程

            result = await _permissionService.GetUserPermissionsAsync(searchReq, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);
            #endregion
        }

        /// <summary>
        /// 取得個人所有權限清單 Menu
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission.SearchList")]  //分組(可多標籤)        
        [HttpPost("GetUserPermissionsMenuAsync")]
        public async Task<ResultResponse<List<PermissionSearchResponse>>> GetUserPermissionsMenuAsync(UserSearchListRequest searchReq, CancellationToken cancellationToken)
        {

            #region 參數宣告
            var result = new List<PermissionSearchResponse>();
            #endregion

            #region 流程

            result = await _permissionService.GetUserPermissionsMenuAsync(searchReq, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);
            #endregion
        }

    }
}