using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsController
    {
        [HttpPost, Route(RoutePermissions)]
        public async Task<ActionResult<SavePermissionsResult>> SavePermissions([FromRoute] int adminId, [FromBody] SavePermissionsRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            var adminInfo = await _administratorRepository.GetByUserIdAsync(adminId);

            await _administratorsInRolesRepository.RemoveUserAsync(adminInfo.UserName);
            if (request.AdminLevel == "SuperAdmin")
            {
                await _administratorRepository.AddUserToRoleAsync(adminInfo.UserName, PredefinedRole.ConsoleAdministrator.GetValue());
            }
            else if (request.AdminLevel == "SiteAdmin")
            {
                await _administratorRepository.AddUserToRoleAsync(adminInfo.UserName, PredefinedRole.SystemAdministrator.GetValue());
            }
            else
            {
                await _administratorRepository.AddUserToRoleAsync(adminInfo.UserName, PredefinedRole.Administrator.GetValue());
                await _administratorRepository.AddUserToRolesAsync(adminInfo.UserName, request.CheckedRoles.ToArray());
            }

            await _administratorRepository.UpdateSiteIdsAsync(adminInfo,
                request.AdminLevel == "SiteAdmin"
                    ? request.CheckedSites
                    : new List<int>());

            _cacheManager.Clear();

            await _authManager.AddAdminLogAsync("设置管理员权限", $"管理员:{adminInfo.UserName}");

            return new SavePermissionsResult
            {
                Roles = await _administratorRepository.GetRolesAsync(adminInfo.UserName)
            };
        }
    }
}
