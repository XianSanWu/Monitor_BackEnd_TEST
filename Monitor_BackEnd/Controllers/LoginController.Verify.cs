using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;
using WebAPi.Controllers;
using Models.Dto.Responses;
using Models.Dto.Requests;
using System.Diagnostics;

namespace WebApi.Controllers
{
    public partial class LoginController : BaseController
    {
        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <param name="loginReq">return bool</param>
        /// <param name="cancellationToken">取消非同步</param>   
        /// <returns></returns>
        [Tags("Login")]  //分組(可多標籤)        
        [HttpPost("Verify")]
        public async Task<ResultResponse<bool>> Verify(LoginRequest loginReq, CancellationToken cancellationToken)
        {
            #region 參數宣告
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

            #region 驗證
            loginValidationResult = await _loginRequestValidator.ValidateAsync(loginReq, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("LoginRequest 參數：{@LoginRequest}", loginReq);
            _logger.LogInformation("LoginRequest 驗證：{ValidationResult}", loginValidationResult);

            if (!string.IsNullOrWhiteSpace(loginValidationResult.ToString()))
            {
                return FailResult<bool>($"參數檢核未通過：{loginValidationResult}");
            }
            #endregion

            result = await _authService.LoginVerify(loginReq, _config, cancellationToken).ConfigureAwait(false);

            return result ? SuccessResult(result) : FailResult<bool>($"驗證失敗，請輸入正確帳號密碼");
        }

    }
}