using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Settings.Configs
{
    public partial class ConfigsHomeMenuController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsConfigsHomeMenu))
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
