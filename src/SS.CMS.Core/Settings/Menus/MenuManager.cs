using System.Collections;
using System.Collections.Generic;
using SS.CMS.Core.Common;
using SS.CMS.Core.Plugin;
using SS.CMS.Utils;

namespace SS.CMS.Core.Settings.Menus
{
    public static class MenuManager
    {
        public static MenuCollection GetTabs(string filePath)
        {
            if (filePath.StartsWith("/") || filePath.StartsWith("~"))
            {
                filePath = PathUtilsEx.MapContentRootPath(filePath);
            }

            var tc = CacheUtils.Get(filePath) as MenuCollection;
            if (tc != null) return tc;

            tc = (MenuCollection)Serializer.ConvertFileToObject(filePath, typeof(MenuCollection));
            CacheUtils.Insert(filePath, tc, filePath);
            return tc;
        }

        public static List<Menu> GetTopMenuTabs()
        {
            var list = new List<Menu>();

            var menuPath = AppContext.GetMenusPath("Top.config");
            if (!FileUtils.IsFileExists(menuPath)) return list;

            var tabs = GetTabs(menuPath);
            foreach (var parent in tabs.Menus)
            {
                list.Add(parent);
            }

            return list;
        }

        public static List<Menu> GetTopMenuTabsWithChildren()
        {
            var list = new List<Menu>();

            var menuPath = AppContext.GetMenusPath("Top.config");
            if (!FileUtils.IsFileExists(menuPath)) return list;

            var tabs = GetTabs(menuPath);
            foreach (var parent in tabs.Menus)
            {
                if (parent.HasChildren)
                {

                }
                list.Add(parent);
            }

            return list;
        }

        public static bool IsValid(Menu menu, IList permissionList)
        {
            if (menu.HasPermissions)
            {
                if (permissionList != null && permissionList.Count > 0)
                {
                    var tabPermissions = menu.Permissions.Split(',');
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

        private static Menu GetPluginTab(Abstractions.Menu menu, string permission)
        {
            var tab = new Menu
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
                tab.Children = new Menu[menu.Menus.Count];
                for (var i = 0; i < menu.Menus.Count; i++)
                {
                    tab.Children[i] = GetPluginTab(menu.Menus[i], permission);
                }
            }
            return tab;
        }

        public static List<Menu> GetTabList(string topId, int siteId)
        {
            var tabs = new List<Menu>();

            if (!string.IsNullOrEmpty(topId))
            {
                var filePath = AppContext.GetMenusPath($"{topId}.config");
                var tabCollection = GetTabs(filePath);
                if (tabCollection?.Menus != null)
                {
                    foreach (var tabCollectionTab in tabCollection.Menus)
                    {
                        tabs.Add(tabCollectionTab.Clone());
                    }
                }
            }

            var menus = new List<PluginMenu>();
            if (siteId > 0 && topId == string.Empty)
            {
                var siteMenus = PluginMenuManager.GetSiteMenus(siteId);
                if (siteMenus != null)
                {
                    menus.AddRange(siteMenus);
                }
            }
            else if (topId == "Plugins")
            {
                var topMenus = PluginMenuManager.GetTopMenus();
                if (topMenus != null)
                {
                    menus.AddRange(topMenus);
                }
            }

            foreach (var menu in menus)
            {
                var isExists = false;
                foreach (var childTab in tabs)
                {
                    if (childTab.Id == menu.Id)
                    {
                        isExists = true;
                    }
                }

                if (isExists) continue;

                tabs.Add(GetPluginTab(menu, menu.PluginId));

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
