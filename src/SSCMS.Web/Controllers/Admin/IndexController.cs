using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SSCMS.Core.Extensions;
using SSCMS.Utils;
using SSCMS.Web.Controllers.Admin.Settings.Sites;
using NSwag.Annotations;
using NuGet.Packaging;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin
{
    [OpenApiIgnore]
    [Route(Constants.ApiAdminPrefix)]
    public partial class IndexController : ControllerBase
    {
        private const string Route = "index";
        private const string RouteActionsCreate = "index/actions/create";
        private const string RouteActionsCache = "index/actions/cache";
        private const string RouteActionsDownload = "index/actions/download";

        private readonly IStringLocalizer<IndexController> _local;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly IPathManager _pathManager;
        private readonly IOldPluginManager _oldPluginManager;
        private readonly IPluginManager _pluginManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IDbCacheRepository _dbCacheRepository;

        public IndexController(IStringLocalizer<IndexController> local, ISettingsManager settingsManager, IAuthManager authManager, ICreateManager createManager, IPathManager pathManager, IOldPluginManager oldPluginManager, IPluginManager pluginManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IDbCacheRepository dbCacheRepository)
        {
            _local = local;
            _settingsManager = settingsManager;
            _authManager = authManager;
            _createManager = createManager;
            _pathManager = pathManager;
            _oldPluginManager = oldPluginManager;
            _pluginManager = pluginManager;
            _configRepository = configRepository;
            _administratorRepository = administratorRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _dbCacheRepository = dbCacheRepository;
        }

        [HttpGet, Route(Route)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
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

                return this.Error(_local["You do not have a site to manage, please contact the super administrator for assistance"]);
            }

            var packageIds = new List<string>
            {
                Constants.PackageId
            };
            var packageList = new List<object>();
            var plugins = _oldPluginManager.GetPlugins();
            foreach (var plugin in plugins)
            {
                packageIds.Add(plugin.PluginId);
                packageList.Add(new
                {
                    Id = plugin.PluginId,
                    plugin.Version
                });
            }

            var allMenus = _settingsManager.GetMenus();

            var siteType = _settingsManager.GetSiteType(site.SiteType);
            var menus = new List<Menu>();
            if (await _authManager.HasSitePermissionsAsync(site.Id))
            {
                var sitePermissions = await _authManager.GetSitePermissionsAsync(site.Id);
                var siteMenu = new Menu
                {
                    Id = IdSite,
                    Text = site.SiteName,
                    Type = siteType.Id,
                    Children = new List<Menu>(allMenus.Where(x => StringUtils.EqualsIgnoreCase(x.Type, siteType.Id)))
                };

                foreach (var plugin in _pluginManager.Plugins.Where(x => x.Menus != null))
                {
                    var pluginMenus = plugin.Menus.Where(x => StringUtils.EqualsIgnoreCase(x.Type, siteType.Id));
                    siteMenu.Children.AddRange(pluginMenus);
                }

                var query = new NameValueCollection {{"siteId", site.Id.ToString()}};

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
                    var siteIdList = await _siteRepository.GetLatestSiteIdListAsync(siteIdListLatestAccessed, siteIdListWithPermissions);
                    foreach (var siteId in siteIdList)
                    {
                        var theSite = await _siteRepository.GetAsync(siteId);
                        if (theSite == null) continue;

                        allSiteMenus.Add(new Menu
                        {
                            Link = $"{_pathManager.GetAdminUrl()}?siteId={theSite.Id}",
                            Target = "_top",
                            Text = theSite.SiteName
                        });
                    }

                    switchMenus.Add(new Menu
                    {
                        IconClass = "ion-edit",
                        Link = _pathManager.GetAdminUrl(SitesLayerSelectController.Route),
                        Target = "_layer",
                        Text = _local["Select site"]
                    });
                    switchMenus.Add(new Menu
                    {
                        IconClass = "ion-earth",
                        Text = _local["Recently site"],
                        Selected = true,
                        Children = allSiteMenus.ToArray()
                    });

                    menus.Add(new Menu
                    {
                        Text = _local["Switch site"],
                        Children = switchMenus.ToArray()
                    });
                }
            }

            var appPermissions = await _authManager.GetAppPermissionsAsync();
            var appMenus = allMenus.Where(x => StringUtils.EqualsIgnoreCase(x.Type, AuthTypes.Resources.App) && _authManager.IsMenuValid(x, appPermissions)).ToList();
            foreach (var appMenu in appMenus)
            {
                appMenu.Children = GetChildren(appMenu, appPermissions);
            }
            menus.AddRange(appMenus);

            var config = await _configRepository.GetAsync();
            var siteUrl = await _pathManager.GetSiteUrlAsync(site, false);
            var previewUrl = _pathManager.GetLocalSiteUrl(site.Id);

            return new GetResult
            {
                Value = true,
                DefaultPageUrl = await _oldPluginManager.GetSystemDefaultPageUrlAsync(request.SiteId) ?? _pathManager.GetAdminUrl(DashboardController.Route),
                IsNightly = _settingsManager.IsNightlyUpdate,
                Version = _settingsManager.Version,
                TargetFramework = _settingsManager.TargetFramework,
                AdminLogoUrl = config.AdminLogoUrl,
                AdminTitle = config.AdminTitle,
                IsSuperAdmin = isSuperAdmin,
                PackageList = packageList,
                PackageIds = packageIds,
                Menus = menus,
                SiteType = siteType.Id,
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