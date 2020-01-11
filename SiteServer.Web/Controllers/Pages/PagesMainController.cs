using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Api.Preview;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Packaging;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.StlParser;

namespace SiteServer.API.Controllers.Pages
{
    
    [RoutePrefix("pages/main")]
    public class PagesMainController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsCreate = "actions/create";
        private const string RouteActionsDownload = "actions/download";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var redirect = await request.AdminRedirectCheckAsync(checkInstall:true, checkDatabaseVersion:true, checkLogin:true);
                if (redirect != null) return Ok(redirect);

                var siteId = request.GetQueryInt("siteId");
                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                var adminInfo = request.Administrator;
                var permissions = request.AdminPermissionsImpl;
                var isSuperAdmin = await permissions.IsSuperAdminAsync();
                var siteIdListWithPermissions = await permissions.GetSiteIdListAsync();

                if (site == null || !siteIdListWithPermissions.Contains(site.Id))
                {
                    if (siteIdListWithPermissions.Contains(adminInfo.SiteId))
                    {
                        return Ok(new
                        {
                            Value = false,
                            RedirectUrl = PageUtils.GetMainUrl(adminInfo.SiteId)
                        });
                    }

                    if (siteIdListWithPermissions.Count > 0)
                    {
                        return Ok(new
                        {
                            Value = false,
                            RedirectUrl = PageUtils.GetMainUrl(siteIdListWithPermissions[0])
                        });
                    }

                    if (isSuperAdmin)
                    {
                        return Ok(new
                        {
                            Value = false,
                            RedirectUrl = PageUtils.GetSettingsUrl("siteAdd")
                        });
                    }

                    return Ok(new
                    {
                        Value = false,
                        RedirectUrl = $"error.html?message={HttpUtility.UrlEncode("您没有可以管理的站点，请联系超级管理员协助解决")}"
                    });
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

                var siteUrl = PageUtility.GetSiteUrl(site, false);
                var previewUrl = ApiRoutePreview.GetSiteUrl(site.Id);

                return Ok(new
                {
                    Value = true,
                    DefaultPageUrl = await PluginMenuManager.GetSystemDefaultPageUrlAsync(siteId) ?? "dashboard.cshtml",
                    IsNightly = WebConfigUtils.IsNightlyUpdate,
                    SystemManager.ProductVersion,
                    SystemManager.PluginVersion,
                    SystemManager.TargetFramework,
                    SystemManager.EnvironmentVersion,
                    config.AdminLogoUrl,
                    config.AdminTitle,
                    IsSuperAdmin = isSuperAdmin,
                    PackageList = packageList,
                    PackageIds = packageIds, 
                    Menus = menus,
                    SiteUrl = siteUrl,
                    PreviewUrl = previewUrl,
                    Local = new
                    {
                        UserId = adminInfo.Id,
                        adminInfo.UserName,
                        adminInfo.AvatarUrl,
                        Level = await permissions.GetAdminLevelAsync()
                    }
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private static async Task<List<Tab>> GetTopMenusAsync(Site siteInfo, bool isSuperAdmin, List<int> siteIdListLatestAccessed, List<int> siteIdListWithPermissions, List<string> permissionList, List<Tab> siteMenus)
        {
            var menus = new List<Tab>();

            if (siteInfo != null && siteIdListWithPermissions.Contains(siteInfo.Id))
            {
                menus.Add(new Tab
                {
                    Id = Constants.TopMenu.SiteCurrent,
                    Text = siteInfo.SiteName,
                    Children = siteMenus.ToArray()
                });

                if (siteIdListWithPermissions.Count > 1)
                {
                    var allSiteMenus = new List<Tab>();

                    var siteIdList = await DataProvider.AdministratorRepository.GetLatestTop10SiteIdListAsync(siteIdListLatestAccessed, siteIdListWithPermissions);
                    foreach (var siteId in siteIdList)
                    {
                        var site = await DataProvider.SiteRepository.GetAsync(siteId);
                        if (site == null) continue;

                        allSiteMenus.Add(new Tab
                        {
                            Href = PageUtils.GetMainUrl(site.Id),
                            Target = "_top",
                            Text = site.SiteName
                        });
                    }
                    allSiteMenus.Add(new Tab
                    {
                        Href = ModalSiteSelect.GetRedirectUrl(siteInfo.Id),
                        Target = "_layer",
                        Text = "全部站点..."
                    });
                    menus.Add(new Tab
                    {
                        Id = Constants.TopMenu.SiteCurrent,
                        Text = "切换站点",
                        Href = ModalSiteSelect.GetRedirectUrl(siteInfo.Id),
                        Target = "_layer",
                        Children = allSiteMenus.ToArray()
                    });
                }
            }

            if (isSuperAdmin)
            {
                foreach (var tab in TabManager.GetTopMenuTabs())
                {
                    var tabs = await TabManager.GetTabListAsync(tab.Id, 0);
                    tab.Children = tabs.ToArray();

                    menus.Add(tab);
                }
            }
            else
            {
                foreach (var tab in TabManager.GetTopMenuTabs())
                {
                    if (!TabManager.IsValid(tab, permissionList)) continue;

                    var tabToAdd = new Tab
                    {
                        Id = tab.Id,
                        Name = tab.Name,
                        Text = tab.Text,
                        Target = tab.Target,
                        Href = tab.Href
                    };
                    var tabs = await TabManager.GetTabListAsync(tab.Id, 0);
                    var tabsToAdd = new List<Tab>();
                    foreach (var menu in tabs)
                    {
                        if (!TabManager.IsValid(menu, permissionList)) continue;

                        Tab[] children = null;
                        if (menu.Children != null)
                        {
                            children = menu.Children.Where(child => TabManager.IsValid(child, permissionList))
                                .ToArray();
                        }

                        tabsToAdd.Add(new Tab
                        {
                            Id = menu.Id,
                            Name = menu.Name,
                            Text = menu.Text,
                            Target = menu.Target,
                            Href = menu.Href,
                            Children = children
                        });
                    }
                    tabToAdd.Children = tabsToAdd.ToArray();

                    menus.Add(tabToAdd);
                }
            }

            return menus;
        }

        private static async Task<List<Tab>> GetLeftMenusAsync(Site site, string topId, bool isSuperAdmin, List<string> permissionList)
        {
            var menus = new List<Tab>();

            var tabs = await TabManager.GetTabListAsync(topId, site.Id);
            foreach (var parent in tabs)
            {
                if (!isSuperAdmin && !TabManager.IsValid(parent, permissionList)) continue;

                var children = new List<Tab>();
                if (parent.Children != null && parent.Children.Length > 0)
                {
                    var tabCollection = new TabCollection(parent.Children);
                    if (tabCollection.Tabs != null && tabCollection.Tabs.Length > 0)
                    {
                        foreach (var childTab in tabCollection.Tabs)
                        {
                            if (!isSuperAdmin && !TabManager.IsValid(childTab, permissionList)) continue;

                            children.Add(new Tab
                            {
                                Id = childTab.Id,
                                Href = GetHref(childTab, site.Id),
                                Text = childTab.Text,
                                Target = childTab.Target,
                                IconClass = childTab.IconClass
                            });
                        }
                    }
                }

                menus.Add(new Tab
                {
                    Id = parent.Id,
                    Href = GetHref(parent, site.Id),
                    Text = parent.Text,
                    Target = parent.Target,
                    IconClass = parent.IconClass,
                    Selected = parent.Selected,
                    Children = children.ToArray()
                });
            }

            return menus;
        }

        public static string GetHref(Tab tab, int siteId)
        {
            var href = tab.Href;
            if (!PageUtils.IsAbsoluteUrl(href))
            {
                href = PageUtils.AddQueryString(href,
                    new NameValueCollection { { "siteId", siteId.ToString() } });
            }

            return href;
        }

        [HttpPost, Route(RouteActionsCreate)]
        public async Task<IHttpActionResult> Create()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin || request.Administrator == null)
                {
                    return Unauthorized();
                }

#if !DEBUG
                var sessionId = request.GetPostString("sessionId");
                var cacheKey = Constants.GetSessionIdCacheKey(request.AdminId);
                if (string.IsNullOrEmpty(sessionId) || CacheUtils.GetString(cacheKey) != sessionId)
                {
                    return Unauthorized();
                }
#endif

                var config = await DataProvider.ConfigRepository.GetAsync();

                if (request.Administrator.LastActivityDate != null && config.IsAdminEnforceLogout)
                {
                    var ts = new TimeSpan(DateTime.Now.Ticks - request.Administrator.LastActivityDate.Value.Ticks);
                    if (ts.TotalMinutes > config.AdminEnforceLogoutMinutes)
                    {
                        return Unauthorized();
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

                return Ok(new
                {
                    Value = count
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsDownload)]
        public async Task<IHttpActionResult> Download()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();

            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

            var packageId = request.GetPostString("packageId");
            var version = request.GetPostString("version");

            try
            {
                PackageUtils.DownloadPackage(packageId, version);
            }
            catch
            {
                PackageUtils.DownloadPackage(packageId, version);
            }

            var isDownload = PackageUtils.IsPackageDownload(packageId, version);
            if (isDownload)
            {
                if (StringUtils.EqualsIgnoreCase(packageId, PackageUtils.PackageIdSsCms))
                {
                    await DataProvider.DbCacheRepository.RemoveAndInsertAsync(PackageUtils.CacheKeySsCmsIsDownload, true.ToString());
                }
            }

            return Ok(new
            {
                Value = isDownload
            });
        }
    }
}