using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using SSCMS;
using SSCMS.Dto.Request;
using SSCMS.Core.Utils;
using SSCMS.Utils;
using SSCMS.Web.Controllers.Admin.Settings.Sites;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class IndexController
    {
        public class Local
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string AvatarUrl { get; set; }
            public string Level { get; set; }
        }

        public class GetRequest : SiteRequest
        {
            public string SessionId { get; set; }
        }

        public class GetResult
        {
            public bool Value { get; set; }
            public string RedirectUrl { get; set; }
            public string DefaultPageUrl { get; set; }
            public bool IsNightly { get; set; }
            public string ProductVersion { get; set; }
            public string PluginVersion { get; set; }
            public string TargetFramework { get; set; }
            public string AdminLogoUrl { get; set; }
            public string AdminTitle { get; set; }
            public bool IsSuperAdmin { get; set; }
            public List<object> PackageList { get; set; }
            public List<string> PackageIds { get; set; }
            public List<Tab> Menus { get; set; }
            public string SiteUrl { get; set; }
            public string PreviewUrl { get; set; }
            public Local Local { get; set; }
        }

        public class CreateRequest
        {
            public string SessionId { get; set; }
        }

        public class DownloadRequest
        {
            public string PackageId { get; set; }
            public string Version { get; set; }
        }

        private async Task<List<Tab>> GetTopMenusAsync(TabManager tabManager, Site siteInfo, bool isSuperAdmin, List<int> siteIdListLatestAccessed, List<int> siteIdListWithPermissions, List<string> permissionList, List<Tab> siteMenus)
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
                    var switchMenus = new List<Tab>();
                    var allSiteMenus = new List<Tab>();

                    var siteIdList = await _siteRepository.GetLatestSiteIdListAsync(siteIdListLatestAccessed, siteIdListWithPermissions);
                    foreach (var siteId in siteIdList)
                    {
                        var site = await _siteRepository.GetAsync(siteId);
                        if (site == null) continue;

                        allSiteMenus.Add(new Tab
                        {
                            Href = $"{_pathManager.GetAdminUrl()}?siteId={site.Id}",
                            Target = "_top",
                            Text = site.SiteName
                        });
                    }

                    switchMenus.Add(new Tab
                    {
                        IconClass = "el-icon-refresh",
                        Href = _pathManager.GetAdminUrl(SitesLayerSelectController.Route),
                        Target = "_layer",
                        Text = "选择站点"
                    });
                    switchMenus.Add(new Tab
                    {
                        IconClass = "ion-earth",
                        Text = "最近访问",
                        Selected = true,
                        Children = allSiteMenus.ToArray()
                    });

                    menus.Add(new Tab
                    {
                        Text = "切换站点",
                        Children = switchMenus.ToArray()
                    });
                }
            }

            if (isSuperAdmin)
            {
                foreach (var tab in tabManager.GetTopMenuTabs())
                {
                    var tabs = await tabManager.GetTabListAsync(tab.Id, 0);
                    tab.Children = tabs.ToArray();

                    menus.Add(tab);
                }
            }
            else
            {
                foreach (var tab in tabManager.GetTopMenuTabs())
                {
                    if (!tabManager.IsValid(tab, permissionList)) continue;

                    var tabToAdd = new Tab
                    {
                        Id = tab.Id,
                        Name = tab.Name,
                        Text = tab.Text,
                        Target = tab.Target,
                        Href = tab.Href
                    };
                    var tabs = await tabManager.GetTabListAsync(tab.Id, 0);
                    var tabsToAdd = new List<Tab>();
                    foreach (var menu in tabs)
                    {
                        if (!tabManager.IsValid(menu, permissionList)) continue;

                        Tab[] children = null;
                        if (menu.Children != null)
                        {
                            children = menu.Children.Where(child => tabManager.IsValid(child, permissionList))
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

        private static async Task<List<Tab>> GetLeftMenusAsync(TabManager tabManager, Site site, string topId, bool isSuperAdmin, List<string> permissionList)
        {
            var menus = new List<Tab>();

            var tabs = await tabManager.GetTabListAsync(topId, site.Id);
            foreach (var parent in tabs)
            {
                if (!isSuperAdmin && !tabManager.IsValid(parent, permissionList)) continue;

                var children = new List<Tab>();
                if (parent.Children != null && parent.Children.Length > 0)
                {
                    var tabCollection = new TabCollection(parent.Children);
                    if (tabCollection.Tabs != null && tabCollection.Tabs.Length > 0)
                    {
                        foreach (var childTab in tabCollection.Tabs)
                        {
                            if (!isSuperAdmin && !tabManager.IsValid(childTab, permissionList)) continue;

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

        private static string GetHref(Tab tab, int siteId)
        {
            var href = tab.Href;
            if (!PageUtils.IsAbsoluteUrl(href))
            {
                href = PageUtils.AddQueryString(href,
                    new NameValueCollection { { "siteId", siteId.ToString() } });
            }

            return href;
        }

        public async Task<(bool redirect, string redirectUrl)> AdminRedirectCheckAsync()
        {
            var redirect = false;
            var redirectUrl = string.Empty;

            var config = await _configRepository.GetAsync();

            if (string.IsNullOrEmpty(_settingsManager.Database.ConnectionString))
            {
                redirect = true;
                redirectUrl = _pathManager.GetAdminUrl(InstallController.Route);
            }
            else if (config.Initialized &&
                     config.DatabaseVersion != _settingsManager.ProductVersion)
            {
                redirect = true;
                redirectUrl = _pathManager.GetAdminUrl(SyncDatabaseController.Route);
            }

            return (redirect, redirectUrl);
        }
    }
}