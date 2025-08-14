using Microsoft.AspNetCore.Mvc;
using Models.Dto.Responses;
using WebAPi.Controllers;
using static Models.Dto.Requests.UserRequest;
using static Models.Dto.Responses.PermissionResponse;

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
        [Tags("Permission")]  //分組(可多標籤)        
        [HttpPost("GetUserListAsync")]
        public async Task<ResultResponse<UserResponse>> GetUserListAsync(UserSearchListRequest searchReq, CancellationToken cancellationToken)
        {

            #region 參數宣告
            var result = new UserResponse();
            #endregion

            #region 流程
            result = await _permissionService.GetUserListAsync(searchReq, cancellationToken);

            return SuccessResult(result);
            #endregion
        }


        /// <summary>
        /// 取得所有權限清單列表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission")]  //分組(可多標籤)        
        [HttpPost("GetPermissionListAsync")]
        public async Task<ResultResponse<List<PermissionSearchListResponse>>> GetPermissionListAsync(CancellationToken cancellationToken)
        {

            #region 參數宣告
            var result = new List<PermissionSearchListResponse>();
            #endregion

            #region 流程
            result = await _permissionService.GetPermissionListAsync(cancellationToken);

            return SuccessResult(result);
            #endregion
        }

        /// <summary>
        /// 取得個人所有權限清單
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission")]  //分組(可多標籤)        
        [HttpPost("GetUserPermissionsAsync")]
        public async Task<ResultResponse<PermissionSearchListResponse>> GetUserPermissionsAsync(CancellationToken cancellationToken)
        {

            #region 參數宣告
            var result = new PermissionSearchListResponse();
            #endregion

            #region 流程

            var userId = User.FindFirst("UserId")?.Value?.ToString() ?? string.Empty;

            result = await _permissionService.GetUserPermissionsAsync(userId, cancellationToken);

            return SuccessResult(result);
            #endregion
        }

    }
}