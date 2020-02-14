using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Api.Preview;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Packaging;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.StlParser;

namespace SiteServer.API.Controllers.Pages
{
    [RoutePrefix("pages/main")]
    public partial class PagesMainController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsCreate = "actions/create";
        private const string RouteActionsCache = "actions/cache";
        private const string RouteActionsDownload = "actions/download";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            var redirectUrl = await auth.AdminRedirectCheckAsync(checkInstall: true, checkDatabaseVersion: true, checkLogin: true);
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                return new GetResult
                {
                    Value = false,
                    RedirectUrl = redirectUrl
                };
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            var adminInfo = auth.Administrator;
            var permissions = auth.AdminPermissionsImpl;
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

                return new GetResult
                {
                    Value = false,
                    RedirectUrl = $"error.html?message={HttpUtility.UrlEncode("您没有可以管理的站点，请联系超级管理员协助解决")}"
                };
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

            var siteIdListLatestAccessed = await DataProvider.AdministratorRepository.UpdateSiteIdAsync(adminInfo, site.Id);

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

            var config = await DataProvider.ConfigRepository.GetAsync();

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
        public async Task<IntResult> Create([FromBody] CreateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin || auth.Administrator == null)
            {
                return Request.Unauthorized<IntResult>();
            }

#if !DEBUG
            var cacheKey = Constants.GetSessionIdCacheKey(auth.AdminId);
            if (string.IsNullOrEmpty(request.SessionId) || CacheUtils.GetString(cacheKey) != request.SessionId)
            {
                return Request.Unauthorized<IntResult>();
            }
#endif

            var config = await DataProvider.ConfigRepository.GetAsync();

            if (auth.Administrator.LastActivityDate != null && config.IsAdminEnforceLogout)
            {
                var ts = new TimeSpan(DateTime.Now.Ticks - auth.Administrator.LastActivityDate.Value.Ticks);
                if (ts.TotalMinutes > config.AdminEnforceLogoutMinutes)
                {
                    return Request.Unauthorized<IntResult>();
                }
            }

            var count = CreateTaskManager.PendingTaskCount;

            var pendingTask = CreateTaskManager.GetFirstPendingTask();
            if (pendingTask != null)
            {
                try
                {
                    var start = DateTime.Now;
                    await FileSystemObjectAsync.ExecuteAsync(pendingTask.SiteId, pendingTask.CreateType,
                        pendingTask.ChannelId,
                        pendingTask.ContentId, pendingTask.FileTemplateId, pendingTask.SpecialId);
                    var timeSpan = DateUtils.GetRelatedDateTimeString(start);
                    CreateTaskManager.AddSuccessLog(pendingTask, timeSpan);
                }
                catch (Exception ex)
                {
                    CreateTaskManager.AddFailureLog(pendingTask, ex);
                }
                finally
                {
                    CreateTaskManager.RemovePendingTask(pendingTask);
                }
            }

            return new IntResult
            {
                Value = count
            };
        }

        [HttpPost, Route(RouteActionsCache)]
        public async Task<IntResult> Cache([FromBody] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin || auth.Administrator == null)
            {
                return Request.Unauthorized<IntResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            await DataProvider.ChannelRepository.CacheAllAsync(site);
            var channelSummaries = await DataProvider.ChannelRepository.GetAllSummaryAsync(site.Id);
            await DataProvider.ContentRepository.CacheAllListAndCountAsync(site, channelSummaries);
            await DataProvider.ContentRepository.CacheAllEntityAsync(site, channelSummaries);

            return new IntResult
            {
                Value = channelSummaries.Count
            };
        }

        [HttpPost, Route(RouteActionsDownload)]
        public async Task<BoolResult> Download([FromBody] DownloadRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin)
            {
                return Request.Unauthorized<BoolResult>();
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
                    await DataProvider.DbCacheRepository.RemoveAndInsertAsync(PackageUtils.CacheKeySsCmsIsDownload, true.ToString());
                }
            }

            return new BoolResult
            {
                Value = isDownload
            };
        }
    }
}