using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Settings.Configs
{
    public partial class ConfigsHomeMenuController
    {
        [HttpPost, Route(RouteReset)]
        public async Task<ActionResult<UserMenusResult>> Reset()
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsConfigsHomeMenu))
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
