using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsRoleAddController
    {
        [HttpPost, Route(RouteSitePermission)]
        public async Task<ActionResult<SitePermissionResult>> SitePermission([FromBody] SitePermissionRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            var allPermissions = _settingsManager.GetPermissions();

            SitePermissions sitePermissionsInfo = null;
            if (request.RoleId > 0)
            {
                var roleInfo = await _roleRepository.GetRoleAsync(request.RoleId);
                sitePermissionsInfo = await _sitePermissionsRepository.GetAsync(roleInfo.RoleName, request.SiteId);
            }
            if (sitePermissionsInfo == null) sitePermissionsInfo = new SitePermissions();

            var sitePermissions = new List<Option>();
            var channelPermissions = new List<Option>();
            var contentPermissions = new List<Option>();

            if (await _authManager.IsSuperAdminAsync())
            {
                foreach (var permission in allPermissions.Where(x => ListUtils.ContainsIgnoreCase(x.Type, site.SiteType)))
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

                var channelPermissionList = allPermissions.Where(x => ListUtils.ContainsIgnoreCase(x.Type, Types.Resources.Channel));
                foreach (var permission in channelPermissionList)
                {
                    channelPermissions.Add(new Option
                    {
                        Name = permission.Id,
                        Text = permission.Text,
                        Selected = ListUtils.ContainsIgnoreCase(sitePermissionsInfo.ChannelPermissions, permission.Id)
                    });
                }

                var contentPermissionList = allPermissions.Where(x => ListUtils.ContainsIgnoreCase(x.Type, Types.Resources.Content));
                foreach (var permission in contentPermissionList)
                {
                    if (permission.Id == MenuUtils.ContentPermissions.CheckLevel1)
                    {
                        if (site.CheckContentLevel < 1)
                        {
                            continue;
                        }
                    }
                    else if (permission.Id == MenuUtils.ContentPermissions.CheckLevel2)
                    {
                        if (site.CheckContentLevel < 2)
                        {
                            continue;
                        }
                    }
                    else if (permission.Id == MenuUtils.ContentPermissions.CheckLevel3)
                    {
                        if (site.CheckContentLevel < 3)
                        {
                            continue;
                        }
                    }
                    else if (permission.Id == MenuUtils.ContentPermissions.CheckLevel4)
                    {
                        if (site.CheckContentLevel < 4)
                        {
                            continue;
                        }
                    }
                    else if (permission.Id == MenuUtils.ContentPermissions.CheckLevel5)
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
                if (await _authManager.HasSitePermissionsAsync(request.SiteId))
                {
                    var websitePermissionList = await _authManager.GetSitePermissionsAsync(request.SiteId);
                    foreach (var websitePermission in websitePermissionList)
                    {
                        foreach (var permission in allPermissions.Where(x => ListUtils.ContainsIgnoreCase(x.Type, site.SiteType)))
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

                var channelPermissionList = await _authManager.GetChannelPermissionsAsync(request.SiteId);
                foreach (var channelPermission in channelPermissionList)
                {
                    foreach (var permission in allPermissions.Where(x => ListUtils.ContainsIgnoreCase(x.Type, Types.Resources.Channel)))
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

                var contentPermissionList = await _authManager.GetContentPermissionsAsync(request.SiteId);
                foreach (var contentPermission in contentPermissionList)
                {
                    foreach (var permission in allPermissions.Where(x => ListUtils.ContainsIgnoreCase(x.Type, Types.Resources.Content)))
                    {
                        if (permission.Id == contentPermission)
                        {
                            if (contentPermission == MenuUtils.ContentPermissions.CheckLevel1)
                            {
                                if (site.CheckContentLevel < 1) continue;
                            }
                            else if (contentPermission == MenuUtils.ContentPermissions.CheckLevel2)
                            {
                                if (site.CheckContentLevel < 2) continue;
                            }
                            else if (contentPermission == MenuUtils.ContentPermissions.CheckLevel3)
                            {
                                if (site.CheckContentLevel < 3) continue;
                            }
                            else if (contentPermission == MenuUtils.ContentPermissions.CheckLevel4)
                            {
                                if (site.CheckContentLevel < 4) continue;
                            }
                            else if (contentPermission == MenuUtils.ContentPermissions.CheckLevel5)
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

            var channelInfo = await _channelRepository.GetAsync(request.SiteId);
            channelInfo.Children = await _channelRepository.GetChildrenAsync(request.SiteId, request.SiteId);
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

            return new SitePermissionResult
            {
                Site = site,
                SitePermissions = sitePermissions,
                ChannelPermissions = channelPermissions,
                ContentPermissions = contentPermissions,
                Channel = channelInfo,
                CheckedChannelIds = checkedChannelIdList
            };
        }
    }
}