using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;
using WebAPi.Controllers;
using Models.Dto.Responses;
using Models.Dto.Requests;
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
        public async Task<ResultResponse<bool>> Login(LoginRequest loginReq, CancellationToken cancellationToken)
        {
            #region �Ѽƫŧi
            bool result = false;
            ValidationResult loginValidationResult;
            #endregion

            #region Mock
#if DEBUG
            //loginReq = new LoginRequest
            //{
            //    UserName = "admin",
            //    Password = "admin_it123",
            //};
#endif
            #endregion

            #region ����
            loginValidationResult = await _loginRequestValidator.ValidateAsync(loginReq, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("LoginRequest �ѼơG{@LoginRequest}", loginReq);
            _logger.LogInformation("LoginRequest ���ҡG{ValidationResult}", loginValidationResult);

            if (!string.IsNullOrWhiteSpace(loginValidationResult.ToString()))
            {
                return FailResult<bool>($"�Ѽ��ˮ֥��q�L�G{loginValidationResult}");
            }
            #endregion

            result = await _authService.Login(loginReq, _config, cancellationToken).ConfigureAwait(false);

            return result ? SuccessResult(result) : FailResult<bool>($"���ҥ��ѡA�п�J���T�b���K�X");
        }

    }
}