using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsRoleController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListRequest>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            var allRoles = await _authManager.IsSuperAdminAsync()
                ? await _roleRepository.GetRolesAsync()
                : await _roleRepository.GetRolesByCreatorUserNameAsync(_authManager.AdminName);

            var roles = new List<Role>();
            foreach (var role in allRoles.Where(x => !_roleRepository.IsPredefinedRole(x.RoleName)))
            {
               var admin = await _administratorRepository.GetByUserNameAsync(role.CreatorUserName);
               if (admin != null)
               {
                    role.Set("AdminDisplay", _administratorRepository.GetDisplay(admin));
                    role.Set("AdminGuid", admin.Guid);
                    roles.Add(role);
               }

               var siteNames = new List<string>();
                var sitePermissionsList = await _sitePermissionsRepository.GetAllAsync(role.RoleName);
                foreach (var sitePermissions in sitePermissionsList)
                {
                    if (sitePermissions.SiteId == 0) continue;
                    var site = await _siteRepository.GetAsync(sitePermissions.SiteId);
                    if (site != null)
                    {
                        siteNames.Add(site.SiteName);
                    }
                }
                role.Set("siteNames", siteNames);
            }

            return new ListRequest
            {
                Roles = roles
            };
        }
    }
}
