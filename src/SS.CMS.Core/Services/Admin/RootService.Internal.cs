using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SS.CMS.Core.Api.Preview;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Common.Create;
using SS.CMS.Core.Models;
using SS.CMS.Core.Packaging;
using SS.CMS.Core.Plugin;
using SS.CMS.Core.Plugin.Impl;
using SS.CMS.Core.Settings.Menus;
using SS.CMS.Core.StlParser;
using SS.CMS.Plugin;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services.Admin
{
    public partial class RootService
    {
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
                            Href = AdminUrl.GetIndexUrl(site.Id, string.Empty),
                            Target = "_top",
                            Text = site.SiteName
                        });
                    }
                    siteMenus.Add(new Tab
                    {
                        //Href = ModalSiteSelect.GetRedirectUrl(siteInfo.Id),
                        Href = PageUtils.UnclickedUrl,
                        Target = "_layer",
                        Text = "全部站点..."
                    });
                    menus.Add(new Tab
                    {
                        Text = siteInfo.SiteName,
                        //Href = ModalSiteSelect.GetRedirectUrl(siteInfo.Id),
                        Href = PageUtils.UnclickedUrl,
                        Target = "_layer",
                        Children = siteMenus.ToArray()
                    });
                }

                var linkMenus = new List<Tab>
                {
                    new Tab {Href = PageUtility.GetSiteUrl(siteInfo, false), Target = "_blank", Text = "访问站点"},
                    new Tab {Href = ApiRoutePreview.GetSiteUrl(siteInfo.Id), Target = "_blank", Text = "预览站点"}
                };
                menus.Add(new Tab { Text = "站点链接", Children = linkMenus.ToArray() });
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

        private static List<Tab> GetLeftMenus(SiteInfo siteInfo, string topId, bool isSuperAdmin, List<string> permissionList)
        {
            var menus = new List<Tab>();

            var tabs = MenuManager.GetTabList(topId, siteInfo.Id);
            foreach (var parent in tabs)
            {
                if (!isSuperAdmin && !MenuManager.IsValid(parent, permissionList)) continue;

                var children = new List<Tab>();
                if (parent.Children != null && parent.Children.Length > 0)
                {
                    var tabCollection = new TabCollection(parent.Children);
                    if (tabCollection.Tabs != null && tabCollection.Tabs.Length > 0)
                    {
                        foreach (var childTab in tabCollection.Tabs)
                        {
                            if (!isSuperAdmin && !MenuManager.IsValid(childTab, permissionList)) continue;

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
    }
}