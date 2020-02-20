using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.CMS.Api.Preview;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Packaging;
using SiteServer.CMS.Plugin;
using SiteServer.Web.Core;

namespace SiteServer.Web.Controllers.Pages
{
    [Route("pages/index")]
    public partial class PagesIndexController : ControllerBase
    {
        private const string Route = "";
        private const string RouteActionsCreate = "actions/create";
        private const string RouteActionsCache = "actions/cache";
        private const string RouteActionsDownload = "actions/download";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IDbCacheRepository _dbCacheRepository;

        public PagesIndexController(IAuthManager authManager, ICreateManager createManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IDbCacheRepository dbCacheRepository)
        {
            _authManager = authManager;
            _createManager = createManager;
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
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            var redirectUrl = await auth.AdminRedirectCheckAsync(checkInstall: true, checkDatabaseVersion: true, checkLogin: true);
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                return new GetResult
                {
                    Value = false,
                    RedirectUrl = redirectUrl
                };
            }

            if (!auth.IsAdminLoggin)
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
                        RedirectUrl = PageUtils.GetMainUrl(adminInfo.SiteId)
                    };
                }

                if (siteIdListWithPermissions.Count > 0)
                {
                    return new GetResult
                    {
                        Value = false,
                        RedirectUrl = PageUtils.GetMainUrl(siteIdListWithPermissions[0])
                    };
                }

                if (isSuperAdmin)
                {
                    return new GetResult
                    {
                        Value = false,
                        RedirectUrl = PageUtils.GetSettingsUrl("siteAdd")
                    };
                }

                return this.Error("您没有可以管理的站点，请联系超级管理员协助解决");
            }

            var packageIds = new List<string>
            {
                PackageUtils.PackageIdSsCms
            };
            var packageList = new List<object>();
            var dict = await PluginManager.GetPluginIdAndVersionDictAsync();
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

            var siteMenus =
                await GetLeftMenusAsync(site, Constants.TopMenu.IdSite, isSuperAdmin, permissionList);
            var pluginMenus = await GetLeftMenusAsync(site, string.Empty, isSuperAdmin, permissionList);
            siteMenus.AddRange(pluginMenus);
            var menus = await GetTopMenusAsync(site, isSuperAdmin, siteIdListLatestAccessed, siteIdListWithPermissions, permissionList, siteMenus);

            var config = await _configRepository.GetAsync();

            var siteUrl = await PageUtility.GetSiteUrlAsync(site, false);
            var previewUrl = ApiRoutePreview.GetSiteUrl(site.Id);

            return new GetResult
            {
                Value = true,
                DefaultPageUrl = await PluginMenuManager.GetSystemDefaultPageUrlAsync(request.SiteId) ?? "dashboard.cshtml",
                IsNightly = WebConfigUtils.IsNightlyUpdate,
                ProductVersion = SystemManager.ProductVersion,
                PluginVersion = SystemManager.PluginVersion,
                TargetFramework = SystemManager.TargetFramework,
                EnvironmentVersion = SystemManager.EnvironmentVersion,
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

        [HttpPost, Route(RouteActionsCreate)]
        public async Task<ActionResult<IntResult>> Create([FromBody] CreateRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin || auth.Administrator == null)
            {
                return Unauthorized();
            }

            //#if !DEBUG
            //todo: Change to CacheManager
            var cacheKey = Constants.GetSessionIdCacheKey(auth.AdminId);
            if (string.IsNullOrEmpty(request.SessionId) || auth.SessionId != request.SessionId)
            {
                return Unauthorized();
            }
            //#endif

            var config = await _configRepository.GetAsync();

            if (auth.Administrator.LastActivityDate != null && config.IsAdminEnforceLogout)
            {
                var ts = new TimeSpan(DateTime.Now.Ticks - auth.Administrator.LastActivityDate.Value.Ticks);
                if (ts.TotalMinutes > config.AdminEnforceLogoutMinutes)
                {
                    return Unauthorized();
                }
            }

            var count = _createManager.PendingTaskCount;

            var pendingTask = _createManager.GetFirstPendingTask();
            if (pendingTask != null)
            {
                try
                {
                    var start = DateTime.Now;
                    await _createManager.ExecuteAsync(pendingTask.SiteId, pendingTask.CreateType,
                        pendingTask.ChannelId,
                        pendingTask.ContentId, pendingTask.FileTemplateId, pendingTask.SpecialId);
                    var timeSpan = DateUtils.GetRelatedDateTimeString(start);
                    _createManager.AddSuccessLog(pendingTask, timeSpan);
                }
                catch (Exception ex)
                {
                    _createManager.AddFailureLog(pendingTask, ex);
                }
                finally
                {
                    _createManager.RemovePendingTask(pendingTask);
                }
            }

            return new IntResult
            {
                Value = count
            };
        }

        [HttpPost, Route(RouteActionsCache)]
        public async Task<ActionResult<IntResult>> Cache([FromBody] SiteRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin || auth.Administrator == null)
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            await _channelRepository.CacheAllAsync(site);
            var channelSummaries = await _channelRepository.GetSummaryAsync(site.Id);
            await _contentRepository.CacheAllListAndCountAsync(site, channelSummaries);
            await _contentRepository.CacheAllEntityAsync(site, channelSummaries);

            return new IntResult
            {
                Value = channelSummaries.Count
            };
        }

        [HttpPost, Route(RouteActionsDownload)]
        public async Task<ActionResult<BoolResult>> Download([FromBody] DownloadRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin)
            {
                return Unauthorized();
            }

            try
            {
                PackageUtils.DownloadPackage(request.PackageId, request.Version);
            }
            catch
            {
                PackageUtils.DownloadPackage(request.PackageId, request.Version);
            }

            var isDownload = PackageUtils.IsPackageDownload(request.PackageId, request.Version);
            if (isDownload)
            {
                if (StringUtils.EqualsIgnoreCase(request.PackageId, PackageUtils.PackageIdSsCms))
                {
                    await _dbCacheRepository.RemoveAndInsertAsync(PackageUtils.CacheKeySsCmsIsDownload, true.ToString());
                }
            }

            return new BoolResult
            {
                Value = isDownload
            };
        }
    }
}