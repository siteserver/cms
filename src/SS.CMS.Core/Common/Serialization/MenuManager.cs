using System.Collections;
using System.Collections.Generic;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Common.Serialization
{
    public class MenuManager
    {
        private readonly IPluginManager _pluginManager;
        private readonly IPathManager _pathManager;
        private readonly IUrlManager _urlManager;

        public MenuManager(IPluginManager pluginManager, IPathManager pathManager, IUrlManager urlManager)
        {
            _pluginManager = pluginManager;
            _pathManager = pathManager;
            _urlManager = urlManager;
        }

        public static class PluginsPermissions
        {
            public const string Add = "plugins_add";
            public const string Management = "plugins_management";
        }

        public static class SettingsPermissions
        {
            public const string SiteAdd = "settings_siteAdd";
            public const string Site = "settings_site";
            public const string Admin = "settings_admin";
            public const string User = "settings_user";
            public const string Chart = "settings_chart";
            public const string Log = "settings_log";
            public const string Utility = "settings_utility";
        }

        public static class WebSitePermissions
        {
            public const string Content = "cms_content";                            //信息管理
            public const string Template = "cms_template";                          //显示管理
            public const string Configration = "cms_configration";                  //设置管理
            public const string Create = "cms_create";                              //生成管理
        }

        public static class ChannelPermissions
        {
            public const string ContentView = "cms_contentView";
            public const string ContentAdd = "cms_contentAdd";
            public const string ContentEdit = "cms_contentEdit";
            public const string ContentDelete = "cms_contentDelete";
            public const string ContentTranslate = "cms_contentTranslate";
            public const string ContentOrder = "cms_contentOrder";
            public const string ChannelAdd = "cms_channelAdd";
            public const string ChannelEdit = "cms_channelEdit";
            public const string ChannelDelete = "cms_channelDelete";
            public const string ChannelTranslate = "cms_channelTranslate";
            public const string CreatePage = "cms_createPage";
            public const string ContentCheck = "cms_contentCheck";
            public const string ContentCheckLevel1 = "cms_contentCheckLevel1";
            public const string ContentCheckLevel2 = "cms_contentCheckLevel2";
            public const string ContentCheckLevel3 = "cms_contentCheckLevel3";
            public const string ContentCheckLevel4 = "cms_contentCheckLevel4";
            public const string ContentCheckLevel5 = "cms_contentCheckLevel5";
        }

        public static class TopMenu
        {
            public const string IdSite = "Site";
            public const string IdPlugins = "Plugins";
            public const string IdSettings = "Settings";
        }

        public static class LeftMenu
        {
            public const string IdContent = "Content";
            public const string IdTemplate = "Template";
            public const string IdConfigration = "Configration";
            public const string IdCreate = "Create";
        }

        public static string GetTopMenuName(string menuId)
        {
            var retval = string.Empty;
            if (menuId == TopMenu.IdSite)
            {
                retval = "站点管理";
            }
            else if (menuId == TopMenu.IdPlugins)
            {
                retval = "插件管理";
            }
            else if (menuId == TopMenu.IdSettings)
            {
                retval = "系统管理";
            }
            return retval;
        }

        public static string GetLeftMenuName(string menuId)
        {
            var retval = string.Empty;
            if (menuId == LeftMenu.IdContent)
            {
                retval = "信息管理";
            }
            else if (menuId == LeftMenu.IdTemplate)
            {
                retval = "显示管理";
            }
            else if (menuId == LeftMenu.IdConfigration)
            {
                retval = "设置管理";
            }
            else if (menuId == LeftMenu.IdCreate)
            {
                retval = "生成管理";
            }
            return retval;
        }

        public List<Menu> GetTabs(string filePath)
        {
            if (filePath.StartsWith("/") || filePath.StartsWith("~"))
            {
                filePath = _pathManager.MapContentRootPath(filePath);
            }

            var tc = CacheUtils.Get(filePath) as List<Menu>;
            if (tc != null) return tc;

            tc = (List<Menu>)Serializer.ConvertFileToObject(filePath, typeof(List<Menu>));
            CacheUtils.Insert(filePath, tc, filePath);
            return tc;
        }

        public List<Menu> GetTopMenuTabs()
        {
            var list = new List<Menu>();

            var menuPath = _pathManager.GetMenusPath("Top.config");
            if (!FileUtils.IsFileExists(menuPath)) return list;

            var tabs = GetTabs(menuPath);
            foreach (var parent in tabs)
            {
                list.Add(parent);
            }

            return list;
        }

        public List<Menu> GetTopMenuTabsWithChildren()
        {
            var list = new List<Menu>();

            var menuPath = _pathManager.GetMenusPath("Top.config");
            if (!FileUtils.IsFileExists(menuPath)) return list;

            var tabs = GetTabs(menuPath);
            foreach (var parent in tabs)
            {
                list.Add(parent);
            }

            return list;
        }

        public bool IsValid(Menu menu, IList permissionList)
        {
            if (menu.Permissions != null && menu.Permissions.Count > 0)
            {
                if (permissionList != null && permissionList.Count > 0)
                {
                    foreach (var tabPermission in menu.Permissions)
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

        private Menu GetPluginTab(Abstractions.Menu menu, string permission)
        {
            var tab = new Menu
            {
                Id = menu.Id,
                Text = menu.Text,
                IconClass = menu.IconClass,
                Selected = false,
                Href = menu.Href,
                Target = menu.Target,
                Permissions = TranslateUtils.StringCollectionToStringList(permission)
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

        public List<Menu> GetTabList(string topId, int siteId)
        {
            var tabs = new List<Menu>();

            if (!string.IsNullOrEmpty(topId))
            {
                var filePath = _pathManager.GetMenusPath($"{topId}.config");
                var tabCollection = GetTabs(filePath);
                if (tabCollection != null)
                {
                    foreach (var tabCollectionTab in tabCollection)
                    {
                        tabs.Add((Menu)tabCollectionTab.Clone());
                    }
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
