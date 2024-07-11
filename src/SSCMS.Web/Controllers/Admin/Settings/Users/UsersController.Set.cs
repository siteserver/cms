using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersController
    {
        [HttpPost, Route(RouteSet)]
        public async Task<ActionResult<BoolResult>> Set([FromBody] SetRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetByUserIdAsync(request.UserId);
            if (StringUtils.EndsWithIgnoreCase(request.Type, "Locked"))
            {
                user.Locked = TranslateUtils.ToBool(request.Value);
                await _authManager.AddAdminLogAsync(user.Locked ? "锁定用户" : "解锁用户", $"用户:{user.UserName}");
            }
            else if (StringUtils.EndsWithIgnoreCase(request.Type, "Manager"))
            {
                user.Manager = TranslateUtils.ToBool(request.Value);
                await _authManager.AddAdminLogAsync(user.Manager ? "设置用户为主管" : "设置用户为非主管", $"用户:{user.UserName}");
            }
            else if (StringUtils.EndsWithIgnoreCase(request.Type, "Checked"))
            {
                user.Checked = TranslateUtils.ToBool(request.Value);
                await _authManager.AddAdminLogAsync(user.Checked ? "设置用户为已审核" : "设置用户为待审核", $"用户:{user.UserName}");
            }

            await _userRepository.UpdateAsync(user);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
