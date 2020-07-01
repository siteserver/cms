using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsRoleAddController
    {
        public class Option
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
            public List<Option> Permissions { get; set; }
            public List<Site> Sites { get; set; }
            public List<int> CheckedSiteIds { get; set; }
            public List<SitePermissionsResult> SitePermissionsList { get; set; }
        }

        public class SitePermissionsResult
        {
            public int SiteId { get; set; }
            public List<Option> SitePermissions { get; set; }
            public List<Option> ChannelPermissions { get; set; }
            public List<Option> ContentPermissions { get; set; }
            public Channel Channel { get; set; }
            public List<int> CheckedChannelIds { get; set; }
        }

        public class RoleRequest
        {
            public string RoleName { get; set; }
            public string Description { get; set; }
            public List<string> AppPermissions { get; set; }
            public List<SitePermissions> SitePermissions { get; set; }
        }

        private async Task<SitePermissionsResult> GetSitePermissionsObjectAsync(List<Permission> allPermissions, int roleId, int siteId)
        {
            SitePermissions sitePermissionsInfo = null;
            if (roleId > 0)
            {
                var roleInfo = await _roleRepository.GetRoleAsync(roleId);
                sitePermissionsInfo = await _sitePermissionsRepository.GetAsync(roleInfo.RoleName, siteId);
            }
            if (sitePermissionsInfo == null) sitePermissionsInfo = new SitePermissions();

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return null;

            var sitePermissions = new List<Option>();
            var channelPermissions = new List<Option>();
            var contentPermissions = new List<Option>();

            if (await _authManager.IsSuperAdminAsync())
            {
                foreach (var permission in allPermissions.Where(x => StringUtils.EqualsIgnoreCase(x.Type, site.SiteType)))
                {
                    sitePermissions.Add(new Option
                    {
                        Name = permission.Id,
                        Text = permission.Text,
                        Selected = ListUtils.ContainsIgnoreCase(sitePermissionsInfo.Permissions, permission.Id)
                    });
                }

                //foreach (var permission in permissions.WebsitePluginPermissions)
                //{
                //    pluginPermissions.Add(new Permission
                //    {
                //        Name = permission.Name,
                //        Text = permission.Text,
                //        Selected = StringUtils.ContainsIgnoreCase(sitePermissionsInfo.WebsitePermissions, permission.Name)
                //    });
                //}

                var channelPermissionList = allPermissions.Where(x => StringUtils.EqualsIgnoreCase(x.Type, AuthTypes.Resources.Channel));
                foreach (var permission in channelPermissionList)
                {
                    channelPermissions.Add(new Option
                    {
                        Name = permission.Id,
                        Text = permission.Text,
                        Selected = ListUtils.ContainsIgnoreCase(sitePermissionsInfo.ChannelPermissions, permission.Id)
                    });
                }

                var contentPermissionList = allPermissions.Where(x => StringUtils.EqualsIgnoreCase(x.Type, AuthTypes.Resources.Content));
                foreach (var permission in contentPermissionList)
                {
                    if (permission.Id == AuthTypes.ContentPermissions.CheckLevel1)
                    {
                        if (site.CheckContentLevel < 1)
                        {
                            continue;
                        }
                    }
                    else if (permission.Id == AuthTypes.ContentPermissions.CheckLevel2)
                    {
                        if (site.CheckContentLevel < 2)
                        {
                            continue;
                        }
                    }
                    else if (permission.Id == AuthTypes.ContentPermissions.CheckLevel3)
                    {
                        if (site.CheckContentLevel < 3)
                        {
                            continue;
                        }
                    }
                    else if (permission.Id == AuthTypes.ContentPermissions.CheckLevel4)
                    {
                        if (site.CheckContentLevel < 4)
                        {
                            continue;
                        }
                    }
                    else if (permission.Id == AuthTypes.ContentPermissions.CheckLevel5)
                    {
                        if (site.CheckContentLevel < 5)
                        {
                            continue;
                        }
                    }

                    contentPermissions.Add(new Option
                    {
                        Name = permission.Id,
                        Text = permission.Text,
                        Selected = ListUtils.ContainsIgnoreCase(sitePermissionsInfo.ContentPermissions, permission.Id)
                    });
                }
            }
            else
            {
                if (await _authManager.HasSitePermissionsAsync(siteId))
                {
                    var websitePermissionList = await _authManager.GetSitePermissionsAsync(siteId);
                    foreach (var websitePermission in websitePermissionList)
                    {
                        foreach (var permission in allPermissions.Where(x => StringUtils.EqualsIgnoreCase(x.Type, site.SiteType)))
                        {
                            if (permission.Id == websitePermission)
                            {
                                sitePermissions.Add(new Option
                                {
                                    Name = permission.Id,
                                    Text = permission.Text,
                                    Selected = ListUtils.ContainsIgnoreCase(sitePermissionsInfo.Permissions, permission.Id)
                                });
                            }
                        }

                        //foreach (var permission in instance.WebsitePluginPermissions)
                        //{
                        //    if (permission.Name == websitePermission)
                        //    {
                        //        pluginPermissions.Add(new Permission
                        //        {
                        //            Name = permission.Name,
                        //            Text = permission.Text,
                        //            Selected = StringUtils.ContainsIgnoreCase(sitePermissionsInfo.WebsitePermissions, permission.Name)
                        //        });
                        //    }
                        //}
                    }
                }

                var channelPermissionList = await _authManager.GetChannelPermissionsAsync(siteId);
                foreach (var channelPermission in channelPermissionList)
                {
                    foreach (var permission in allPermissions.Where(x => StringUtils.EqualsIgnoreCase(x.Type, AuthTypes.Resources.Channel)))
                    {
                        if (permission.Id == channelPermission)
                        {
                            channelPermissions.Add(new Option
                            {
                                Name = permission.Id,
                                Text = permission.Text,
                                Selected = ListUtils.ContainsIgnoreCase(sitePermissionsInfo.ChannelPermissions, permission.Id)
                            });
                        }
                    }
                }

                var contentPermissionList = await _authManager.GetContentPermissionsAsync(siteId);
                foreach (var contentPermission in contentPermissionList)
                {
                    foreach (var permission in allPermissions.Where(x => StringUtils.EqualsIgnoreCase(x.Type, AuthTypes.Resources.Content)))
                    {
                        if (permission.Id == contentPermission)
                        {
                            if (contentPermission == AuthTypes.ContentPermissions.CheckLevel1)
                            {
                                if (site.CheckContentLevel < 1) continue;
                            }
                            else if (contentPermission == AuthTypes.ContentPermissions.CheckLevel2)
                            {
                                if (site.CheckContentLevel < 2) continue;
                            }
                            else if (contentPermission == AuthTypes.ContentPermissions.CheckLevel3)
                            {
                                if (site.CheckContentLevel < 3) continue;
                            }
                            else if (contentPermission == AuthTypes.ContentPermissions.CheckLevel4)
                            {
                                if (site.CheckContentLevel < 4) continue;
                            }
                            else if (contentPermission == AuthTypes.ContentPermissions.CheckLevel5)
                            {
                                if (site.CheckContentLevel < 5) continue;
                            }

                            contentPermissions.Add(new Option
                            {
                                Name = permission.Id,
                                Text = permission.Text,
                                Selected = ListUtils.ContainsIgnoreCase(sitePermissionsInfo.ContentPermissions, permission.Id)
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
                ChannelPermissions = channelPermissions,
                ContentPermissions = contentPermissions,
                Channel = channelInfo,
                CheckedChannelIds = checkedChannelIdList
            };
        }
    }
}