using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Extensions;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AdministratorsRoleAddController : ControllerBase
    {
        private const string Route = "settings/administratorsRoleAdd";
        private const string RouteSiteId = "settings/administratorsRoleAdd/{siteId:int}";
        private const string RouteRoleId = "settings/administratorsRoleAdd/{roleId:int}";

        private readonly ICacheManager<object> _cacheManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISitePermissionsRepository _sitePermissionsRepository;
        private readonly IPermissionsInRolesRepository _permissionsInRolesRepository;

        public AdministratorsRoleAddController(ICacheManager<object> cacheManager, ISettingsManager settingsManager, IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IRoleRepository roleRepository, ISitePermissionsRepository sitePermissionsRepository, IPermissionsInRolesRepository permissionsInRolesRepository)
        {
            _cacheManager = cacheManager;
            _settingsManager = settingsManager;
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
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsAdministratorsRole))
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

            var allAppPermissions = allPermissions.Where(x => ListUtils.ContainsIgnoreCase(x.Type, AuthTypes.Resources.App));

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
            var checkedSiteIdList = new List<int>();
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

            foreach (var sitePermissions in sitePermissionsList)
            {
                checkedSiteIdList.Add(sitePermissions.SiteId);
            }

            var list = new List<SitePermissionsResult>();
            foreach (var siteId in checkedSiteIdList)
            {
                var result = await GetSitePermissionsObjectAsync(allPermissions, request.RoleId, siteId);
                if (result != null)
                {
                    list.Add(result);
                }
            }

            return new GetResult
            {
                Role = role,
                Permissions = permissions,
                Sites = siteList,
                CheckedSiteIds = checkedSiteIdList,
                SitePermissionsList = list
            };
        }

        [HttpGet, Route(RouteSiteId)]
        public async Task<ActionResult<SitePermissionsResult>> GetSitePermissions([FromRoute]int siteId, [FromQuery]GetRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            var allPermissions = _settingsManager.GetPermissions();
            return await GetSitePermissionsObjectAsync(allPermissions, request.RoleId, siteId);
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> InsertRole([FromBody]RoleRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsAdministratorsRole))
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

            if (request.AppPermissions != null && request.AppPermissions.Count > 0)
            {
                var permissionsInRolesInfo = new PermissionsInRoles
                {
                    Id = 0,
                    RoleName = request.RoleName,
                    AppPermissions = request.AppPermissions
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
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsAdministratorsRole))
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

            if (request.AppPermissions != null && request.AppPermissions.Count > 0)
            {
                var permissionsInRolesInfo = new PermissionsInRoles
                {
                    Id = 0,
                    RoleName = request.RoleName,
                    AppPermissions = request.AppPermissions
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
