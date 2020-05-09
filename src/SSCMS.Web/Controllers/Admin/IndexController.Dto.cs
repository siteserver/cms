using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class IndexController
    {
        private const string IdSite = "site";

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

        public class GetPlugin
        {
            public string PluginId { get; set; }
            public string DisplayName { get; set; }
            public string Version { get; set; }
        }

        public class GetResult
        {
            public bool Value { get; set; }
            public string RedirectUrl { get; set; }
            public bool IsNightly { get; set; }
            public string Version { get; set; }
            public string OperatingSystem { get; set; }
            public string AdminLogoUrl { get; set; }
            public string AdminTitle { get; set; }
            public bool IsSuperAdmin { get; set; }
            public string Culture { get; set; }
            public List<GetPlugin> Plugins { get; set; }
            public IList<Menu> Menus { get; set; }
            public string SiteType { get; set; }
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
            public string Version { get; set; }
        }

        public class SetLanguageRequest
        {
            public string Culture { get; set; }
        }

        private IList<Menu> GetChildren(Menu menu, IList<string> permissions, Func<Menu, Menu> op = null)
        {
            if (menu.Children == null || menu.Children.Count == 0) return null;

            foreach (var child in menu.Children)
            {
                child.Children = GetChildren(child, permissions, op);
            }

            var children = new List<Menu>(menu.Children);

            if (op != null)
            {
                children = children.Select(op).ToList();
            }

            return children.Where(x => _authManager.IsMenuValid(x, permissions)).ToList();
        }

        //private async Task<IList<Menu>> GetTopMenusAsync(Site siteInfo, bool isSuperAdmin, List<int> siteIdListLatestAccessed, List<int> siteIdListWithPermissions, List<string> permissionList)
        //{
        //    IList<Menu> menus = new List<Menu>();

        //    if (siteInfo != null && siteIdListWithPermissions.Contains(siteInfo.Id))
        //    {
        //        var siteMenu = _menusAccessor.CurrentValue.FirstOrDefault(x => x.Id == IdSite);

        //        if (siteMenu != null)
        //        {
        //            siteMenu.Text = siteInfo.SiteName;
        //            menus.Add(siteMenu);
        //        }

        //        if (siteIdListWithPermissions.Count > 1)
        //        {
        //            var switchMenus = new List<Menu>();
        //            var allSiteMenus = new List<Menu>();

        //            var siteIdList = await _siteRepository.GetLatestSiteIdListAsync(siteIdListLatestAccessed, siteIdListWithPermissions);
        //            foreach (var siteId in siteIdList)
        //            {
        //                var site = await _siteRepository.GetAsync(siteId);
        //                if (site == null) continue;

        //                allSiteMenus.Add(new Menu
        //                {
        //                    Link = $"{_pathManager.GetAdminUrl()}?siteId={site.Id}",
        //                    Target = "_top",
        //                    Text = site.SiteName
        //                });
        //            }

        //            switchMenus.Add(new Menu
        //            {
        //                IconClass = "el-icon-refresh",
        //                Link = _pathManager.GetAdminUrl(SitesLayerSelectController.Route),
        //                Target = "_layer",
        //                Text = "选择站点"
        //            });
        //            switchMenus.Add(new Menu
        //            {
        //                IconClass = "ion-earth",
        //                Text = "最近访问",
        //                Selected = true,
        //                Children = allSiteMenus.ToArray()
        //            });

        //            menus.Add(new Menu
        //            {
        //                Text = "切换站点",
        //                Children = switchMenus.ToArray()
        //            });
        //        }
        //    }

        //    var topMenus = _menusAccessor.CurrentValue.Where(x => x.Id != IdSite).ToList();

        //    if (isSuperAdmin)
        //    {
        //        menus = topMenus;
        //    }
        //    else
        //    {
        //        foreach (var tab in topMenus)
        //        {
        //            if (!_authManager.IsValid(tab, permissionList)) continue;

        //            var tabToAdd = new Menu
        //            {
        //                Id = tab.Id,
        //                Text = tab.Text,
        //                Target = tab.Target,
        //                Link = tab.Link
        //            };
        //            var tabs = tab.Children;
        //            var tabsToAdd = new List<Menu>();
        //            foreach (var menu in tabs)
        //            {
        //                if (!_authManager.IsValid(menu, permissionList)) continue;

        //                List<Menu> children = null;
        //                if (menu.Children != null)
        //                {
        //                    children = menu.Children.Where(child => _authManager.IsValid(child, permissionList)).ToList();
        //                }

        //                tabsToAdd.Add(new Menu
        //                {
        //                    Id = menu.Id,
        //                    Text = menu.Text,
        //                    Target = menu.Target,
        //                    Link = menu.Link,
        //                    Children = children
        //                });
        //            }
        //            tabToAdd.Children = tabsToAdd.ToArray();

        //            menus.Add(tabToAdd);
        //        }
        //    }

        //    return menus;
        //}

        //private List<Menu> GetLeftMenus(Site site, bool isSuperAdmin, List<string> permissionList)
        //{
        //    var menus = new List<Menu>();

        //    var siteMenu = _menusAccessor.CurrentValue.FirstOrDefault(x => x.Id == IdSite);
        //    if (siteMenu == null) return menus;

        //    foreach (var parent in siteMenu.Children)
        //    {
        //        if (!isSuperAdmin && !_authManager.IsValid(parent, permissionList)) continue;

        //        var children = new List<Menu>();
        //        if (parent.Children != null)
        //        {
        //            var tabCollection = new List<Menu>(parent.Children);
        //            foreach (var childTab in tabCollection)
        //            {
        //                if (!isSuperAdmin && !_authManager.IsValid(childTab, permissionList)) continue;

        //                children.Add(new Menu
        //                {
        //                    Id = childTab.Id,
        //                    Link = GetHref(childTab, site.Id),
        //                    Text = childTab.Text,
        //                    Target = childTab.Target,
        //                    IconClass = childTab.IconClass
        //                });
        //            }
        //        }

        //        menus.Add(new Menu
        //        {
        //            Id = parent.Id,
        //            Link = GetHref(parent, site.Id),
        //            Text = parent.Text,
        //            Target = parent.Target,
        //            IconClass = parent.IconClass,
        //            Selected = parent.Selected,
        //            Children = children.ToArray()
        //        });
        //    }

        //    return menus;
        //}

        //private static string GetHref(Menu tab, int siteId)
        //{
        //    var href = tab.Link;
        //    if (!PageUtils.IsAbsoluteUrl(href))
        //    {
        //        href = PageUtils.AddQueryString(href,
        //            new NameValueCollection { { "siteId", siteId.ToString() } });
        //    }

        //    return href;
        //}

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
                     config.DatabaseVersion != _settingsManager.Version)
            {
                redirect = true;
                redirectUrl = _pathManager.GetAdminUrl(SyncDatabaseController.Route);
            }

            return (redirect, redirectUrl);
        }
    }
}