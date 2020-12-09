using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsRoleController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListRequest>> GetList()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            var roleInfoList = await _authManager.IsSuperAdminAsync()
                ? await _roleRepository.GetRolesAsync()
                : await _roleRepository.GetRolesByCreatorUserNameAsync(_authManager.AdminName);

            var roles = roleInfoList.Where(x => !_roleRepository.IsPredefinedRole(x.RoleName)).ToList();

            return new ListRequest
            {
                Roles = roles
            };
        }
    }
}
