using Microsoft.AspNetCore.Mvc;
using WebAPi.Controllers;
using Models.Dto.Responses;
using static Models.Entities.Requests.AuthEntityRequest;
using Models.Entities.Responses;
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
#if DEBUG
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

            if (result.IsLogin)
            {
                // 設定 HttpOnly cookie
                Response.Cookies.Append("accessToken", result.AccessToken!, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // 是用HTTP故要關閉
                    SameSite = SameSiteMode.Strict, // 前端和後端同站 (localhost)
                    Expires = result.AccessTokenExpiresAt,
                    Path = "/" // 讓所有 API 都能帶上
                });

                Response.Cookies.Append("refreshToken", result.RefreshToken!, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // 是用HTTP故要關閉
                    SameSite = SameSiteMode.Strict, // 前端和後端同站 (localhost)
                    Expires = result.RefreshTokenExpiresAt,
                    Path = "/" // 讓所有 API 都能帶上
                });
            }

            return SuccessResult(result);
            #endregion
        }


        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="cancellationToken">取消非同步</param>   
        /// <returns></returns>
        [Tags("Auth")]
        [HttpPost("RefreshToken")]
        public async Task<ResultResponse<bool>> RefreshToken(CancellationToken cancellationToken)
        {
            // 1️. 讀取 HttpOnly Cookie 的 RefreshToken
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return ResultResponse<bool>.FailResult("RefreshToken 不存在");

            // 2️. 從資料庫查詢 UserToken
            var userToken = await _authService.GetUserTokenByRefreshTokenAsync(refreshToken, cancellationToken).ConfigureAwait(false);
            if (userToken == null)
                return ResultResponse<bool>.FailResult("無效的 RefreshToken");

            // 3️. 判斷 RefreshToken 是否過期
            bool refreshExpired = userToken.RefreshTokenExpiresAt < DateTime.Now;

            // 判斷是否過期
            if (refreshExpired)
                return ResultResponse<bool>.FailResult("RefreshToken 已過期");

            // 4️. 如果過期，生成新的 RefreshToken，否則沿用舊的
            string newRefreshToken = refreshExpired
                ? await _tokenService.GenerateRefreshTokenAsync()
                : userToken.RefreshToken;

            DateTime refreshTokenExpiresAt = refreshExpired
                ? DateTime.Now.AddMinutes(_config.GetValue<int>("Jwt:RefreshTokenExpiresAt"))
                : userToken.RefreshTokenExpiresAt;

            var user = await _permissionService.GetUserByUserIdAsync(userToken.UserId, cancellationToken).ConfigureAwait(false);

            // 5️. 生成新的 AccessToken
            string newAccessToken = await _tokenService.GenerateTokenAsync(userToken.UserId, user.FeatureMask).ConfigureAwait(false);
            DateTime accessTokenExpiresAt = DateTime.Now.AddMinutes(_config.GetValue<int>("Jwt:AccessTokenExpiresAt"));

            // 6️. 新增一筆 UserTokens
            await _tokenService.InsertUserTokenAsync(
                userToken.UserId,
                newAccessToken,
                accessTokenExpiresAt,
                newRefreshToken,
                refreshTokenExpiresAt,
                cancellationToken
            ).ConfigureAwait(false);

            // 7️. 更新 HttpOnly cookie
            Response.Cookies.Append("accessToken", newAccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // 開發用 HTTP 可設 false
                SameSite = SameSiteMode.Strict, // 前端和後端同站 (localhost)
                Expires = accessTokenExpiresAt,
                Path = "/" // 讓所有 API 都能帶上
            });

            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict, // 前端和後端同站 (localhost)
                Expires = refreshTokenExpiresAt,
                Path = "/" // 讓所有 API 都能帶上
            });

            return ResultResponse<bool>.SuccessResult(true);
        }

    }
}