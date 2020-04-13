using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CacheManager.Core;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public class TabManager
    {
        private readonly ICacheManager<object> _cacheManager;
        private readonly IPathManager _pathManager;
        private readonly IOldPluginManager _pluginManager;

        public TabManager(ICacheManager<object> cacheManager, IPathManager pathManager, IOldPluginManager pluginManager)
        {
            _cacheManager = cacheManager;
            _pathManager = pathManager;
            _pluginManager = pluginManager;
        }

        public TabCollection GetTabs(string filePath)
	    {
	        var tc = _cacheManager.Get<TabCollection>(CacheUtils.GetPathKey(filePath));
	        if (tc != null) return tc;

	        tc = Serializer.ConvertFileToObject<TabCollection>(filePath);
	        CacheUtils.SetFileContent(_cacheManager, tc, filePath);
	        return tc;
	    }

        public List<Tab> GetTopMenuTabs()
        {
            var list = new List<Tab>();

            var menuPath = _pathManager.GetConfigPath("menus", "Top.config");
            if (!FileUtils.IsFileExists(menuPath)) return list;

            var tabs = GetTabs(menuPath);
            foreach (var parent in tabs.Tabs)
            {
                list.Add(parent);
            }

            return list;
        }

	    public List<Tab> GetTopMenuTabsWithChildren()
	    {
	        var list = new List<Tab>();

	        var menuPath = _pathManager.GetConfigPath("menus", "Top.config");
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

        public bool IsValid(Tab tab, IList permissionList)
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

        public async Task<List<Tab>> GetTabListAsync(string topId, int siteId)
        {
            var tabs = new List<Tab>();

            if (!string.IsNullOrEmpty(topId))
            {
                var filePath = _pathManager.GetConfigPath("menus", $"{topId}.config");
                var tabCollection = GetTabs(filePath);
                if (tabCollection?.Tabs != null)
                {
                    foreach (var tabCollectionTab in tabCollection.Tabs)
                    {
                        tabs.Add(tabCollectionTab.Clone());
                    }
                }
            }

            var menus = new List<Menu>();
            if (siteId > 0 && topId == string.Empty)
            {
                var siteMenus = await _pluginManager.GetSiteMenusAsync(siteId);
                if (siteMenus != null)
                {
                    menus.AddRange(siteMenus);
                }
            }
            else if (topId == "Plugins")
            {
                var topMenus = await _pluginManager.GetTopMenusAsync();
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

                tabs.Add(_pluginManager.GetPluginTab(menu.PluginId, string.Empty, menu));

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
