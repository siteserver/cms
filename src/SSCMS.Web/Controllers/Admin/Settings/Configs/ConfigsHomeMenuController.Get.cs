using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Web.Controllers.Admin.Settings.Configs
{
    public partial class ConfigsHomeMenuController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsConfigsHomeMenu))
            {
                return Unauthorized();
            }

            return new GetResult
            {
                UserMenus = await _userMenuRepository.GetUserMenusAsync(),
                Groups = await _userGroupRepository.GetUserGroupsAsync()
            };
        }
    }
}
