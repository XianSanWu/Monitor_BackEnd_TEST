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
        /// �ҥ�/���ΨϥΪ�
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission.Action")]  //����(�i�h����)        
        [HttpPost("IsUseUserAsync")]
        public async Task<ResultResponse<bool>> IsUseUserAsync(UserUpdateRequest updateReq, CancellationToken cancellationToken)
        {

            #region �Ѽƫŧi
            var result = false;
            #endregion

            #region �y�{
            result = await _permissionService.IsUseUserAsync(updateReq, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);
            #endregion
        }

        /// <summary>
        /// �x�s�ϥΪ�
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission.Action")]  //����(�i�h����)        
        [HttpPost("SaveUserAsync")]
        public async Task<ResultResponse<bool>> SaveUserAsync(UserUpdateRequest updateReq, CancellationToken cancellationToken)
        {

            #region �Ѽƫŧi
            var result = false;
            #endregion

            #region �y�{

            result = await _permissionService.SaveUserAsync(updateReq, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);
            #endregion
        }

        /// <summary>
        /// �ˬd�ݧ�s�ϥΪ̬O�_�s�b
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission.Action")]  //����(�i�h����)        
        [HttpPost("CheckUpdateUserAsync")]
        public async Task<ResultResponse<bool>> CheckUpdateUserAsync(UserUpdateRequest updateReq, CancellationToken cancellationToken)
        {

            #region �Ѽƫŧi
            var result = false;
            #endregion

            #region �y�{

            var checkUser = await _permissionService.CheckUpdateUserAsync(updateReq, cancellationToken).ConfigureAwait(false);

            if (checkUser)
            {
                return FailResult<bool>("�w�g�����b���A���i�i��s�W�A�Шϥνs��@�~");
            }
            result = !checkUser;
            return SuccessResult(result);
            #endregion
        }


        /// <summary>
        /// �x�s�����v��
        /// </summary>
        /// <param name="updateReq"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("Permission.Action")]  //����(�i�h����)        
        [HttpPost("SaveFeaturePermissionsAsync")]
        public async Task<ResultResponse<bool>> SaveFeaturePermissionsAsync(PermissionUpdateRequest updateReq, CancellationToken cancellationToken)
        {

            #region �Ѽƫŧi
            var result = false;
            #endregion

            #region �y�{

            result = await _permissionService.SaveFeaturePermissionsAsync(updateReq, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);
            #endregion
        }

    }
}