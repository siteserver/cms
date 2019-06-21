using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using SS.CMS.Core.Common;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Admin
{
    public partial class RootController
    {
        private object AdminRedirectCheck(bool checkInstall = false, bool checkDatabaseVersion = false, bool checkLogin = false)
        {
            var redirect = false;
            var redirectUrl = string.Empty;

            if (checkInstall && string.IsNullOrWhiteSpace(_settingsManager.DatabaseConnectionString))
            {
                redirect = true;
                redirectUrl = _urlManager.AdminInstallUrl;
            }
            else if (checkDatabaseVersion && _configRepository.Instance != null &&
                     _configRepository.Instance.DatabaseVersion != _settingsManager.ProductVersion)
            {
                redirect = true;
                redirectUrl = _urlManager.AdminSyncUrl;
            }

            else if (checkLogin && !User.Identity.IsAuthenticated)
            {
                redirect = true;
                redirectUrl = _urlManager.AdminLoginUrl;
            }

            if (redirect)
            {
                return new
                {
                    Value = false,
                    RedirectUrl = redirectUrl
                };
            }

            return null;
        }

        public const string TopMenuIdSite = "Site";
        public const string TopMenuIdLink = "Link";
        public const string TopMenuIdPlugins = "Plugins";
        public const string TopMenuIdSettings = "Settings";
        public const string SiteMenuIdContent = "Content";
        public const string SiteMenuIdTemplate = "Template";
        public const string SiteMenuIdConfiguration = "Configuration";
        public const string SiteMenuIdCreate = "Create";

        public IList<Menu> GetTopMenus()
        {
            var menus = new List<Menu>();

            foreach (var menu in _settingsManager.Menus)
            {
                var valid = false;
                if (StringUtils.EqualsIgnoreCase(menu.Id, TopMenuIdSite) ||
                    StringUtils.EqualsIgnoreCase(menu.Id, TopMenuIdLink))
                {
                    valid = _userManager.HasSitePermissions();
                }
                else
                {
                    if (_userManager.IsSuperAdministrator())
                    {
                        valid = true;
                    }
                    else
                    {
                        if (menu.Permissions != null)
                        {
                            if (menu.Permissions.Any(permission => _userManager.HasAppPermissions(permission)))
                            {
                                valid = true;
                            }
                        }
                    }
                }

                if (valid)
                {
                    menus.Add(menu);
                }
            }

            return menus;
        }

        public IList<Menu> GetAppMenus(string topId)
        {
            var menus = new List<Menu>();

            if (!string.IsNullOrEmpty(topId))
            {
                var topMenu = _settingsManager.Menus.FirstOrDefault(x => x.Id == topId);
                if (topMenu?.Menus != null)
                {
                    foreach (var leftMenu in topMenu.Menus)
                    {
                        var valid = false;
                        if (_userManager.IsSuperAdministrator())
                        {
                            valid = true;
                        }
                        else
                        {
                            if (leftMenu.Permissions != null)
                            {
                                if (leftMenu.Permissions.Any(permission => _userManager.HasAppPermissions(permission)))
                                {
                                    valid = true;
                                }
                            }
                        }

                        if (valid)
                        {
                            menus.Add(leftMenu);
                        }
                    }
                }
            }

            return menus;
        }

        public IList<Menu> GetSiteMenus(int siteId)
        {
            var menus = new List<Menu>();

            var topMenu = _settingsManager.Menus.FirstOrDefault(x => x.Id == TopMenuIdSite);
            if (topMenu?.Menus != null)
            {
                foreach (var leftMenu in topMenu.Menus)
                {
                    var valid = false;
                    if (_userManager.IsSuperAdministrator() || _userManager.IsSiteAdministrator(siteId))
                    {
                        valid = true;
                    }
                    else
                    {
                        if (leftMenu.Permissions != null)
                        {
                            if (leftMenu.Permissions.Any(permission => _userManager.HasSitePermissions(siteId, permission)))
                            {
                                valid = true;
                            }
                        }
                    }

                    if (valid)
                    {
                        menus.Add(new Menu
                        {
                            Id = leftMenu.Id,
                            Link = GetHref(leftMenu, siteId),
                            Text = leftMenu.Text,
                            Target = leftMenu.Target,
                            IconClass = leftMenu.IconClass,
                            Selected = leftMenu.Selected,
                            Menus = leftMenu.Menus
                        });
                    }
                }
            }

            return menus;
        }

        private static string GetHref(Menu menu, int siteId)
        {
            var href = menu.Link;
            if (!PageUtils.IsAbsoluteUrl(href))
            {
                href = PageUtils.AddQueryString(href,
                    new NameValueCollection { { "siteId", siteId.ToString() } });
            }

            return href;
        }

        private Menu GetPluginMenu(Menu menu, IList<string> permissions)
        {
            var tab = new Menu
            {
                Id = menu.Id,
                Text = menu.Text,
                IconClass = menu.IconClass,
                Selected = false,
                Link = menu.Link,
                Target = menu.Target,
                Permissions = permissions
            };
            if (menu.Menus != null && menu.Menus.Count > 0)
            {
                tab.Menus = new List<Menu>();
                foreach (var childMenu in menu.Menus)
                {
                    tab.Menus.Add(childMenu);
                }
            }
            return tab;
        }

        //private List<Menu> GetTopMenus(SiteInfo siteInfo, bool isSuperAdmin, List<int> siteIdListLatestAccessed, List<int> siteIdListWithPermissions)
        //{
        //    var menus = new List<Menu>();

        //    if (siteInfo != null && siteIdListWithPermissions.Contains(siteInfo.Id))
        //    {
        //        var siteMenus = new List<Menu>();
        //        if (siteIdListWithPermissions.Count == 1)
        //        {
        //            menus.Add(new Menu
        //            {
        //                Text = siteInfo.SiteName,
        //                Menus = siteMenus
        //            });
        //        }
        //        else
        //        {
        //            var siteIdListOrderByLevel = _siteRepository.GetSiteIdListOrderByLevel();
        //            var siteIdList = _administratorRepository.GetLatestTop10SiteIdList(siteIdListLatestAccessed, siteIdListOrderByLevel, siteIdListWithPermissions);
        //            foreach (var siteId in siteIdList)
        //            {
        //                var site = _siteRepository.GetSiteInfo(siteId);
        //                if (site == null) continue;

        //                siteMenus.Add(new Menu
        //                {
        //                    Link = _urlManager.GetAdminIndexUrl(site.Id, string.Empty),
        //                    Target = "_top",
        //                    Text = site.SiteName
        //                });
        //            }
        //            siteMenus.Add(new Menu
        //            {
        //                //Href = ModalSiteSelect.GetRedirectUrl(siteInfo.Id),
        //                Link = PageUtils.UnClickableUrl,
        //                Target = "_layer",
        //                Text = "全部站点..."
        //            });
        //            menus.Add(new Menu
        //            {
        //                Text = siteInfo.SiteName,
        //                //Href = ModalSiteSelect.GetRedirectUrl(siteInfo.Id),
        //                Link = PageUtils.UnClickableUrl,
        //                Target = "_layer",
        //                Menus = siteMenus
        //            });
        //        }

        //        var linkMenus = new List<Menu>
        //        {
        //            new Menu {Link = _urlManager.GetSiteUrl(siteInfo, false), Target = "_blank", Text = "访问站点"},
        //            new Menu {Link = _urlManager.GetPreviewSiteUrl(siteInfo.Id), Target = "_blank", Text = "预览站点"}
        //        };
        //        menus.Add(new Menu { Text = "站点链接", Menus = linkMenus });
        //    }

        //    if (isSuperAdmin)
        //    {
        //        foreach (var tab in _menuManager.GetTopMenuTabs())
        //        {
        //            var tabs = _menuManager.GetTabList(tab.Id, 0);
        //            tab.Menus = tabs;

        //            menus.Add(tab);
        //        }
        //    }

        //    return menus;
        //}

        //private List<Menu> GetLeftMenus(SiteInfo siteInfo, string topId)
        //{
        //    var menus = new List<Menu>();

        //    var tabs = _menuManager.GetTabList(topId, siteInfo.Id);
        //    foreach (var parent in tabs)
        //    {
        //        if (!_userManager.IsSuperAdministrator() && !_menuManager.IsValid(parent, permissionList)) continue;

        //        var children = new List<Menu>();
        //        if (parent.Menus != null && parent.Menus.Count > 0)
        //        {
        //            foreach (var childTab in parent.Menus)
        //            {
        //                if (!isSuperAdmin && !_menuManager.IsValid(childTab, permissionList)) continue;

        //                children.Add(new Menu
        //                {
        //                    Id = childTab.Id,
        //                    Link = GetHref(childTab, siteInfo.Id),
        //                    Text = childTab.Text,
        //                    Target = childTab.Target,
        //                    IconClass = childTab.IconClass
        //                });
        //            }
        //        }

        //        menus.Add(new Menu
        //        {
        //            Id = parent.Id,
        //            Link = GetHref(parent, siteInfo.Id),
        //            Text = parent.Text,
        //            Target = parent.Target,
        //            IconClass = parent.IconClass,
        //            Selected = parent.Selected,
        //            Menus = children
        //        });
        //    }

        //    return menus;
        //}

        //private static string GetHref(Menu menu, int siteId)
        //{
        //    var href = menu.Link;
        //    if (!PageUtils.IsAbsoluteUrl(href))
        //    {
        //        href = PageUtils.AddQueryString(href,
        //            new NameValueCollection { { "siteId", siteId.ToString() } });
        //    }

        //    return href;
        //}
    }
}