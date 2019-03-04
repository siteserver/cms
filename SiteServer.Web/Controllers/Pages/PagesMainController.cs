using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.Api.Preview;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Packaging;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.StlParser;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages
{
    [RoutePrefix("pages/main")]
    public class PagesMainController : ApiController
    {
        private const string Route = "";
        private const string RouteActionsCreate = "actions/create";
        private const string RouteActionsDownload = "actions/download";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var request = new RequestImpl();
                var redirect = request.AdminRedirectCheck(checkInstall:true, checkDatabaseVersion:true, checkLogin:true);
                if (redirect != null) return Ok(redirect);

                var siteId = request.GetQueryInt("siteId");
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var adminInfo = request.AdminInfo;
                var permissions = request.AdminPermissionsImpl;
                var isSuperAdmin = permissions.IsConsoleAdministrator;
                var siteIdListWithPermissions = permissions.GetSiteIdList();

                if (siteInfo == null || !siteIdListWithPermissions.Contains(siteInfo.Id))
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
                            RedirectUrl = PageSiteAdd.GetRedirectUrl()
                        });
                    }

                    return Ok(new
                    {
                        Value = false,
                        RedirectUrl = $"pageError.html?message={HttpUtility.UrlEncode("您没有可以管理的站点，请联系超级管理员协助解决")}"
                    });
                }

                var packageIds = new List<string>
                {
                    PackageUtils.PackageIdSsCms
                };
                var packageList = new List<object>();
                var dict = PluginManager.GetPluginIdAndVersionDict();
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

                var siteIdListLatestAccessed = DataProvider.AdministratorDao.UpdateSiteId(adminInfo, siteInfo.Id);

                var permissionList = new List<string>(permissions.PermissionList);
                if (permissions.HasSitePermissions(siteInfo.Id))
                {
                    var websitePermissionList = permissions.GetSitePermissions(siteInfo.Id);
                    if (websitePermissionList != null)
                    {
                        permissionList.AddRange(websitePermissionList);
                    }
                }
                var channelPermissions = permissions.GetChannelPermissions(siteInfo.Id);
                if (channelPermissions.Count > 0)
                {
                    permissionList.AddRange(channelPermissions);
                }

                var topMenus = GetTopMenus(siteInfo, isSuperAdmin, siteIdListLatestAccessed, siteIdListWithPermissions);
                var siteMenus =
                    GetLeftMenus(siteInfo, ConfigManager.TopMenu.IdSite, isSuperAdmin, permissionList);
                var pluginMenus = GetLeftMenus(siteInfo, string.Empty, isSuperAdmin, permissionList);

                return Ok(new
                {
                    Value = true,
                    DefaultPageUrl = PluginMenuManager.GetSystemDefaultPageUrl(siteId) ?? "dashboard.cshtml",
                    IsNightly = WebConfigUtils.IsNightlyUpdate,
                    Version = SystemManager.PluginVersion,
                    IsSuperAdmin = isSuperAdmin,
                    PackageList = packageList,
                    PackageIds = packageIds,
                    CurrentVersion = SystemManager.Version,
                    TopMenus = topMenus,
                    SiteMenus = siteMenus,
                    PluginMenus = pluginMenus,
                    Local = new
                    {
                        UserId = adminInfo.Id,
                        adminInfo.UserName,
                        adminInfo.AvatarUrl,
                        Level = permissions.GetAdminLevel()
                    }
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private static List<Tab> GetTopMenus(SiteInfo siteInfo, bool isSuperAdmin, List<int> siteIdListLatestAccessed, List<int> siteIdListWithPermissions)
        {
            var menus = new List<Tab>();

            if (siteInfo != null && siteIdListWithPermissions.Contains(siteInfo.Id))
            {
                var siteMenus = new List<Tab>();
                if (siteIdListWithPermissions.Count == 1)
                {
                    menus.Add(new Tab
                    {
                        Text = siteInfo.SiteName,
                        Children = siteMenus.ToArray()
                    });
                }
                else
                {
                    var siteIdList = AdminManager.GetLatestTop10SiteIdList(siteIdListLatestAccessed, siteIdListWithPermissions);
                    foreach (var siteId in siteIdList)
                    {
                        var site = SiteManager.GetSiteInfo(siteId);
                        if (site == null) continue;

                        siteMenus.Add(new Tab
                        {
                            Href = PageUtils.GetMainUrl(site.Id),
                            Target = "_top",
                            Text = site.SiteName
                        });
                    }
                    siteMenus.Add(new Tab
                    {
                        Href = ModalSiteSelect.GetRedirectUrl(siteInfo.Id),
                        Target = "_layer",
                        Text = "全部站点..."
                    });
                    menus.Add(new Tab
                    {
                        Text = siteInfo.SiteName,
                        Href = ModalSiteSelect.GetRedirectUrl(siteInfo.Id),
                        Target = "_layer",
                        Children = siteMenus.ToArray()
                    });
                }

                var linkMenus = new List<Tab>
                {
                    new Tab {Href = PageUtility.GetSiteUrl(siteInfo, false), Target = "_blank", Text = "访问站点"},
                    new Tab {Href = ApiRoutePreview.GetSiteUrl(siteInfo.Id), Target = "_blank", Text = "预览站点"}
                };
                menus.Add(new Tab {Text = "站点链接", Children = linkMenus.ToArray()});
            }

            if (isSuperAdmin)
            {
                foreach (var tab in TabManager.GetTopMenuTabs())
                {
                    var tabs = TabManager.GetTabList(tab.Id, 0);
                    tab.Children = tabs.ToArray();

                    menus.Add(tab);
                }
            }

            return menus;
        }

        private static List<Tab> GetLeftMenus(SiteInfo siteInfo, string topId, bool isSuperAdmin, List<string> permissionList)
        {
            var menus = new List<Tab>();

            var tabs = TabManager.GetTabList(topId, siteInfo.Id);
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
                                Href = GetHref(childTab, siteInfo.Id),
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
                    Href = GetHref(parent, siteInfo.Id),
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
                var request = new RequestImpl();
                if (!request.IsAdminLoggin)
                {
                    return Unauthorized();
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
        public IHttpActionResult Download()
        {
            var request = new RequestImpl();

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

            if (StringUtils.EqualsIgnoreCase(packageId, PackageUtils.PackageIdSsCms))
            {
                CacheDbUtils.RemoveAndInsert(PackageUtils.CacheKeySsCmsIsDownload, true.ToString());
            }

            return Ok(new
            {
                Value = true
            });
        }
    }
}