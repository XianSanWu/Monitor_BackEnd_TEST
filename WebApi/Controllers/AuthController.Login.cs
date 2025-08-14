using Microsoft.AspNetCore.Mvc;
using WebAPi.Controllers;
using Models.Dto.Responses;
using static Models.Dto.Requests.AuthRequest;

namespace WebApi.Controllers
{
    public partial class AuthController : BaseController
    {
        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="loginReq">return bool</param>
        /// <param name="cancellationToken">取消非同步</param>   
        /// <returns></returns>
        [Tags("Auth")]  //分組(可多標籤)        
        [HttpPost("Login")]
        public async Task<ResultResponse<AuthResponse>> Login(LoginRequest loginReq, CancellationToken cancellationToken)
        {
            #region 參數宣告
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

            #region 驗證
            //loginValidationResult = await _loginRequestValidator.ValidateAsync(loginReq, cancellationToken).ConfigureAwait(false);
            //_logger.LogInformation("LoginRequest 參數：{@LoginRequest}", loginReq);
            //_logger.LogInformation("LoginRequest 驗證：{ValidationResult}", loginValidationResult);

            //if (!string.IsNullOrWhiteSpace(loginValidationResult.ToString()))
            //{
            //    return FailResult<bool>($"參數檢核未通過：{loginValidationResult}");
            //}
            #endregion

            #region 流程
            result = await _authService.Login(loginReq, cancellationToken).ConfigureAwait(false);

            return SuccessResult(result);
            #endregion
        }

    }
}