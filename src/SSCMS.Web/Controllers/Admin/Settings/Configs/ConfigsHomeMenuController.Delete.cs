using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Configs
{
    public partial class ConfigsHomeMenuController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<UserMenusResult>> Delete([FromBody] IdRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsConfigsHomeMenu))
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
