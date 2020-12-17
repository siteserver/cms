using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsRoleAddController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            var sitePermissionsList = new List<SitePermissions>();
            var permissionList = new List<string>();
            Role role = null;

            if (request.RoleId > 0)
            {
                role = await _roleRepository.GetRoleAsync(request.RoleId);
                sitePermissionsList =
                    await _sitePermissionsRepository.GetAllAsync(role.RoleName);
                permissionList =
                    await _permissionsInRolesRepository.GetAppPermissionsAsync(new[] { role.RoleName });
            }

            var permissions = new List<Option>();
            var appPermissions = await _authManager.GetAppPermissionsAsync();

            var allPermissions = _settingsManager.GetPermissions();

            var allAppPermissions = allPermissions.Where(x => ListUtils.ContainsIgnoreCase(x.Type, Types.Resources.App));

            foreach (var permission in allAppPermissions)
            {
                if (appPermissions.Contains(permission.Id))
                {
                    permissions.Add(new Option
                    {
                        Name = permission.Id,
                        Text = permission.Text,
                        Selected = ListUtils.ContainsIgnoreCase(permissionList, permission.Id)
                    });
                }
            }

            var siteList = new List<Site>();
            foreach (var permissionSiteId in await _authManager.GetSiteIdsAsync())
            {
                if (!await _authManager.HasChannelPermissionsAsync(permissionSiteId, permissionSiteId) ||
                    !await _authManager.HasSitePermissionsAsync(permissionSiteId)) continue;

                var listOne =
                    await _authManager.GetChannelPermissionsAsync(permissionSiteId, permissionSiteId);
                var listTwo = await _authManager.GetSitePermissionsAsync(permissionSiteId);
                if (listOne != null && listOne.Count > 0 || listTwo != null && listTwo.Count > 0)
                {
                    siteList.Add(await _siteRepository.GetAsync(permissionSiteId));
                }
            }

            return new GetResult
            {
                Role = role,
                Permissions = permissions,
                Sites = siteList,
                SitePermissions = sitePermissionsList
            };
        }
    }
}