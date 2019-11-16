using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.DataCache
{
	public static class UserMenuManager
	{
	    private static class UserMenuManagerCache
        {
	        private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(UserMenuManager));

	        public static void Clear()
	        {
	            DataCacheManager.Remove(CacheKey);
	        }

            public static async Task<List<UserMenu>> GetAllUserMenusAsync()
	        {
	            var retVal = DataCacheManager.Get<List<UserMenu>>(CacheKey);
	            if (retVal != null) return retVal;

                retVal = DataCacheManager.Get<List<UserMenu>>(CacheKey);
                if (retVal == null)
                {
                    retVal = await DataProvider.UserMenuDao.GetUserMenuListAsync();

                    DataCacheManager.Insert(CacheKey, retVal);
                }

                return retVal;
	        }
	    }

	    public static void ClearCache()
	    {
	        UserMenuManagerCache.Clear();
	    }

        public static async Task<UserMenu> GetUserMenuAsync(int menuId)
	    {
	        var list = await UserMenuManagerCache.GetAllUserMenusAsync();
	        return list.FirstOrDefault(menu => menu.Id == menuId);
	    }

	    public static async Task<List<UserMenu>> GetAllUserMenuListAsync()
	    {
	        return await UserMenuManagerCache.GetAllUserMenusAsync();
	    }

	    private const string Dashboard = nameof(Dashboard);
	    private const string ContentAdd = nameof(ContentAdd);
	    private const string Contents = nameof(Contents);
	    private const string Return = nameof(Return);

        public static readonly Lazy<List<KeyValuePair<UserMenu, List<UserMenu>>>> SystemMenus =
	        new Lazy<List<KeyValuePair<UserMenu, List<UserMenu>>>>(() =>
	            new List<KeyValuePair<UserMenu, List<UserMenu>>>
	            {
	                new KeyValuePair<UserMenu, List<UserMenu>>(new UserMenu
	                {
	                    Id = 0,
	                    SystemId = Dashboard,
	                    GroupIds = new List<int>(),
	                    Disabled = false,
	                    ParentId = 0,
	                    Taxis = 1,
	                    Text = "用户中心",
	                    IconClass = "fa fa-home",
                        Href = "index.html",
	                    Target = "_top"
	                }, null),
	                new KeyValuePair<UserMenu, List<UserMenu>>(new UserMenu
	                {
	                    Id = 0,
	                    SystemId = ContentAdd,
                        GroupIds = new List<int>(),
                        Disabled = false,
	                    ParentId = 0,
	                    Taxis = 2,
	                    Text = "新增稿件",
	                    IconClass = "fa fa-plus",
	                    Href = "pages/contentAdd.html",
	                    Target = "_self"
                    }, null),
	                new KeyValuePair<UserMenu, List<UserMenu>>(new UserMenu
	                {
	                    Id = 0,
	                    SystemId = Contents,
                        GroupIds = new List<int>(),
                        Disabled = false,
	                    ParentId = 0,
	                    Taxis = 3,
	                    Text = "稿件管理",
	                    IconClass = "fa fa-list",
	                    Href = "pages/contents.html",
	                    Target = "_self"
                    }, null),
	                new KeyValuePair<UserMenu, List<UserMenu>>(new UserMenu
	                {
	                    Id = 0,
	                    SystemId = Return,
                        GroupIds = new List<int>(),
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
