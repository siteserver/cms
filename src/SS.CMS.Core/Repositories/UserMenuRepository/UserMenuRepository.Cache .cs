using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace SS.CMS.Core.Repositories
{
    public partial class UserMenuRepository
    {
        public async Task<List<UserMenu>> GetAllUserMenusAsync()
        {
            return await _cache.GetOrCreateAsync(_cacheKey, async options =>
            {
                return await GetUserMenuInfoListToCacheAsync();
            });
        }

        public async Task<UserMenu> GetUserMenuInfoAsync(int menuId)
        {
            var list = await GetAllUserMenusAsync();
            return list.FirstOrDefault(menu => menu.Id == menuId);
        }

        private const string Dashboard = nameof(Dashboard);
        private const string ContentAdd = nameof(ContentAdd);
        private const string Contents = nameof(Contents);
        private const string Return = nameof(Return);

        private readonly Lazy<List<KeyValuePair<UserMenu, List<UserMenu>>>> SystemMenus =
            new Lazy<List<KeyValuePair<UserMenu, List<UserMenu>>>>(() =>
                new List<KeyValuePair<UserMenu, List<UserMenu>>>
                {
                    new KeyValuePair<UserMenu, List<UserMenu>>(new UserMenu
                    {
                        Id = 0,
                        SystemId = Dashboard,
                        GroupIdCollection = string.Empty,
                        IsDisabled = false,
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
                        GroupIdCollection = string.Empty,
                        IsDisabled = false,
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
                        GroupIdCollection = string.Empty,
                        IsDisabled = false,
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
                        GroupIdCollection = string.Empty,
                        IsDisabled = false,
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
