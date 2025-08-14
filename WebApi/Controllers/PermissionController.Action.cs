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
        /// �ҥ�/���ΨϥΪ�
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission")]  //����(�i�h����)        
        [HttpPost("IsUseUserAsync")]
        public async Task<ResultResponse<bool>> IsUseUserAsync(UserUpdateRequest updateReq, CancellationToken cancellationToken)
        {

            #region �Ѽƫŧi
            var result = false;
            #endregion

            #region �y�{
            result = await _permissionService.IsUseUserAsync(updateReq, cancellationToken);

            return SuccessResult(result);
            #endregion
        }

    }
}