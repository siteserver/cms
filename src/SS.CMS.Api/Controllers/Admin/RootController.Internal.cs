using System.Collections.Generic;
using System.Collections.Specialized;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Models;
using SS.CMS.Core.Common;
using SS.CMS.Core.Common.Serialization;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Admin
{
    public partial class RootController
    {
        private object AdminRedirectCheck(bool checkInstall = false, bool checkDatabaseVersion = false,
            bool checkLogin = false)
        {
            var redirect = false;
            var redirectUrl = string.Empty;

            if (checkInstall && string.IsNullOrWhiteSpace(_settingsManager.DatabaseConnectionString))
            {
                redirect = true;
                redirectUrl = _urlManager.AdminInstallUrl;
            }
            else if (checkDatabaseVersion && _settingsManager.ConfigInfo.Initialized &&
                     _settingsManager.ConfigInfo.DatabaseVersion != SystemManager.ProductVersion)
            {
                redirect = true;
                redirectUrl = _urlManager.AdminSyncUrl;
            }
            else if (checkLogin && !_identityManager.IsAdminLoggin)
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

        private List<Menu> GetTopMenus(MenuManager menuManager, SiteInfo siteInfo, bool isSuperAdmin, List<int> siteIdListLatestAccessed, List<int> siteIdListWithPermissions)
        {
            var menus = new List<Menu>();

            if (siteInfo != null && siteIdListWithPermissions.Contains(siteInfo.Id))
            {
                var siteMenus = new List<Menu>();
                if (siteIdListWithPermissions.Count == 1)
                {
                    menus.Add(new Menu
                    {
                        Text = siteInfo.SiteName,
                        Menus = siteMenus
                    });
                }
                else
                {
                    var siteIdListOrderByLevel = _siteRepository.GetSiteIdListOrderByLevel();
                    var siteIdList = _administratorRepository.GetLatestTop10SiteIdList(siteIdListLatestAccessed, siteIdListOrderByLevel, siteIdListWithPermissions);
                    foreach (var siteId in siteIdList)
                    {
                        var site = _siteRepository.GetSiteInfo(siteId);
                        if (site == null) continue;

                        siteMenus.Add(new Menu
                        {
                            Href = _urlManager.GetAdminIndexUrl(site.Id, string.Empty),
                            Target = "_top",
                            Text = site.SiteName
                        });
                    }
                    siteMenus.Add(new Menu
                    {
                        //Href = ModalSiteSelect.GetRedirectUrl(siteInfo.Id),
                        Href = PageUtils.UnclickedUrl,
                        Target = "_layer",
                        Text = "全部站点..."
                    });
                    menus.Add(new Menu
                    {
                        Text = siteInfo.SiteName,
                        //Href = ModalSiteSelect.GetRedirectUrl(siteInfo.Id),
                        Href = PageUtils.UnclickedUrl,
                        Target = "_layer",
                        Menus = siteMenus
                    });
                }

                var linkMenus = new List<Menu>
                {
                    new Menu {Href = _urlManager.GetSiteUrl(siteInfo, false), Target = "_blank", Text = "访问站点"},
                    new Menu {Href = _urlManager.GetPreviewSiteUrl(siteInfo.Id), Target = "_blank", Text = "预览站点"}
                };
                menus.Add(new Menu { Text = "站点链接", Menus = linkMenus });
            }

            if (isSuperAdmin)
            {
                foreach (var tab in menuManager.GetTopMenuTabs())
                {
                    var tabs = menuManager.GetTabList(tab.Id, 0);
                    tab.Menus = tabs;

                    menus.Add(tab);
                }
            }

            return menus;
        }

        private static List<Menu> GetLeftMenus(MenuManager menuManager, SiteInfo siteInfo, string topId, bool isSuperAdmin, List<string> permissionList)
        {
            var menus = new List<Menu>();

            var tabs = menuManager.GetTabList(topId, siteInfo.Id);
            foreach (var parent in tabs)
            {
                if (!isSuperAdmin && !menuManager.IsValid(parent, permissionList)) continue;

                var children = new List<Menu>();
                if (parent.Menus != null && parent.Menus.Count > 0)
                {
                    foreach (var childTab in parent.Menus)
                    {
                        if (!isSuperAdmin && !menuManager.IsValid(childTab, permissionList)) continue;

                        children.Add(new Menu
                        {
                            Id = childTab.Id,
                            Href = GetHref(childTab, siteInfo.Id),
                            Text = childTab.Text,
                            Target = childTab.Target,
                            IconClass = childTab.IconClass
                        });
                    }
                }

                menus.Add(new Menu
                {
                    Id = parent.Id,
                    Href = GetHref(parent, siteInfo.Id),
                    Text = parent.Text,
                    Target = parent.Target,
                    IconClass = parent.IconClass,
                    Selected = parent.Selected,
                    Menus = children
                });
            }

            return menus;
        }

        private static string GetHref(Menu menu, int siteId)
        {
            var href = menu.Href;
            if (!PageUtils.IsAbsoluteUrl(href))
            {
                href = PageUtils.AddQueryString(href,
                    new NameValueCollection { { "siteId", siteId.ToString() } });
            }

            return href;
        }
    }
}