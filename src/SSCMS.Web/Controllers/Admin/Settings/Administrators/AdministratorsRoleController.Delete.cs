using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsRoleController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<ListRequest>> Delete([FromBody] IdRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            var roleInfo = await _roleRepository.GetRoleAsync(request.Id);

            await _permissionsInRolesRepository.DeleteAsync(roleInfo.RoleName);
            await _sitePermissionsRepository.DeleteAsync(roleInfo.RoleName);
            await _roleRepository.DeleteRoleAsync(roleInfo.Id);

            await _authManager.AddAdminLogAsync("删除管理员角色", $"角色名称:{roleInfo.RoleName}");

            var roles = await _authManager.IsSuperAdminAsync()
                ? await _roleRepository.GetRolesAsync()
                : await _roleRepository.GetRolesByCreatorUserNameAsync(_authManager.AdminName);

            return new ListRequest
            {
                Roles = roles
            };
        }
    }
}
