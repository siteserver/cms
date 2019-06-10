using System.Collections.Generic;
using System.Collections.Specialized;
using SS.CMS.Abstractions;
using SS.CMS.Core.Api.Preview;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Core.Settings;
using SS.CMS.Core.Settings.Menus;
using SS.CMS.Utils;
using Menu = SS.CMS.Core.Settings.Menus.Menu;
using AppContext = SS.CMS.Core.Settings.AppContext;

namespace SS.CMS.Api.Controllers.Admin
{
    public partial class RootController
    {
        private static object AdminRedirectCheck(IIdentity identity, bool checkInstall = false, bool checkDatabaseVersion = false,
            bool checkLogin = false)
        {
            var redirect = false;
            var redirectUrl = string.Empty;

            if (checkInstall && string.IsNullOrWhiteSpace(AppContext.Db.ConnectionString))
            {
                redirect = true;
                redirectUrl = AdminUrl.InstallUrl;
            }
            else if (checkDatabaseVersion && ConfigManager.Instance.Initialized &&
                     ConfigManager.Instance.DatabaseVersion != SystemManager.ProductVersion)
            {
                redirect = true;
                redirectUrl = AdminUrl.SyncUrl;
            }
            else if (checkLogin && !identity.IsAdminLoggin)
            {
                redirect = true;
                redirectUrl = AdminUrl.LoginUrl;
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

        private static List<Menu> GetTopMenus(SiteInfo siteInfo, bool isSuperAdmin, List<int> siteIdListLatestAccessed, List<int> siteIdListWithPermissions)
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

                        siteMenus.Add(new Menu
                        {
                            Href = AdminUrl.GetIndexUrl(site.Id, string.Empty),
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
                        Children = siteMenus.ToArray()
                    });
                }

                var linkMenus = new List<Menu>
                {
                    new Menu {Href = PageUtility.GetSiteUrl(siteInfo, false), Target = "_blank", Text = "访问站点"},
                    new Menu {Href = ApiRoutePreview.GetSiteUrl(siteInfo.Id), Target = "_blank", Text = "预览站点"}
                };
                menus.Add(new Menu { Text = "站点链接", Children = linkMenus.ToArray() });
            }

            if (isSuperAdmin)
            {
                foreach (var tab in MenuManager.GetTopMenuTabs())
                {
                    var tabs = MenuManager.GetTabList(tab.Id, 0);
                    tab.Children = tabs.ToArray();

                    menus.Add(tab);
                }
            }

            return menus;
        }

        private static List<Menu> GetLeftMenus(SiteInfo siteInfo, string topId, bool isSuperAdmin, List<string> permissionList)
        {
            var menus = new List<Menu>();

            var tabs = MenuManager.GetTabList(topId, siteInfo.Id);
            foreach (var parent in tabs)
            {
                if (!isSuperAdmin && !MenuManager.IsValid(parent, permissionList)) continue;

                var children = new List<Menu>();
                if (parent.Children != null && parent.Children.Length > 0)
                {
                    var tabCollection = new MenuCollection(parent.Children);
                    if (tabCollection.Menus != null && tabCollection.Menus.Length > 0)
                    {
                        foreach (var childTab in tabCollection.Menus)
                        {
                            if (!isSuperAdmin && !MenuManager.IsValid(childTab, permissionList)) continue;

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
                }

                menus.Add(new Menu
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