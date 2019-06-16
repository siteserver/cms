using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public class MenuManager : IMenuManager
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IPluginManager _pluginManager;
        private readonly IPathManager _pathManager;
        private readonly IUrlManager _urlManager;

        public MenuManager(ISettingsManager settingsManager, IPluginManager pluginManager, IPathManager pathManager, IUrlManager urlManager)
        {
            _settingsManager = settingsManager;
            _pluginManager = pluginManager;
            _pathManager = pathManager;
            _urlManager = urlManager;
        }

        // public string GetTopMenuName(string menuId)
        // {
        //     var retval = string.Empty;
        //     if (menuId == Constants.TopMenu.IdSite)
        //     {
        //         retval = "站点管理";
        //     }
        //     else if (menuId == Constants.TopMenu.IdPlugins)
        //     {
        //         retval = "插件管理";
        //     }
        //     else if (menuId == Constants.TopMenu.IdSettings)
        //     {
        //         retval = "系统管理";
        //     }
        //     return retval;
        // }

        // public string GetLeftMenuName(string menuId)
        // {
        //     var retval = string.Empty;
        //     if (menuId == Constants.LeftMenu.IdContent)
        //     {
        //         retval = "信息管理";
        //     }
        //     else if (menuId == Constants.LeftMenu.IdTemplate)
        //     {
        //         retval = "显示管理";
        //     }
        //     else if (menuId == Constants.LeftMenu.IdConfigration)
        //     {
        //         retval = "设置管理";
        //     }
        //     else if (menuId == Constants.LeftMenu.IdCreate)
        //     {
        //         retval = "生成管理";
        //     }
        //     return retval;
        // }

        public IList<Menu> GetTabs(string filePath)
        {
            if (filePath.StartsWith("/") || filePath.StartsWith("~"))
            {
                filePath = _pathManager.MapContentRootPath(filePath);
            }

            var tc = CacheUtils.Get(filePath) as List<Menu>;
            if (tc != null) return tc;

            tc = FileUtils.XmlFileToObject<List<Menu>>(filePath);
            CacheUtils.Insert(filePath, tc, filePath);
            return tc;
        }

        public IList<Menu> GetTopMenuTabs()
        {
            return _settingsManager.NavSettings.Menus;
            // var list = new List<Menu>();

            // var menuPath = _pathManager.GetMenusPath("Top.config");
            // if (!FileUtils.IsFileExists(menuPath)) return list;

            // var tabs = GetTabs(menuPath);
            // foreach (var parent in tabs)
            // {
            //     list.Add(parent);
            // }

            // return list;
        }

        public bool IsValid(Menu menu, IList<string> permissionList)
        {
            var permissions = menu.Permissions;
            if (permissions != null && permissions.Count > 0)
            {
                if (permissionList != null && permissionList.Count > 0)
                {
                    foreach (var tabPermission in permissions)
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

        private Menu GetPluginTab(Abstractions.Menu menu, IList<string> permissions)
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

        public IList<Menu> GetTabList(string topId, int siteId)
        {
            IList<Menu> tabs = new List<Menu>();

            if (!string.IsNullOrEmpty(topId))
            {
                var topMenu = _settingsManager.NavSettings.Menus.FirstOrDefault(x => x.Id == topId);
                if (topMenu != null)
                {
                    tabs = topMenu.Menus;
                }
            }

            var menus = new List<Menu>();
            if (siteId > 0 && topId == string.Empty)
            {
                var siteMenus = _pluginManager.GetSiteMenus(_urlManager, siteId);
                if (siteMenus != null)
                {
                    menus.AddRange(siteMenus);
                }
            }
            else if (topId == "Plugins")
            {
                var topMenus = _pluginManager.GetTopMenus(_urlManager);
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

                tabs.Add(GetPluginTab(menu, new List<string> { menu.PluginId }));

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