using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
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
                    if (permission.Id == Types.ContentPermissions.CheckLevel1)
                    {
                        if (site.CheckContentLevel < 1)
                        {
                            continue;
                        }
                    }
                    else if (permission.Id == Types.ContentPermissions.CheckLevel2)
                    {
                        if (site.CheckContentLevel < 2)
                        {
                            continue;
                        }
                    }
                    else if (permission.Id == Types.ContentPermissions.CheckLevel3)
                    {
                        if (site.CheckContentLevel < 3)
                        {
                            continue;
                        }
                    }
                    else if (permission.Id == Types.ContentPermissions.CheckLevel4)
                    {
                        if (site.CheckContentLevel < 4)
                        {
                            continue;
                        }
                    }
                    else if (permission.Id == Types.ContentPermissions.CheckLevel5)
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

                var channelPermissionList = await _authManager.GetChannelPermissionsAsync(siteId);
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

                var contentPermissionList = await _authManager.GetContentPermissionsAsync(siteId);
                foreach (var contentPermission in contentPermissionList)
                {
                    foreach (var permission in allPermissions.Where(x => ListUtils.ContainsIgnoreCase(x.Type, Types.Resources.Content)))
                    {
                        if (permission.Id == contentPermission)
                        {
                            if (contentPermission == Types.ContentPermissions.CheckLevel1)
                            {
                                if (site.CheckContentLevel < 1) continue;
                            }
                            else if (contentPermission == Types.ContentPermissions.CheckLevel2)
                            {
                                if (site.CheckContentLevel < 2) continue;
                            }
                            else if (contentPermission == Types.ContentPermissions.CheckLevel3)
                            {
                                if (site.CheckContentLevel < 3) continue;
                            }
                            else if (contentPermission == Types.ContentPermissions.CheckLevel4)
                            {
                                if (site.CheckContentLevel < 4) continue;
                            }
                            else if (contentPermission == Types.ContentPermissions.CheckLevel5)
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
