using System;
using System.Collections.Generic;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.API.Results.Pages.Settings;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/adminRoleAdd")]
    public class PagesAdminRoleAddController : ApiController
    {
        private const string Route = "";
        private const string RouteSiteId = "{siteId:int}";
        private const string RouteRoleId = "{roleId:int}";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsAdminRole))
                {
                    return Unauthorized();
                }

                var roleId = request.GetQueryInt("roleId");

                var systemPermissionsInfoList = new List<SitePermissionsInfo>();
                var permissionList = new List<string>();
                RoleInfo roleInfo = null;

                if (roleId > 0)
                {
                    roleInfo = DataProvider.RoleDao.GetRoleInfo(roleId);
                    systemPermissionsInfoList =
                        DataProvider.SitePermissionsDao.GetSystemPermissionsInfoList(roleInfo.RoleName);
                    permissionList =
                        DataProvider.PermissionsInRolesDao.GetGeneralPermissionList(new[] { roleInfo.RoleName });
                }

                var permissions = new List<Permission>();
                var generalPermissionList = request.AdminPermissionsImpl.PermissionList;
                var generalPermissions = PermissionConfigManager.Instance.GeneralPermissions;

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

                var siteInfoList = new List<SiteInfo>();
                var checkedSiteIdList = new List<int>();
                foreach (var permissionSiteId in request.AdminPermissionsImpl.GetSiteIdList())
                {
                    if (request.AdminPermissionsImpl.HasChannelPermissions(permissionSiteId, permissionSiteId) &&
                        request.AdminPermissionsImpl.HasSitePermissions(permissionSiteId))
                    {
                        var listOne =
                            request.AdminPermissionsImpl.GetChannelPermissions(permissionSiteId, permissionSiteId);
                        var listTwo = request.AdminPermissionsImpl.GetSitePermissions(permissionSiteId);
                        if (listOne != null && listOne.Count > 0 || listTwo != null && listTwo.Count > 0)
                        {
                            siteInfoList.Add(SiteManager.GetSiteInfo(permissionSiteId));
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
                    sitePermissionsList.Add(GetSitePermissionsObject(roleId, siteId, request));
                }

                return Ok(new
                {
                    Value = true,
                    RoleInfo = roleInfo,
                    Permissions = permissions,
                    SiteInfoList = siteInfoList,
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
        public IHttpActionResult GetSitePermissions(int siteId)
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsAdminRole))
                {
                    return Unauthorized();
                }

                var roleId = request.GetQueryInt("roleId");

                return Ok(GetSitePermissionsObject(roleId, siteId, request));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private object GetSitePermissionsObject(int roleId, int siteId, AuthenticatedRequest request)
        {
            SitePermissionsInfo sitePermissionsInfo = null;
            if (roleId > 0)
            {
                var roleInfo = DataProvider.RoleDao.GetRoleInfo(roleId);
                sitePermissionsInfo = DataProvider.SitePermissionsDao.GetSystemPermissionsInfo(roleInfo.RoleName, siteId);
            }
            if (sitePermissionsInfo == null) sitePermissionsInfo = new SitePermissionsInfo();

            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var sitePermissions = new List<Permission>();
            var pluginPermissions = new List<Permission>();
            var channelPermissions = new List<Permission>();
            if (request.AdminPermissionsImpl.IsSystemAdministrator)
            {
                foreach (var permission in PermissionConfigManager.Instance.WebsiteSysPermissions)
                {
                    sitePermissions.Add(new Permission
                    {
                        Name = permission.Name,
                        Text = permission.Text,
                        Selected = StringUtils.In(sitePermissionsInfo.WebsitePermissions, permission.Name)
                    });
                }

                foreach (var permission in PermissionConfigManager.Instance.WebsitePluginPermissions)
                {
                    pluginPermissions.Add(new Permission
                    {
                        Name = permission.Name,
                        Text = permission.Text,
                        Selected = StringUtils.In(sitePermissionsInfo.WebsitePermissions, permission.Name)
                    });
                }

                var channelPermissionList = PermissionConfigManager.Instance.ChannelPermissions;
                foreach (var permission in channelPermissionList)
                {
                    if (permission.Name == ConfigManager.ChannelPermissions.ContentCheckLevel1)
                    {
                        if (siteInfo.Additional.IsCheckContentLevel)
                        {
                            if (siteInfo.Additional.CheckContentLevel < 1)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (permission.Name == ConfigManager.ChannelPermissions.ContentCheckLevel2)
                    {
                        if (siteInfo.Additional.IsCheckContentLevel)
                        {
                            if (siteInfo.Additional.CheckContentLevel < 2)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (permission.Name == ConfigManager.ChannelPermissions.ContentCheckLevel3)
                    {
                        if (siteInfo.Additional.IsCheckContentLevel)
                        {
                            if (siteInfo.Additional.CheckContentLevel < 3)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (permission.Name == ConfigManager.ChannelPermissions.ContentCheckLevel4)
                    {
                        if (siteInfo.Additional.IsCheckContentLevel)
                        {
                            if (siteInfo.Additional.CheckContentLevel < 4)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (permission.Name == ConfigManager.ChannelPermissions.ContentCheckLevel5)
                    {
                        if (siteInfo.Additional.IsCheckContentLevel)
                        {
                            if (siteInfo.Additional.CheckContentLevel < 5)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    channelPermissions.Add(new Permission
                    {
                        Name = permission.Name,
                        Text = permission.Text,
                        Selected = StringUtils.In(sitePermissionsInfo.ChannelPermissions, permission.Name)
                    });
                }
            }
            else
            {
                if (request.AdminPermissionsImpl.HasSitePermissions(siteId))
                {
                    var websitePermissionList = request.AdminPermissionsImpl.GetSitePermissions(siteId);
                    foreach (var websitePermission in websitePermissionList)
                    {
                        foreach (var permission in PermissionConfigManager.Instance.WebsiteSysPermissions)
                        {
                            if (permission.Name == websitePermission)
                            {
                                sitePermissions.Add(new Permission
                                {
                                    Name = permission.Name,
                                    Text = permission.Text,
                                    Selected = StringUtils.In(sitePermissionsInfo.WebsitePermissions, permission.Name)
                                });
                            }
                        }

                        foreach (var permission in PermissionConfigManager.Instance.WebsitePluginPermissions)
                        {
                            if (permission.Name == websitePermission)
                            {
                                pluginPermissions.Add(new Permission
                                {
                                    Name = permission.Name,
                                    Text = permission.Text,
                                    Selected = StringUtils.In(sitePermissionsInfo.WebsitePermissions, permission.Name)
                                });
                            }
                        }
                    }
                }

                var channelPermissionList = request.AdminPermissionsImpl.GetChannelPermissions(siteId);
                foreach (var channelPermission in channelPermissionList)
                {
                    foreach (var permission in PermissionConfigManager.Instance.ChannelPermissions)
                    {
                        if (permission.Name == channelPermission)
                        {
                            if (channelPermission == ConfigManager.ChannelPermissions.ContentCheck)
                            {
                                if (siteInfo.Additional.IsCheckContentLevel) continue;
                            }
                            else if (channelPermission == ConfigManager.ChannelPermissions.ContentCheckLevel1)
                            {
                                if (siteInfo.Additional.IsCheckContentLevel == false || siteInfo.Additional.CheckContentLevel < 1) continue;
                            }
                            else if (channelPermission == ConfigManager.ChannelPermissions.ContentCheckLevel2)
                            {
                                if (siteInfo.Additional.IsCheckContentLevel == false || siteInfo.Additional.CheckContentLevel < 2) continue;
                            }
                            else if (channelPermission == ConfigManager.ChannelPermissions.ContentCheckLevel3)
                            {
                                if (siteInfo.Additional.IsCheckContentLevel == false || siteInfo.Additional.CheckContentLevel < 3) continue;
                            }
                            else if (channelPermission == ConfigManager.ChannelPermissions.ContentCheckLevel4)
                            {
                                if (siteInfo.Additional.IsCheckContentLevel == false || siteInfo.Additional.CheckContentLevel < 4) continue;
                            }
                            else if (channelPermission == ConfigManager.ChannelPermissions.ContentCheckLevel5)
                            {
                                if (siteInfo.Additional.IsCheckContentLevel == false || siteInfo.Additional.CheckContentLevel < 5) continue;
                            }

                            channelPermissions.Add(new Permission
                            {
                                Name = permission.Name,
                                Text = permission.Text,
                                Selected = StringUtils.In(sitePermissionsInfo.ChannelPermissions, permission.Name)
                            });
                        }
                    }
                }
            }

            var channelInfo = ChannelManager.GetChannelInfo(siteId, siteId);
            channelInfo.Children = ChannelManager.GetChildren(siteId, siteId);
            var checkedChannelIdList = new List<int>();
            foreach (var i in TranslateUtils.StringCollectionToIntList(sitePermissionsInfo.ChannelIdCollection))
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
        public IHttpActionResult InsertRole()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsAdminRole))
                {
                    return Unauthorized();
                }

                var roleName = request.GetPostString("roleName");
                var description = request.GetPostString("description");
                var generalPermissionList = request.GetPostObject<List<string>>("generalPermissions");
                var sitePermissionsInRolesInfoList =
                    request.GetPostObject<List<SitePermissionsInfo>>("sitePermissions");

                if (EPredefinedRoleUtils.IsPredefinedRole(roleName))
                {
                    return BadRequest($"角色添加失败，{roleName}为系统角色！");
                }
                if (DataProvider.RoleDao.IsRoleExists(roleName))
                {
                    return BadRequest("角色名称已存在，请更换角色名称！");
                }

                DataProvider.RoleDao.InsertRole(new RoleInfo
                {
                    RoleName = roleName,
                    CreatorUserName = request.AdminName,
                    Description = description
                });

                if (generalPermissionList != null && generalPermissionList.Count > 0)
                {
                    var permissionsInRolesInfo = new PermissionsInRolesInfo(0, roleName,
                        TranslateUtils.ObjectCollectionToString(generalPermissionList));
                    DataProvider.PermissionsInRolesDao.Insert(permissionsInRolesInfo);
                }

                if (sitePermissionsInRolesInfoList != null && sitePermissionsInRolesInfoList.Count > 0)
                {
                    foreach (var sitePermissionsInfo in sitePermissionsInRolesInfoList)
                    {
                        sitePermissionsInfo.RoleName = roleName;
                        DataProvider.SitePermissionsDao.Insert(sitePermissionsInfo);
                    }
                }

                PermissionsImpl.ClearAllCache();

                request.AddAdminLog("新增管理员角色", $"角色名称:{roleName}");

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
        public IHttpActionResult UpdateRole(int roleId)
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsAdminRole))
                {
                    return Unauthorized();
                }

                var roleName = request.GetPostString("roleName");
                var description = request.GetPostString("description");
                var generalPermissionList = request.GetPostObject<List<string>>("generalPermissions");
                var sitePermissionsInRolesInfoList =
                    request.GetPostObject<List<SitePermissionsInfo>>("sitePermissions");

                var roleInfo = DataProvider.RoleDao.GetRoleInfo(roleId);
                if (roleInfo.RoleName != roleName)
                {
                    if (EPredefinedRoleUtils.IsPredefinedRole(roleName))
                    {
                        return BadRequest($"角色添加失败，{roleName}为系统角色！");
                    }
                    if (DataProvider.RoleDao.IsRoleExists(roleName))
                    {
                        return BadRequest("角色名称已存在，请更换角色名称！");
                    }
                }

                DataProvider.PermissionsInRolesDao.Delete(roleInfo.RoleName);
                DataProvider.SitePermissionsDao.Delete(roleInfo.RoleName);

                if (generalPermissionList != null && generalPermissionList.Count > 0)
                {
                    var permissionsInRolesInfo = new PermissionsInRolesInfo(0, roleName,
                        TranslateUtils.ObjectCollectionToString(generalPermissionList));
                    DataProvider.PermissionsInRolesDao.Insert(permissionsInRolesInfo);
                }

                if (sitePermissionsInRolesInfoList != null && sitePermissionsInRolesInfoList.Count > 0)
                {
                    foreach (var sitePermissionsInfo in sitePermissionsInRolesInfoList)
                    {
                        sitePermissionsInfo.RoleName = roleName;
                        DataProvider.SitePermissionsDao.Insert(sitePermissionsInfo);
                    }
                }

                roleInfo.RoleName = roleName;
                roleInfo.Description = description;

                DataProvider.RoleDao.UpdateRole(roleInfo);

                PermissionsImpl.ClearAllCache();

                request.AddAdminLog("修改管理员角色", $"角色名称:{roleName}");

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
