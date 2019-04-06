using System;
using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Caches.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.CMS.Caches
{
	public static class UserMenuManager
	{
	    private static class UserMenuManagerCache
        {
	        private static readonly object LockObject = new object();

	        private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(UserMenuManager));

	        public static void Clear()
	        {
	            DataCacheManager.Remove(CacheKey);
	        }

            public static List<UserMenuInfo> GetAllUserMenus()
	        {
	            var retval = DataCacheManager.Get<List<UserMenuInfo>>(CacheKey);
	            if (retval != null) return retval;

	            lock (LockObject)
	            {
	                retval = DataCacheManager.Get<List<UserMenuInfo>>(CacheKey);
	                if (retval == null)
	                {
	                    retval = DataProvider.UserMenu.GetUserMenuInfoList();

	                    DataCacheManager.Insert(CacheKey, retval);
	                }
	            }

	            return retval;
	        }
	    }

	    public static void ClearCache()
	    {
	        UserMenuManagerCache.Clear();
	    }

        public static UserMenuInfo GetUserMenuInfo(int menuId)
	    {
	        var list = UserMenuManagerCache.GetAllUserMenus();
	        return list.FirstOrDefault(menu => menu.Id == menuId);
	    }

	    public static List<UserMenuInfo> GetAllUserMenuInfoList()
	    {
	        return UserMenuManagerCache.GetAllUserMenus();
	    }

	    private const string Dashboard = nameof(Dashboard);
	    private const string ContentAdd = nameof(ContentAdd);
	    private const string Contents = nameof(Contents);
	    private const string Return = nameof(Return);

        public static readonly Lazy<List<KeyValuePair<UserMenuInfo, List<UserMenuInfo>>>> SystemMenus =
	        new Lazy<List<KeyValuePair<UserMenuInfo, List<UserMenuInfo>>>>(() =>
	            new List<KeyValuePair<UserMenuInfo, List<UserMenuInfo>>>
	            {
	                new KeyValuePair<UserMenuInfo, List<UserMenuInfo>>(new UserMenuInfo
	                {
	                    Id = 0,
	                    SystemId = Dashboard,
	                    GroupIdCollection = string.Empty,
	                    Disabled = false,
	                    ParentId = 0,
	                    Taxis = 1,
	                    Text = "用户中心",
	                    IconClass = "fa fa-home",
                        Href = "index.html",
	                    Target = "_top"
	                }, null),
	                new KeyValuePair<UserMenuInfo, List<UserMenuInfo>>(new UserMenuInfo
	                {
	                    Id = 0,
	                    SystemId = ContentAdd,
	                    GroupIdCollection = string.Empty,
	                    Disabled = false,
	                    ParentId = 0,
	                    Taxis = 2,
	                    Text = "新增稿件",
	                    IconClass = "fa fa-plus",
	                    Href = "pages/contentAdd.html",
	                    Target = "_self"
                    }, null),
	                new KeyValuePair<UserMenuInfo, List<UserMenuInfo>>(new UserMenuInfo
	                {
	                    Id = 0,
	                    SystemId = Contents,
	                    GroupIdCollection = string.Empty,
	                    Disabled = false,
	                    ParentId = 0,
	                    Taxis = 3,
	                    Text = "稿件管理",
	                    IconClass = "fa fa-list",
	                    Href = "pages/contents.html",
	                    Target = "_self"
                    }, null),
	                new KeyValuePair<UserMenuInfo, List<UserMenuInfo>>(new UserMenuInfo
	                {
	                    Id = 0,
	                    SystemId = Return,
	                    GroupIdCollection = string.Empty,
	                    Disabled = false,
	                    ParentId = 0,
	                    Taxis = 4,
	                    Text = "返回网站",
	                    IconClass = "fa fa-arrow-left",
	                    Href = "/",
	                    Target = "_top"
	                }, null)
                });
	}
}
