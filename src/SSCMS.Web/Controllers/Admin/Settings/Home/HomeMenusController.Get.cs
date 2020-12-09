using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Home
{
    public partial class HomeMenusController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsHomeMenus))
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
