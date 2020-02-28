using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsRoleAddController
    {
        public class Permission
        {
            public string Name { get; set; }

            public string Text { get; set; }

            public bool Selected { get; set; }
        }

        public class GetRequest
        {
            public int RoleId { get; set; }
        }

        public class GetResult
        {
            public Role Role { get; set; }
            public List<Permission> Permissions { get; set; }
            public List<Site> Sites { get; set; }
            public List<int> CheckedSiteIds { get; set; }
            public List<object> SitePermissionsList { get; set; }
        }

        public class SitePermissionsResult
        {
            public int SiteId { get; set; }
            public List<Permission> SitePermissions { get; set; }
            public List<Permission> PluginPermissions { get; set; }
            public List<Permission> ChannelPermissions { get; set; }
            public Channel Channel { get; set; }
            public List<int> CheckedChannelIds { get; set; }
        }

        public class RoleRequest
        {
            public string RoleName { get; set; }
            public string Description { get; set; }
            public List<string> GeneralPermissions { get; set; }
            public List<SitePermissions> SitePermissions { get; set; }
        }

        private async Task<SitePermissionsResult> GetSitePermissionsObjectAsync(int roleId, int siteId)
        {
            SitePermissions sitePermissionsInfo = null;
            if (roleId > 0)
            {
                var roleInfo = await _roleRepository.GetRoleAsync(roleId);
                sitePermissionsInfo = await _sitePermissionsRepository.GetSystemPermissionsAsync(roleInfo.RoleName, siteId);
            }
            if (sitePermissionsInfo == null) sitePermissionsInfo = new SitePermissions();

            var site = await _siteRepository.GetAsync(siteId);
            var sitePermissions = new List<Permission>();
            var pluginPermissions = new List<Permission>();
            var channelPermissions = new List<Permission>();
            var instance = await PermissionConfigManager.GetInstanceAsync(_pathManager);

            if (await _authManager.AdminPermissions.IsSuperAdminAsync())
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
                if (await _authManager.AdminPermissions.HasSitePermissionsAsync(siteId))
                {
                    var websitePermissionList = await _authManager.AdminPermissions.GetSitePermissionsAsync(siteId);
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

                var channelPermissionList = await _authManager.AdminPermissions.GetChannelPermissionsAsync(siteId);
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
                                if (site.CheckContentLevel < 3) continue;
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

            var channelInfo = await _channelRepository.GetAsync(siteId);
            channelInfo.Children = await _channelRepository.GetChildrenAsync(siteId, siteId);
            var checkedChannelIdList = new List<int>();
            if (sitePermissionsInfo.ChannelIds != null)
            {
                foreach (var i in sitePermissionsInfo.ChannelIds)
                {
                    if (!checkedChannelIdList.Contains(i))
                    {
                        checkedChannelIdList.Add(i);
                    }
                }
            }

            return new SitePermissionsResult
            {
                SiteId = siteId,
                SitePermissions = sitePermissions,
                PluginPermissions = pluginPermissions,
                ChannelPermissions = channelPermissions,
                Channel = channelInfo,
                CheckedChannelIds = checkedChannelIdList
            };
        }
    }
}