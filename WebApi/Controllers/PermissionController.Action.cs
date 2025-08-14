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
        /// 啟用/停用使用者
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission")]  //分組(可多標籤)        
        [HttpPost("IsUseUserAsync")]
        public async Task<ResultResponse<bool>> IsUseUserAsync(UserUpdateRequest updateReq, CancellationToken cancellationToken)
        {

            #region 參數宣告
            var result = false;
            #endregion

            #region 流程
            result = await _permissionService.IsUseUserAsync(updateReq, cancellationToken);

            return SuccessResult(result);
            #endregion
        }

    }
}