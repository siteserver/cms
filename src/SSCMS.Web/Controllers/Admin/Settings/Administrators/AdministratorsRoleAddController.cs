using System.Collections.Generic;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using SSCMS.Core.Extensions;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    [OpenApiIgnore]
    [Authorize(Roles = Constants.RoleTypeAdministrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AdministratorsRoleAddController : ControllerBase
    {
        private const string Route = "settings/administratorsRoleAdd";
        private const string RouteSiteId = "settings/administratorsRoleAdd/{siteId:int}";
        private const string RouteRoleId = "settings/administratorsRoleAdd/{roleId:int}";

        private readonly IOptionsMonitor<PermissionsOptions> _permissionsAccessor;
        private readonly ICacheManager<object> _cacheManager;
        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISitePermissionsRepository _sitePermissionsRepository;
        private readonly IPermissionsInRolesRepository _permissionsInRolesRepository;

        public AdministratorsRoleAddController(IOptionsMonitor<PermissionsOptions> permissionsAccessor, ICacheManager<object> cacheManager, IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IRoleRepository roleRepository, ISitePermissionsRepository sitePermissionsRepository, IPermissionsInRolesRepository permissionsInRolesRepository)
        {
            _permissionsAccessor = permissionsAccessor;
            _cacheManager = cacheManager;
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _roleRepository = roleRepository;
            _sitePermissionsRepository = sitePermissionsRepository;
            _permissionsInRolesRepository = permissionsInRolesRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsRole))
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
            var appPermissions = await _authManager.GetAppPermissionsAsync();
    
            var generalPermissions = _permissionsAccessor.CurrentValue.App;

            if (generalPermissions.Count > 0)
            {
                foreach (var permission in generalPermissions)
                {
                    if (appPermissions.Contains(permission.Id))
                    {
                        permissions.Add(new Permission
                        {
                            Name = permission.Id,
                            Text = permission.Text,
                            Selected = StringUtils.ContainsIgnoreCase(permissionList, permission.Id)
                        });
                    }
                }
            }

            var siteList = new List<Site>();
            var checkedSiteIdList = new List<int>();
            foreach (var permissionSiteId in await _authManager.GetSiteIdsAsync())
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
            if (!await _authManager.HasAppPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            return await GetSitePermissionsObjectAsync(request.RoleId, siteId);
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> InsertRole([FromBody]RoleRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsRole))
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
                CreatorUserName = _authManager.AdminName,
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

            _cacheManager.Clear();

            await _authManager.AddAdminLogAsync("新增管理员角色", $"角色名称:{request.RoleName}");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPut, Route(RouteRoleId)]
        public async Task<ActionResult<BoolResult>> UpdateRole([FromRoute]int roleId, [FromBody]RoleRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsRole))
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

            _cacheManager.Clear();

            await _authManager.AddAdminLogAsync("修改管理员角色", $"角色名称:{request.RoleName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
