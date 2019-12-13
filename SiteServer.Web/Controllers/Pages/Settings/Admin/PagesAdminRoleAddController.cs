using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Admin
{
    
    [RoutePrefix("pages/settings/adminRoleAdd")]
    public partial class PagesAdminRoleAddController : ApiController
    {
        private const string Route = "";
        private const string RouteSiteId = "{siteId:int}";
        private const string RouteRoleId = "{roleId:int}";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdminRole))
                {
                    return Unauthorized();
                }

                var roleId = request.GetQueryInt("roleId");

                var systemPermissionsInfoList = new List<SitePermissions>();
                var permissionList = new List<string>();
                Role role = null;

                if (roleId > 0)
                {
                    role = await DataProvider.RoleRepository.GetRoleAsync(roleId);
                    systemPermissionsInfoList =
                        await DataProvider.SitePermissionsRepository.GetSystemPermissionsListAsync(role.RoleName);
                    permissionList =
                        await DataProvider.PermissionsInRolesRepository.GetGeneralPermissionListAsync(new[] { role.RoleName });
                }

                var permissions = new List<Permission>();
                var generalPermissionList = await request.AdminPermissionsImpl.GetPermissionListAsync();
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

                var siteList = new List<Abstractions.Site>();
                var checkedSiteIdList = new List<int>();
                foreach (var permissionSiteId in await request.AdminPermissionsImpl.GetSiteIdListAsync())
                {
                    if (await request.AdminPermissionsImpl.HasChannelPermissionsAsync(permissionSiteId, permissionSiteId) &&
                        await request.AdminPermissionsImpl.HasSitePermissionsAsync(permissionSiteId))
                    {
                        var listOne =
                            await request.AdminPermissionsImpl.GetChannelPermissionsAsync(permissionSiteId, permissionSiteId);
                        var listTwo = await request.AdminPermissionsImpl.GetSitePermissionsAsync(permissionSiteId);
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
                    sitePermissionsList.Add(await GetSitePermissionsObjectAsync(roleId, siteId, request));
                }

                return Ok(new
                {
                    Value = true,
                    RoleInfo = role,
                    Permissions = permissions,
                    SiteList = siteList,
                    CheckedSiteIdList = checkedSiteIdList,
                    SitePermissionsList = sitePermissionsList
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteSiteId)]
        public async Task<IHttpActionResult> GetSitePermissions(int siteId)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdminRole))
                {
                    return Unauthorized();
                }

                var roleId = request.GetQueryInt("roleId");

                return Ok(await GetSitePermissionsObjectAsync(roleId, siteId, request));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private async Task<object> GetSitePermissionsObjectAsync(int roleId, int siteId, AuthenticatedRequest request)
        {
            SitePermissions sitePermissionsInfo = null;
            if (roleId > 0)
            {
                var roleInfo = await DataProvider.RoleRepository.GetRoleAsync(roleId);
                sitePermissionsInfo = await DataProvider.SitePermissionsRepository.GetSystemPermissionsAsync(roleInfo.RoleName, siteId);
            }
            if (sitePermissionsInfo == null) sitePermissionsInfo = new SitePermissions();

            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            var sitePermissions = new List<Permission>();
            var pluginPermissions = new List<Permission>();
            var channelPermissions = new List<Permission>();
            var instance = await PermissionConfigManager.GetInstanceAsync();

            if (await request.AdminPermissionsImpl.IsSuperAdminAsync())
            {
                foreach (var permission in instance.WebsiteSysPermissions)
                {
                    sitePermissions.Add(new Permission
                    {
                        Name = permission.Name,
                        Text = permission.Text,
                        Selected = StringUtils.ContainsIgnoreCase(sitePermissionsInfo.WebsitePermissions, permission.Name)
                    });
                }

                foreach (var permission in instance.WebsitePluginPermissions)
                {
                    pluginPermissions.Add(new Permission
                    {
                        Name = permission.Name,
                        Text = permission.Text,
                        Selected = StringUtils.ContainsIgnoreCase(sitePermissionsInfo.WebsitePermissions, permission.Name)
                    });
                }

                var channelPermissionList = instance.ChannelPermissions;
                foreach (var permission in channelPermissionList)
                {
                    if (permission.Name == Constants.ChannelPermissions.ContentCheckLevel1)
                    {
                        if (site.CheckContentLevel < 1)
                        {
                            continue;
                        }
                    }
                    else if (permission.Name == Constants.ChannelPermissions.ContentCheckLevel2)
                    {
                        if (site.CheckContentLevel < 2)
                        {
                            continue;
                        }
                    }
                    else if (permission.Name == Constants.ChannelPermissions.ContentCheckLevel3)
                    {
                        if (site.CheckContentLevel < 3)
                        {
                            continue;
                        }
                    }
                    else if (permission.Name == Constants.ChannelPermissions.ContentCheckLevel4)
                    {
                        if (site.CheckContentLevel < 4)
                        {
                            continue;
                        }
                    }
                    else if (permission.Name == Constants.ChannelPermissions.ContentCheckLevel5)
                    {
                        if (site.CheckContentLevel < 5)
                        {
                            continue;
                        }
                    }

                    channelPermissions.Add(new Permission
                    {
                        Name = permission.Name,
                        Text = permission.Text,
                        Selected = StringUtils.ContainsIgnoreCase(sitePermissionsInfo.ChannelPermissions, permission.Name)
                    });
                }
            }
            else
            {
                if (await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId))
                {
                    var websitePermissionList = await request.AdminPermissionsImpl.GetSitePermissionsAsync(siteId);
                    foreach (var websitePermission in websitePermissionList)
                    {
                        foreach (var permission in instance.WebsiteSysPermissions)
                        {
                            if (permission.Name == websitePermission)
                            {
                                sitePermissions.Add(new Permission
                                {
                                    Name = permission.Name,
                                    Text = permission.Text,
                                    Selected = StringUtils.ContainsIgnoreCase(sitePermissionsInfo.WebsitePermissions, permission.Name)
                                });
                            }
                        }

                        foreach (var permission in instance.WebsitePluginPermissions)
                        {
                            if (permission.Name == websitePermission)
                            {
                                pluginPermissions.Add(new Permission
                                {
                                    Name = permission.Name,
                                    Text = permission.Text,
                                    Selected = StringUtils.ContainsIgnoreCase(sitePermissionsInfo.WebsitePermissions, permission.Name)
                                });
                            }
                        }
                    }
                }

                var channelPermissionList = await request.AdminPermissionsImpl.GetChannelPermissionsAsync(siteId);
                foreach (var channelPermission in channelPermissionList)
                {
                    foreach (var permission in instance.ChannelPermissions)
                    {
                        if (permission.Name == channelPermission)
                        {
                            if (channelPermission == Constants.ChannelPermissions.ContentCheckLevel1)
                            {
                                if (site.CheckContentLevel < 1) continue;
                            }
                            else if (channelPermission == Constants.ChannelPermissions.ContentCheckLevel2)
                            {
                                if (site.CheckContentLevel < 2) continue;
                            }
                            else if (channelPermission == Constants.ChannelPermissions.ContentCheckLevel3)
                            {
                                if ( site.CheckContentLevel < 3) continue;
                            }
                            else if (channelPermission == Constants.ChannelPermissions.ContentCheckLevel4)
                            {
                                if (site.CheckContentLevel < 4) continue;
                            }
                            else if (channelPermission == Constants.ChannelPermissions.ContentCheckLevel5)
                            {
                                if (site.CheckContentLevel < 5) continue;
                            }

                            channelPermissions.Add(new Permission
                            {
                                Name = permission.Name,
                                Text = permission.Text,
                                Selected = StringUtils.ContainsIgnoreCase(sitePermissionsInfo.ChannelPermissions, permission.Name)
                            });
                        }
                    }
                }
            }

            var channelInfo = await ChannelManager.GetChannelAsync(siteId, siteId);
            channelInfo.Children = await ChannelManager.GetChildrenAsync(siteId, siteId);
            var checkedChannelIdList = new List<int>();
            foreach (var i in sitePermissionsInfo.ChannelIdList)
            {
                if (!checkedChannelIdList.Contains(i))
                {
                    checkedChannelIdList.Add(i);
                }
            }

            return new
            {
                Value = siteId,
                SitePermissions = sitePermissions,
                PluginPermissions = pluginPermissions,
                ChannelPermissions = channelPermissions,
                ChannelInfo = channelInfo,
                CheckedChannelIdList = checkedChannelIdList
            };
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> InsertRole()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdminRole))
                {
                    return Unauthorized();
                }

                var roleName = request.GetPostString("roleName");
                var description = request.GetPostString("description");
                var generalPermissionList = request.GetPostObject<List<string>>("generalPermissions");
                var sitePermissionsInRolesInfoList =
                    request.GetPostObject<List<SitePermissions>>("sitePermissions");

                if (EPredefinedRoleUtils.IsPredefinedRole(roleName))
                {
                    return BadRequest($"角色添加失败，{roleName}为系统角色！");
                }
                if (await DataProvider.RoleRepository.IsRoleExistsAsync(roleName))
                {
                    return BadRequest("角色名称已存在，请更换角色名称！");
                }

                await DataProvider.RoleRepository.InsertRoleAsync(new Role
                {
                    RoleName = roleName,
                    CreatorUserName = request.AdminName,
                    Description = description
                });

                if (generalPermissionList != null && generalPermissionList.Count > 0)
                {
                    var permissionsInRolesInfo = new PermissionsInRoles
                    {
                        Id = 0,
                        RoleName = roleName,
                        GeneralPermissionList = generalPermissionList
                    };
                    await DataProvider.PermissionsInRolesRepository.InsertAsync(permissionsInRolesInfo);
                }

                if (sitePermissionsInRolesInfoList != null && sitePermissionsInRolesInfoList.Count > 0)
                {
                    foreach (var sitePermissionsInfo in sitePermissionsInRolesInfoList)
                    {
                        sitePermissionsInfo.RoleName = roleName;
                        await DataProvider.SitePermissionsRepository.InsertAsync(sitePermissionsInfo);
                    }
                }

                PermissionsImpl.ClearAllCache();

                await request.AddAdminLogAsync("新增管理员角色", $"角色名称:{roleName}");

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route(RouteRoleId)]
        public async Task<IHttpActionResult> UpdateRole(int roleId)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdminRole))
                {
                    return Unauthorized();
                }

                var roleName = request.GetPostString("roleName");
                var description = request.GetPostString("description");
                var generalPermissionList = request.GetPostObject<List<string>>("generalPermissions");
                var sitePermissionsInRolesInfoList =
                    request.GetPostObject<List<SitePermissions>>("sitePermissions");

                var roleInfo = await DataProvider.RoleRepository.GetRoleAsync(roleId);
                if (roleInfo.RoleName != roleName)
                {
                    if (EPredefinedRoleUtils.IsPredefinedRole(roleName))
                    {
                        return BadRequest($"角色添加失败，{roleName}为系统角色！");
                    }
                    if (await DataProvider.RoleRepository.IsRoleExistsAsync(roleName))
                    {
                        return BadRequest("角色名称已存在，请更换角色名称！");
                    }
                }

                await DataProvider.PermissionsInRolesRepository.DeleteAsync(roleInfo.RoleName);
                await DataProvider.SitePermissionsRepository.DeleteAsync(roleInfo.RoleName);

                if (generalPermissionList != null && generalPermissionList.Count > 0)
                {
                    var permissionsInRolesInfo = new PermissionsInRoles
                    {
                        Id = 0,
                        RoleName = roleName,
                        GeneralPermissionList = generalPermissionList
                    };
                    await DataProvider.PermissionsInRolesRepository.InsertAsync(permissionsInRolesInfo);
                }

                if (sitePermissionsInRolesInfoList != null && sitePermissionsInRolesInfoList.Count > 0)
                {
                    foreach (var sitePermissionsInfo in sitePermissionsInRolesInfoList)
                    {
                        sitePermissionsInfo.RoleName = roleName;
                        await DataProvider.SitePermissionsRepository.InsertAsync(sitePermissionsInfo);
                    }
                }

                roleInfo.RoleName = roleName;
                roleInfo.Description = description;

                await DataProvider.RoleRepository.UpdateRoleAsync(roleInfo);

                PermissionsImpl.ClearAllCache();

                await request.AddAdminLogAsync("修改管理员角色", $"角色名称:{roleName}");

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
