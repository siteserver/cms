using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Home
{
    public partial class HomeMenusController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<UserMenusResult>> Delete([FromBody] IdRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsHomeMenus))
            {
                return Unauthorized();
            }

            await _userMenuRepository.DeleteAsync(request.Id);

            return new UserMenusResult
            {
                UserMenus = await _userMenuRepository.GetUserMenusAsync()
            };
        }
    }
}
