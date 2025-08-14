using Microsoft.AspNetCore.Mvc;
using WebAPi.Controllers;
using Models.Dto.Responses;
using static Models.Dto.Requests.AuthRequest;

namespace WebApi.Controllers
{
    public partial class AuthController : BaseController
    {
        /// <summary>
        /// �n�J
        /// </summary>
        /// <param name="loginReq">return bool</param>
        /// <param name="cancellationToken">�����D�P�B</param>   
        /// <returns></returns>
        [Tags("Auth")]  //����(�i�h����)        
        [HttpPost("Login")]
        public async Task<ResultResponse<AuthResponse>> Login(LoginRequest loginReq, CancellationToken cancellationToken)
        {
            #region �Ѽƫŧi
            var result = new AuthResponse();
            //ValidationResult loginValidationResult;
            #endregion

            #region Mock
#if TEST
            //loginReq = new LoginRequest
            //{
            //    UserName = "admin",
            //    Password = "admin_it123",
            //};
#endif
            #endregion

            #region ����
            //loginValidationResult = await _loginRequestValidator.ValidateAsync(loginReq, cancellationToken).ConfigureAwait(false);
            //_logger.LogInformation("LoginRequest �ѼơG{@LoginRequest}", loginReq);
            //_logger.LogInformation("LoginRequest ���ҡG{ValidationResult}", loginValidationResult);

            //if (!string.IsNullOrWhiteSpace(loginValidationResult.ToString()))
            //{
            //    return FailResult<bool>($"�Ѽ��ˮ֥��q�L�G{loginValidationResult}");
            //}
            #endregion

            #region �y�{
            result = await _authService.Login(loginReq, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);
            #endregion
        }

    }
}