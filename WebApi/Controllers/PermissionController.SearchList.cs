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
        /// ���o�Ҧ��ϥΪ̲M��C��
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission")]  //����(�i�h����)        
        [HttpPost("GetUserListAsync")]
        public async Task<ResultResponse<UserResponse>> GetUserListAsync(UserSearchListRequest searchReq, CancellationToken cancellationToken)
        {

            #region �Ѽƫŧi
            var result = new UserResponse();
            #endregion

            #region �y�{
            result = await _permissionService.GetUserListAsync(searchReq, cancellationToken);

            return SuccessResult(result);
            #endregion
        }


        /// <summary>
        /// ���o�Ҧ��v���M��C��
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission")]  //����(�i�h����)        
        [HttpPost("GetPermissionListAsync")]
        public async Task<ResultResponse<List<PermissionSearchListResponse>>> GetPermissionListAsync(CancellationToken cancellationToken)
        {

            #region �Ѽƫŧi
            var result = new List<PermissionSearchListResponse>();
            #endregion

            #region �y�{
            result = await _permissionService.GetPermissionListAsync(cancellationToken);

            return SuccessResult(result);
            #endregion
        }

        /// <summary>
        /// ���o�ӤH�Ҧ��v���M��
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission")]  //����(�i�h����)        
        [HttpPost("GetUserPermissionsAsync")]
        public async Task<ResultResponse<PermissionSearchListResponse>> GetUserPermissionsAsync(CancellationToken cancellationToken)
        {

            #region �Ѽƫŧi
            var result = new PermissionSearchListResponse();
            #endregion

            #region �y�{

            var userId = User.FindFirst("UserId")?.Value?.ToString() ?? string.Empty;

            result = await _permissionService.GetUserPermissionsAsync(userId, cancellationToken);

            return SuccessResult(result);
            #endregion
        }

    }
}