using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsController
    {
        [HttpGet, Route(RoutePermissions)]
        public async Task<ActionResult<GetPermissionsResult>> GetPermissions(int adminId)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            var roles = await _roleRepository.GetRoleNamesAsync();
            roles = roles.Where(x => !_roleRepository.IsPredefinedRole(x)).ToList();
            var allSites = await _siteRepository.GetSitesAsync();

            var adminInfo = await _administratorRepository.GetByUserIdAsync(adminId);
            var adminRoles = await _administratorsInRolesRepository.GetRolesForUserAsync(adminInfo.UserName);
            string adminLevel;
            var checkedSites = new List<int>();
            var checkedRoles = new List<string>();
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
