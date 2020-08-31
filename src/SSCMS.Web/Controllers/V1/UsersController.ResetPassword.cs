using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Extensions;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [HttpPost, Route(RouteUserResetPassword)]
        public async Task<ActionResult<User>> ResetPassword(int id, [FromBody]ResetPasswordRequest request)
        {
            var isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers) ||
                         _authManager.IsUser &&
                         _authManager.UserId == id ||
                         _authManager.IsAdmin &&
                         await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUsers);
            if (!isAuth) return Unauthorized();

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

            await _logRepository.AddUserLogAsync(user, "修改密码", string.Empty);

            return user;
        }
    }
}
