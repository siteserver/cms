using System;
using System.Collections.Generic;
using System.Linq;
using SS.CMS.Abstractions.Models;
using SS.CMS.Core.Cache.Core;

namespace SS.CMS.Core.Repositories
{
    public partial class UserMenuRepository
    {
        private readonly object _lockObject = new object();

        public void ClearCache()
        {
            DataCacheManager.Remove(CacheKey);
        }

        public List<UserMenuInfo> GetAllUserMenus()
        {
            var retval = DataCacheManager.Get<List<UserMenuInfo>>(CacheKey);
            if (retval != null) return retval;

            lock (_lockObject)
            {
                retval = DataCacheManager.Get<List<UserMenuInfo>>(CacheKey);
                if (retval == null)
                {
                    retval = GetUserMenuInfoListToCache();

                    DataCacheManager.Insert(CacheKey, retval);
                }
            }

            return retval;
        }

        public UserMenuInfo GetUserMenuInfo(int menuId)
        {
            var list = GetAllUserMenus();
            return list.FirstOrDefault(menu => menu.Id == menuId);
        }

        private const string Dashboard = nameof(Dashboard);
        private const string ContentAdd = nameof(ContentAdd);
        private const string Contents = nameof(Contents);
        private const string Return = nameof(Return);

        public readonly Lazy<List<KeyValuePair<UserMenuInfo, List<UserMenuInfo>>>> SystemMenus =
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
