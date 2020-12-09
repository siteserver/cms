using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Home
{
    public partial class HomeMenusController
    {
        [HttpPost, Route(RouteReset)]
        public async Task<ActionResult<UserMenusResult>> Reset()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsHomeMenus))
            {
                return Unauthorized();
            }

            await _userMenuRepository.ResetAsync();
            await _authManager.AddAdminLogAsync("重置用户菜单");

            return new UserMenusResult
            {
                UserMenus = await _userMenuRepository.GetUserMenusAsync()
            };
        }
    }
}
