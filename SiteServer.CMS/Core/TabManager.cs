using System.Collections;
using System.Collections.Generic;
using System.Web;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Core
{
    public static class TabManager
	{
	    public static TabCollection GetTabs(string filePath)
	    {
	        if (filePath.StartsWith("/") || filePath.StartsWith("~"))
	        {
	            filePath = HttpContext.Current.Server.MapPath(filePath);
	        }

	        var tc = CacheUtils.Get(filePath) as TabCollection;
	        if (tc != null) return tc;

	        tc = (TabCollection)Serializer.ConvertFileToObject(filePath, typeof(TabCollection));
	        CacheUtils.Insert(filePath, tc, filePath);
	        return tc;
	    }

        public static List<Tab> GetTopMenuTabs()
        {
            var list = new List<Tab>();

            var menuPath = PathUtils.GetMenusPath("Top.config");
            if (!FileUtils.IsFileExists(menuPath)) return list;

            var tabs = GetTabs(menuPath);
            foreach (var parent in tabs.Tabs)
            {
                list.Add(parent);
            }

            return list;
        }

	    public static List<Tab> GetTopMenuTabsWithChildren()
	    {
	        var list = new List<Tab>();

	        var menuPath = PathUtils.GetMenusPath("Top.config");
	        if (!FileUtils.IsFileExists(menuPath)) return list;

	        var tabs = GetTabs(menuPath);
	        foreach (var parent in tabs.Tabs)
	        {
	            if (parent.HasChildren)
	            {

	            }
	            list.Add(parent);
	        }

	        return list;
	    }

        public static bool IsValid(Tab tab, IList permissionList)
        {
            if (tab.HasPermissions)
            {
                if (permissionList != null && permissionList.Count > 0)
                {
                    var tabPermissions = tab.Permissions.Split(',');
                    foreach (var tabPermission in tabPermissions)
                    {
                        if (permissionList.Contains(tabPermission))
                            return true;
                    }
                }

                //ITab valid, but invalid role set
                return false;
            }

            //ITab valid, but no roles
            return true;
        }

        private static Tab GetPluginTab(SiteServer.Plugin.Menu menu, string permission)
        {
            var tab = new Tab
            {
                Id = menu.Id,
                Text = menu.Text,
                IconClass = menu.IconClass,
                Selected = false,
                Href = menu.Href,
                Target = menu.Target,
                Permissions = permission
            };
            if (menu.Menus != null && menu.Menus.Count > 0)
            {
                tab.Children = new Tab[menu.Menus.Count];
                for (var i = 0; i < menu.Menus.Count; i++)
                {
                    tab.Children[i] = GetPluginTab(menu.Menus[i], permission);
                }
            }
            return tab;
        }

        public static List<Tab> GetTabList(string topId, int siteId)
        {
            var tabs = new List<Tab>();

            if (!string.IsNullOrEmpty(topId))
            {
                var filePath = PathUtils.GetMenusPath($"{topId}.config");
                var tabCollection = GetTabs(filePath);
                if (tabCollection?.Tabs != null)
                {
                    foreach (var tabCollectionTab in tabCollection.Tabs)
                    {
                        tabs.Add(tabCollectionTab.Clone());
                    }
                }
            }

            var menus = new Dictionary<string, SiteServer.Plugin.Menu>();
            if (siteId > 0 && topId == string.Empty)
            {
                var siteMenus = PluginMenuManager.GetSiteMenus(siteId);
                if (siteMenus != null)
                {
                    foreach (var siteMenu in siteMenus)
                    {
                        menus.Add(siteMenu.Key, siteMenu.Value);
                    }
                }
            }
            else if (topId == "Plugins")
            {
                var topMenus = PluginMenuManager.GetTopMenus();
                if (topMenus != null)
                {
                    foreach (var topMenu in topMenus)
                    {
                        menus.Add(topMenu.Key, topMenu.Value);
                    }
                }
            }

            foreach (var pluginId in menus.Keys)
            {
                var menu = menus[pluginId];

                var isExists = false;
                foreach (var childTab in tabs)
                {
                    if (childTab.Id == menu.Id)
                    {
                        isExists = true;
                    }
                }

                if (isExists) continue;

                tabs.Add(GetPluginTab(menu, pluginId));

                //if (string.IsNullOrEmpty(menu.ParentId))
                //{
                //    var isExists = false;
                //    foreach (var childTab in tabs)
                //    {
                //        if (childTab.Id == menu.Id)
                //        {
                //            isExists = true;
                //        }
                //    }

                //    if (isExists) continue;

                //    tabs.Add(GetPluginTab(menu));
                //}
                //else
                //{
                //    foreach (var tab in tabs)
                //    {
                //        if (!StringUtils.EqualsIgnoreCase(menu.ParentId, tab.Id)) continue;

                //        var isExists = false;
                //        foreach (var childTab in tab.Children)
                //        {
                //            if (childTab.Id == menu.Id)
                //            {
                //                isExists = true;
                //            }
                //        }

                //        if (isExists) continue;

                //        var list = new List<Tab>();
                //        if (tab.Children != null)
                //        {
                //            list = tab.Children.ToList();
                //        }
                //        list.Add(GetPluginTab(menu));
                //        tab.Children = list.ToArray();
                //    }
                //}
            }

            return tabs;
        }
    }
}
