using System;
using System.Collections.Generic;
using SSCMS;

namespace SSCMS.Core.Repositories.UserMenuRepository
{
    public partial class UserMenuRepository
    {
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
