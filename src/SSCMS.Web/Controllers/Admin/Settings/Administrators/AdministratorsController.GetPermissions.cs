using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsController
    {
        [HttpGet, Route(RoutePermissions)]
        public async Task<ActionResult<GetPermissionsResult>> GetPermissions(int adminId)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            var isSuperAdmin = await _authManager.IsSuperAdminAsync();

            List<string> roles;
            var allSites = new List<Site>();
            string adminLevel;
            var checkedSites = new List<int>();
            var checkedRoles = new List<string>();

            if (isSuperAdmin)
            {
                roles = await _roleRepository.GetRoleNamesAsync();
                roles = roles.Where(x => !_roleRepository.IsPredefinedRole(x)).ToList();

                allSites = await _siteRepository.GetSitesAsync();

                var adminInfo = await _administratorRepository.GetByUserIdAsync(adminId);
                var adminRoles = await _administratorsInRolesRepository.GetRolesForUserAsync(adminInfo.UserName);
                if (_roleRepository.IsConsoleAdministrator(adminRoles))
                {
                    adminLevel = "SuperAdmin";
                }
                else if (_roleRepository.IsSystemAdministrator(adminRoles))
                {
                    adminLevel = "SiteAdmin";
                    checkedSites = adminInfo.SiteIds;
                }
                else
                {
                    adminLevel = "Admin";
                    foreach (var role in roles)
                    {
                        if (!checkedRoles.Contains(role) && !_roleRepository.IsPredefinedRole(role) && adminRoles.Contains(role))
                        {
                            checkedRoles.Add(role);
                        }
                    }
                }
            }
            else
            {
                roles = await _roleRepository.GetRoleNamesByCreatorUserNameAsync(_authManager.AdminName);
                roles = roles.Where(x => !_roleRepository.IsPredefinedRole(x)).ToList();

                var adminInfo = await _administratorRepository.GetByUserIdAsync(adminId);
                var adminRoles = await _administratorsInRolesRepository.GetRolesForUserAsync(adminInfo.UserName);

                adminLevel = "Admin";
                foreach (var role in roles)
                {
                    if (!checkedRoles.Contains(role) && !_roleRepository.IsPredefinedRole(role) && adminRoles.Contains(role))
                    {
                        checkedRoles.Add(role);
                    }
                }
            }

            return new GetPermissionsResult
            {
                Roles = roles,
                AllSites = allSites,
                AdminLevel = adminLevel,
                CheckedSites = checkedSites,
                CheckedRoles = checkedRoles
            };
        }
    }
}
