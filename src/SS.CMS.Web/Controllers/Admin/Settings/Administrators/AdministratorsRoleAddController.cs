using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    [Route("admin/settings/administratorsRoleAdd")]
    public partial class AdministratorsRoleAddController : ControllerBase
    {
        private const string Route = "";
        private const string RouteSiteId = "{siteId:int}";
        private const string RouteRoleId = "{roleId:int}";

        private readonly IAuthManager _authManager;

        public AdministratorsRoleAddController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            var systemPermissionsInfoList = new List<SitePermissions>();
            var permissionList = new List<string>();
            Role role = null;

            if (request.RoleId > 0)
            {
                role = await DataProvider.RoleRepository.GetRoleAsync(request.RoleId);
                systemPermissionsInfoList =
                    await DataProvider.SitePermissionsRepository.GetSystemPermissionsListAsync(role.RoleName);
                permissionList =
                    await DataProvider.PermissionsInRolesRepository.GetGeneralPermissionListAsync(new[] { role.RoleName });
            }

            var permissions = new List<Permission>();
            var generalPermissionList = await auth.AdminPermissions.GetPermissionListAsync();
            var instance = await PermissionConfigManager.GetInstanceAsync();
            var generalPermissions = instance.GeneralPermissions;

            if (generalPermissions.Count > 0)
            {
                foreach (var permission in generalPermissions)
                {
                    if (generalPermissionList.Contains(permission.Name))
                    {
                        permissions.Add(new Permission
                        {
                            Name = permission.Name,
                            Text = permission.Text,
                            Selected = StringUtils.ContainsIgnoreCase(permissionList, permission.Name)
                        });
                    }
                }
            }

            var siteList = new List<Site>();
            var checkedSiteIdList = new List<int>();
            foreach (var permissionSiteId in await auth.AdminPermissions.GetSiteIdListAsync())
            {
                if (await auth.AdminPermissions.HasChannelPermissionsAsync(permissionSiteId, permissionSiteId) &&
                    await auth.AdminPermissions.HasSitePermissionsAsync(permissionSiteId))
                {
                    var listOne =
                        await auth.AdminPermissions.GetChannelPermissionsAsync(permissionSiteId, permissionSiteId);
                    var listTwo = await auth.AdminPermissions.GetSitePermissionsAsync(permissionSiteId);
                    if (listOne != null && listOne.Count > 0 || listTwo != null && listTwo.Count > 0)
                    {
                        siteList.Add(await DataProvider.SiteRepository.GetAsync(permissionSiteId));
                    }
                }
            }

            foreach (var systemPermissionsInfo in systemPermissionsInfoList)
            {
                checkedSiteIdList.Add(systemPermissionsInfo.SiteId);
            }

            var sitePermissionsList = new List<object>();
            foreach (var siteId in checkedSiteIdList)
            {
                sitePermissionsList.Add(await GetSitePermissionsObjectAsync(request.RoleId, siteId));
            }

            return new GetResult
            {
                Role = role,
                Permissions = permissions,
                Sites = siteList,
                CheckedSiteIds = checkedSiteIdList,
                SitePermissionsList = sitePermissionsList
            };
        }

        [HttpGet, Route(RouteSiteId)]
        public async Task<ActionResult<SitePermissionsResult>> GetSitePermissions([FromRoute]int siteId, [FromQuery]GetRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            return await GetSitePermissionsObjectAsync(request.RoleId, siteId);
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> InsertRole([FromBody]RoleRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            if (DataProvider.RoleRepository.IsPredefinedRole(request.RoleName))
            {
                return this.Error($"角色添加失败，{request.RoleName}为系统角色！");
            }
            if (await DataProvider.RoleRepository.IsRoleExistsAsync(request.RoleName))
            {
                return this.Error("角色名称已存在，请更换角色名称！");
            }

            await DataProvider.RoleRepository.InsertRoleAsync(new Role
            {
                RoleName = request.RoleName,
                CreatorUserName = auth.AdminName,
                Description = request.Description
            });

            if (request.GeneralPermissions != null && request.GeneralPermissions.Count > 0)
            {
                var permissionsInRolesInfo = new PermissionsInRoles
                {
                    Id = 0,
                    RoleName = request.RoleName,
                    GeneralPermissions = request.GeneralPermissions
                };
                await DataProvider.PermissionsInRolesRepository.InsertAsync(permissionsInRolesInfo);
            }

            if (request.SitePermissions != null && request.SitePermissions.Count > 0)
            {
                foreach (var sitePermissionsInfo in request.SitePermissions)
                {
                    sitePermissionsInfo.RoleName = request.RoleName;
                    await DataProvider.SitePermissionsRepository.InsertAsync(sitePermissionsInfo);
                }
            }

            CacheUtils.ClearAll();

            await auth.AddAdminLogAsync("新增管理员角色", $"角色名称:{request.RoleName}");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPut, Route(RouteRoleId)]
        public async Task<ActionResult<BoolResult>> UpdateRole([FromRoute]int roleId, [FromBody]RoleRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            var roleInfo = await DataProvider.RoleRepository.GetRoleAsync(roleId);
            if (roleInfo.RoleName != request.RoleName)
            {
                if (DataProvider.RoleRepository.IsPredefinedRole(request.RoleName))
                {
                    return this.Error($"角色添加失败，{request.RoleName}为系统角色！");
                }
                if (await DataProvider.RoleRepository.IsRoleExistsAsync(request.RoleName))
                {
                    return this.Error("角色名称已存在，请更换角色名称！");
                }
            }

            await DataProvider.PermissionsInRolesRepository.DeleteAsync(roleInfo.RoleName);
            await DataProvider.SitePermissionsRepository.DeleteAsync(roleInfo.RoleName);

            if (request.GeneralPermissions != null && request.GeneralPermissions.Count > 0)
            {
                var permissionsInRolesInfo = new PermissionsInRoles
                {
                    Id = 0,
                    RoleName = request.RoleName,
                    GeneralPermissions = request.GeneralPermissions
                };
                await DataProvider.PermissionsInRolesRepository.InsertAsync(permissionsInRolesInfo);
            }

            if (request.SitePermissions != null && request.SitePermissions.Count > 0)
            {
                foreach (var sitePermissionsInfo in request.SitePermissions)
                {
                    sitePermissionsInfo.RoleName = request.RoleName;
                    await DataProvider.SitePermissionsRepository.InsertAsync(sitePermissionsInfo);
                }
            }

            roleInfo.RoleName = request.RoleName;
            roleInfo.Description = request.Description;

            await DataProvider.RoleRepository.UpdateRoleAsync(roleInfo);

            CacheUtils.ClearAll();

            await auth.AddAdminLogAsync("修改管理员角色", $"角色名称:{request.RoleName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
