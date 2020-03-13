using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    [Route("admin/settings/administratorsRoleAdd")]
    public partial class AdministratorsRoleAddController : ControllerBase
    {
        private const string Route = "";
        private const string RouteSiteId = "{siteId:int}";
        private const string RouteRoleId = "{roleId:int}";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISitePermissionsRepository _sitePermissionsRepository;
        private readonly IPermissionsInRolesRepository _permissionsInRolesRepository;

        public AdministratorsRoleAddController(IAuthManager authManager, IPathManager pathManager, IPluginManager pluginManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IRoleRepository roleRepository, ISitePermissionsRepository sitePermissionsRepository, IPermissionsInRolesRepository permissionsInRolesRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _roleRepository = roleRepository;
            _sitePermissionsRepository = sitePermissionsRepository;
            _permissionsInRolesRepository = permissionsInRolesRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            var systemPermissionsInfoList = new List<SitePermissions>();
            var permissionList = new List<string>();
            Role role = null;

            if (request.RoleId > 0)
            {
                role = await _roleRepository.GetRoleAsync(request.RoleId);
                systemPermissionsInfoList =
                    await _sitePermissionsRepository.GetSystemPermissionsListAsync(role.RoleName);
                permissionList =
                    await _permissionsInRolesRepository.GetGeneralPermissionListAsync(new[] { role.RoleName });
            }

            var permissions = new List<Permission>();
            var generalPermissionList = await _authManager.GetPermissionListAsync();
            var instance = await PermissionConfigManager.GetInstanceAsync(_pathManager, _pluginManager);
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
            foreach (var permissionSiteId in await _authManager.GetSiteIdListAsync())
            {
                if (await _authManager.HasChannelPermissionsAsync(permissionSiteId, permissionSiteId) &&
                    await _authManager.HasSitePermissionsAsync(permissionSiteId))
                {
                    var listOne =
                        await _authManager.GetChannelPermissionsAsync(permissionSiteId, permissionSiteId);
                    var listTwo = await _authManager.GetSitePermissionsAsync(permissionSiteId);
                    if (listOne != null && listOne.Count > 0 || listTwo != null && listTwo.Count > 0)
                    {
                        siteList.Add(await _siteRepository.GetAsync(permissionSiteId));
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
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            return await GetSitePermissionsObjectAsync(request.RoleId, siteId);
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> InsertRole([FromBody]RoleRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            if (_roleRepository.IsPredefinedRole(request.RoleName))
            {
                return this.Error($"角色添加失败，{request.RoleName}为系统角色！");
            }
            if (await _roleRepository.IsRoleExistsAsync(request.RoleName))
            {
                return this.Error("角色名称已存在，请更换角色名称！");
            }

            await _roleRepository.InsertRoleAsync(new Role
            {
                RoleName = request.RoleName,
                CreatorUserName = await _authManager.GetAdminNameAsync(),
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
                await _permissionsInRolesRepository.InsertAsync(permissionsInRolesInfo);
            }

            if (request.SitePermissions != null && request.SitePermissions.Count > 0)
            {
                foreach (var sitePermissionsInfo in request.SitePermissions)
                {
                    sitePermissionsInfo.RoleName = request.RoleName;
                    await _sitePermissionsRepository.InsertAsync(sitePermissionsInfo);
                }
            }

            CacheUtils.ClearAll();

            await _authManager.AddAdminLogAsync("新增管理员角色", $"角色名称:{request.RoleName}");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPut, Route(RouteRoleId)]
        public async Task<ActionResult<BoolResult>> UpdateRole([FromRoute]int roleId, [FromBody]RoleRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            var roleInfo = await _roleRepository.GetRoleAsync(roleId);
            if (roleInfo.RoleName != request.RoleName)
            {
                if (_roleRepository.IsPredefinedRole(request.RoleName))
                {
                    return this.Error($"角色添加失败，{request.RoleName}为系统角色！");
                }
                if (await _roleRepository.IsRoleExistsAsync(request.RoleName))
                {
                    return this.Error("角色名称已存在，请更换角色名称！");
                }
            }

            await _permissionsInRolesRepository.DeleteAsync(roleInfo.RoleName);
            await _sitePermissionsRepository.DeleteAsync(roleInfo.RoleName);

            if (request.GeneralPermissions != null && request.GeneralPermissions.Count > 0)
            {
                var permissionsInRolesInfo = new PermissionsInRoles
                {
                    Id = 0,
                    RoleName = request.RoleName,
                    GeneralPermissions = request.GeneralPermissions
                };
                await _permissionsInRolesRepository.InsertAsync(permissionsInRolesInfo);
            }

            if (request.SitePermissions != null && request.SitePermissions.Count > 0)
            {
                foreach (var sitePermissionsInfo in request.SitePermissions)
                {
                    sitePermissionsInfo.RoleName = request.RoleName;
                    await _sitePermissionsRepository.InsertAsync(sitePermissionsInfo);
                }
            }

            roleInfo.RoleName = request.RoleName;
            roleInfo.Description = request.Description;

            await _roleRepository.UpdateRoleAsync(roleInfo);

            CacheUtils.ClearAll();

            await _authManager.AddAdminLogAsync("修改管理员角色", $"角色名称:{request.RoleName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
