using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Models;

namespace SS.CMS.Api.Controllers.Users
{
    public partial class UsersController
    {
        /// <summary>
        /// User login
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /users/login
        ///
        /// </remarks>
        /// <returns>User info</returns>
        /// <response code="200">Returns the user token</response>
        /// <response code="400">If current user is not exists</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            User userInfo;

            var (isSuccess, userName, errorMessage) = await _userRepository.ValidateAsync(request.UserName, request.Password, true);

            if (!isSuccess)
            {
                userInfo = await _userRepository.GetByUserNameAsync(userName);
                if (userInfo != null)
                {
                    await _userRepository.UpdateLastActivityDateAndCountOfFailedLoginAsync(userInfo); // 记录最后登录时间、失败次数+1
                }
                return BadRequest(new
                {
                    Code = 400,
                    Message = errorMessage
                });
            }

            userInfo = await _userRepository.GetByUserNameAsync(userName);
            await _userRepository.UpdateLastActivityDateAndCountOfLoginAsync(userInfo); // 记录最后登录时间、失败次数清零
            //var accessToken = AdminLogin(userInfo.UserName, context.IsAutoLogin);
            //var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);

            var token = _userManager.GetToken(userInfo);

            return new LoginResponse
            {
                Token = token,
                //AccessToken = accessToken,
                //ExpiresAt = expiresAt
            };
        }

        public class LoginRequest
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public string Captcha { get; set; }
            public bool IsAutoLogin { get; set; }
        }

        public class LoginResponse
        {
            public string Token { get; set; }
        }
    }
}