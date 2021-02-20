using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [OpenApiOperation("修改用户密码 API", "修改用户密码，使用POST发起请求，请求地址为/api/v1/users/actions/resetPassword")]
        [HttpPost, Route(RouteUserResetPassword)]
        public async Task<ActionResult<User>> ResetPassword([FromRoute] int id, [FromBody]ResetPasswordRequest request)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers))
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetByUserIdAsync(id);
            if (user == null) return NotFound();

            if (!_userRepository.CheckPassword(request.Password, false, user.Password, user.PasswordFormat, user.PasswordSalt))
            {
                return this.Error("原密码不正确，请重新输入");
            }

            var (success, errorMessage) = await _userRepository.ChangePasswordAsync(user.Id, request.NewPassword);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            await _logRepository.AddUserLogAsync(user, PageUtils.GetIpAddress(Request), "修改密码");

            return user;
        }
    }
}
