using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Web.Controllers.Admin.Settings.Sites;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class IndexController
    {
        [HttpGet, Route(Route)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var allowed = true;
            if (!string.IsNullOrEmpty(_settingsManager.AdminRestrictionHost))
            {
                var currentHost = PageUtils.RemoveProtocolFromUrl(PageUtils.GetHost(Request));
                if (!StringUtils.StartsWithIgnoreCase(currentHost, PageUtils.RemoveProtocolFromUrl(_settingsManager.AdminRestrictionHost)))
                {
                    allowed = false;
                }
            }

            if (!allowed)
            {
                var ipAddress = PageUtils.GetIpAddress(Request);
                allowed = PageUtils.IsAllowed(ipAddress,
                    new List<string>(_settingsManager.AdminRestrictionBlockList),
                    new List<string>(_settingsManager.AdminRestrictionAllowList));
            }

            if (!allowed)
            {
                return this.Error($"访问已被禁止，IP地址：{PageUtils.GetIpAddress(Request)}，请与网站管理员联系开通访问权限");
            }

            var (redirect, redirectUrl) = await AdminRedirectCheckAsync();
            if (redirect)
            {
                return new GetResult
                {
                    Value = false,
                    RedirectUrl = redirectUrl
                };
            }

            if (!_authManager.IsAdmin)
            {
                return Unauthorized();
            }

            var admin = await _authManager.GetAdminAsync();
            var cacheKey = Constants.GetSessionIdCacheKey(admin.Id);
            var sessionId = await _dbCacheRepository.GetValueAsync(cacheKey);
            if (string.IsNullOrEmpty(request.SessionId) || sessionId != request.SessionId)
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var isSuperAdmin = await _authManager.IsSuperAdminAsync();
            var siteIdListWithPermissions = await _authManager.GetSiteIdsAsync();

            if (site == null || !siteIdListWithPermissions.Contains(site.Id))
            {
                if (siteIdListWithPermissions.Contains(admin.SiteId))
                {
                    return new GetResult
                    {
                        Value = false,
                        RedirectUrl = $"{_pathManager.GetAdminUrl()}?siteId={admin.SiteId}"
                    };
                }

                if (siteIdListWithPermissions.Count > 0)
                {
                    return new GetResult
                    {
                        Value = false,
                        RedirectUrl = $"{_pathManager.GetAdminUrl()}?siteId={siteIdListWithPermissions[0]}"
                    };
                }

                if (isSuperAdmin)
                {
                    return new GetResult
                    {
                        Value = false,
                        RedirectUrl = _pathManager.GetAdminUrl(SitesAddController.Route)
                    };
                }

                //return this.Error(_local["You do not have a site to manage, please contact the super administrator for assistance"]);
            }

            var enabledPlugins = _pluginManager.EnabledPlugins;
            var allMenus = _settingsManager.GetMenus();

            var menus = new List<Menu>();
            var siteType = new SiteType();
            var siteUrl = string.Empty;
            var previewUrl = string.Empty;
            if (site != null)
            {
                siteType = _settingsManager.GetSiteType(site.SiteType);
                if (await _authManager.HasSitePermissionsAsync(site.Id))
                {
                    var sitePlugins = _pluginManager.GetPlugins(site.Id);
                    var allPluginMenus = new List<Menu>();
                    var sitePluginMenus = new List<Menu>();
                    foreach (var enabledPlugin in enabledPlugins)
                    {
                        var pluginMenus = enabledPlugin.GetMenus()
                            .Where(x => ListUtils.ContainsIgnoreCase(x.Type, siteType.Id)).ToList();
                        if (pluginMenus.Count == 0) continue;

                        allPluginMenus.AddRange(pluginMenus);
                        if (sitePlugins.Exists(x => x.PluginId == enabledPlugin.PluginId))
                        {
                            sitePluginMenus.AddRange(pluginMenus);
                        }
                    }

                    var siteMenus = allMenus
                        .Where(menu => ListUtils.ContainsIgnoreCase(menu.Type, siteType.Id))
                        .Where(menu => !allPluginMenus.Exists(x => x.Id == menu.Id))
                        .ToList();
                    siteMenus.AddRange(sitePluginMenus);

                    var siteMenu = new Menu
                    {
                        Id = IdSite,
                        Text = site.SiteName,
                        Type = new List<string>
                        {
                            siteType.Id
                        },
                        Children = siteMenus
                    };

                    var sitePermissions = await _authManager.GetSitePermissionsAsync(site.Id);
                    var query = new NameValueCollection { { "siteId", site.Id.ToString() } };
                    siteMenu.Children = GetChildren(siteMenu, sitePermissions, x =>
                    {
                        x.Link = PageUtils.AddQueryStringIfNotExists(x.Link, query);
                        return x;
                    });
                    menus.Add(siteMenu);

                    if (siteIdListWithPermissions.Count > 1)
                    {
                        var switchMenus = new List<Menu>();
                        var allSiteMenus = new List<Menu>();
                        var siteIdListLatestAccessed = await _administratorRepository.UpdateSiteIdAsync(admin, site.Id);
                        var siteIdList = await _siteRepository.GetLatestSiteIdsAsync(siteIdListLatestAccessed, siteIdListWithPermissions);
                        foreach (var siteId in siteIdList)
                        {
                            var theSite = await _siteRepository.GetAsync(siteId);
                            if (theSite == null) continue;

                            var theSiteType = _settingsManager.GetSiteType(theSite.SiteType);
                            allSiteMenus.Add(new Menu
                            {
                                Id = $"site_switch_{theSite.Id}",
                                IconClass = theSiteType.IconClass,
                                Link = $"{_pathManager.GetAdminUrl()}?siteId={theSite.Id}",
                                Target = "_top",
                                Text = theSite.SiteName
                            });
                        }

                        switchMenus.Add(new Menu
                        {
                            Id = "site_switch_all",
                            IconClass = "ion-clock",
                            //Text = _local["Recently site"],
                            Text = "最近访问",
                            Children = allSiteMenus.ToArray()
                        });
                        switchMenus.Add(new Menu
                        {
                            Id = "site_switch_select",
                            IconClass = "ion-checkmark",
                            Link = _pathManager.GetAdminUrl(SitesLayerSelectController.Route),
                            Target = "_layer",
                            //Text = _local["Select site"]
                            Text = "选择站点"
                        });

                        menus.Add(new Menu
                        {
                            Id = "site_switch",
                            //Text = _local["Switch site"],
                            Text = "切换站点",
                            Children = switchMenus.ToArray()
                        });
                    }
                }
                siteUrl = await _pathManager.GetSiteUrlAsync(site, false);
                previewUrl = _pathManager.GetPreviewSiteUrl(site.Id);
            }

            var appPermissions = await _authManager.GetAppPermissionsAsync();
            var appMenus = allMenus.Where(x => ListUtils.ContainsIgnoreCase(x.Type, Types.Resources.App) && _authManager.IsMenuValid(x, appPermissions)).ToList();
            foreach (var appMenu in appMenus)
            {
                appMenu.Children = GetChildren(appMenu, appPermissions);
            }
            menus.AddRange(appMenus);

            var config = await _configRepository.GetAsync();

            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            var culture = requestCulture.RequestCulture.UICulture.Name;
            var plugins = enabledPlugins.Select(plugin => new GetPlugin { PluginId = plugin.PluginId, DisplayName = plugin.DisplayName, Version = plugin.Version }).ToList();

            return new GetResult
            {
                Value = true,
                Version = _settingsManager.Version,
                OSArchitecture = _settingsManager.OSArchitecture,
                AdminLogoUrl = config.AdminLogoUrl,
                AdminTitle = config.AdminTitle,
                IsSuperAdmin = isSuperAdmin,
                Culture = culture,
                Plugins = plugins,
                Menus = menus,
                SiteType = siteType,
                SiteUrl = siteUrl,
                PreviewUrl = previewUrl,
                Local = new Local
                {
                    UserId = admin.Id,
                    UserName = admin.UserName,
                    AvatarUrl = admin.AvatarUrl,
                    Level = await _authManager.GetAdminLevelAsync()
                }
            };
        }
    }
}
