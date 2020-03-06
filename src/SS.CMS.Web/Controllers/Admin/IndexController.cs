using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Packaging;
using SS.CMS.Web.Controllers.Admin.Settings.Sites;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin
{
    [ApiController]
    [Route(Constants.ApiRoute)]
    public partial class IndexController : ControllerBase
    {
        private const string Route = "index";
        private const string RouteActionsCreate = "index/actions/create";
        private const string RouteActionsCache = "index/actions/cache";
        private const string RouteActionsDownload = "index/actions/download";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly IPathManager _pathManager;
        private readonly IPluginManager _pluginManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IDbCacheRepository _dbCacheRepository;

        public IndexController(ISettingsManager settingsManager, IAuthManager authManager, ICreateManager createManager, IPathManager pathManager, IPluginManager pluginManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IDbCacheRepository dbCacheRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _createManager = createManager;
            _pathManager = pathManager;
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

            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin)
            {
                return Unauthorized();
            }
            var cacheKey = Constants.GetSessionIdCacheKey(auth.AdminId);
            var sessionId = await _dbCacheRepository.GetValueAsync(cacheKey);
            if (string.IsNullOrEmpty(request.SessionId) || sessionId != request.SessionId)
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var adminInfo = auth.Administrator;
            var permissions = auth.AdminPermissions;
            var isSuperAdmin = await permissions.IsSuperAdminAsync();
            var siteIdListWithPermissions = await permissions.GetSiteIdListAsync();

            if (site == null || !siteIdListWithPermissions.Contains(site.Id))
            {
                if (siteIdListWithPermissions.Contains(adminInfo.SiteId))
                {
                    return new GetResult
                    {
                        Value = false,
                        RedirectUrl = $"{_pathManager.GetAdminUrl()}?siteId={adminInfo.SiteId}"
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

                return this.Error("您没有可以管理的站点，请联系超级管理员协助解决");
            }

            var packageIds = new List<string>
            {
                PackageUtils.PackageIdSsCms
            };
            var packageList = new List<object>();
            var dict = await _pluginManager.GetPluginIdAndVersionDictAsync();
            foreach (var id in dict.Keys)
            {
                packageIds.Add(id);
                var version = dict[id];
                packageList.Add(new
                {
                    id,
                    version
                });
            }

            var siteIdListLatestAccessed = await _administratorRepository.UpdateSiteIdAsync(adminInfo, site.Id);

            var permissionList = await permissions.GetPermissionListAsync();
            if (await permissions.HasSitePermissionsAsync(site.Id))
            {
                var websitePermissionList = await permissions.GetSitePermissionsAsync(site.Id);
                if (websitePermissionList != null)
                {
                    permissionList.AddRange(websitePermissionList);
                }
            }
            var channelPermissions = await permissions.GetChannelPermissionsAsync(site.Id);
            if (channelPermissions.Count > 0)
            {
                permissionList.AddRange(channelPermissions);
            }

            var tabManager = new TabManager(_pathManager, _pluginManager);

            var siteMenus =
                await GetLeftMenusAsync(tabManager, site, Constants.TopMenu.IdSite, isSuperAdmin, permissionList);
            var pluginMenus = await GetLeftMenusAsync(tabManager, site, string.Empty, isSuperAdmin, permissionList);
            siteMenus.AddRange(pluginMenus);
            var menus = await GetTopMenusAsync(tabManager, site, isSuperAdmin, siteIdListLatestAccessed, siteIdListWithPermissions, permissionList, siteMenus);

            var config = await _configRepository.GetAsync();

            var siteUrl = await _pathManager.GetSiteUrlAsync(site, false);

            var previewUrl = _pathManager.GetLocalSiteUrl(site.Id);

            return new GetResult
            {
                Value = true,
                DefaultPageUrl = await _pluginManager.GetSystemDefaultPageUrlAsync(request.SiteId) ?? _pathManager.GetAdminUrl(DashboardController.Route),
                IsNightly = _settingsManager.IsNightlyUpdate,
                ProductVersion = _settingsManager.ProductVersion,
                PluginVersion = _settingsManager.PluginVersion,
                TargetFramework = _settingsManager.TargetFramework,
                AdminLogoUrl = config.AdminLogoUrl,
                AdminTitle = config.AdminTitle,
                IsSuperAdmin = isSuperAdmin,
                PackageList = packageList,
                PackageIds = packageIds,
                Menus = menus,
                SiteUrl = siteUrl,
                PreviewUrl = previewUrl,
                Local = new Local
                {
                    UserId = adminInfo.Id,
                    UserName = adminInfo.UserName,
                    AvatarUrl = adminInfo.AvatarUrl,
                    Level = await permissions.GetAdminLevelAsync()
                }
            };
        }
    }
}