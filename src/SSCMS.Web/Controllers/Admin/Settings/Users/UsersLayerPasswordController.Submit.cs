using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersLayerPasswordController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetByUserIdAsync(request.UserId);
            if (user == null) return NotFound();

            var (success, errorMessage) = await _userRepository.ChangePasswordAsync(user.Id, request.Password);
            if (!success)
            {
                return this.Error($"更改密码失败：{errorMessage}");
            }

            await _authManager.AddAdminLogAsync("重设用户密码", $"用户:{user.UserName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
