using System.Collections.Generic;
using System.Linq;
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
            if (adminInfo == null || !await _authManager.CanManageAdministratorAsync(adminInfo))
            {
                return Unauthorized();
            }

            var isSuperAdmin = await _authManager.IsSuperAdminAsync();
            // 防止提权：非超级管理员只能分配普通管理员级别，且只能分配自己创建的角色；不能修改自己的权限
            if (!isSuperAdmin)
            {
                if (adminInfo.Id == _authManager.AdminId || request.AdminLevel == "SuperAdmin" || request.AdminLevel == "SiteAdmin")
                {
                    return Unauthorized();
                }
                var ownRoles = await _roleRepository.GetRoleNamesByCreatorUserNameAsync(_authManager.AdminName);
                if (request.CheckedRoles != null && request.CheckedRoles.Any(r => !ownRoles.Contains(r)))
                {
                    return Unauthorized();
                }
            }

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
