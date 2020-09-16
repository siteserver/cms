using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class IndexController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var config = await _configRepository.GetAsync();
            if (config.IsHomeClosed) return this.Error("对不起，用户中心已被禁用！");

            var menus = new List<Menu>();
            var user = await _authManager.GetUserAsync();
            var userMenus = await _userMenuRepository.GetUserMenusAsync();

            foreach (var menuInfo1 in userMenus)
            {
                var groupIds = menuInfo1.GroupIds ?? new List<int>();
                if (menuInfo1.Disabled || menuInfo1.ParentId != 0 ||
                    groupIds.Contains(user.GroupId)) continue;
                var children = new List<Menu>();
                foreach (var menuInfo2 in userMenus)
                {
                    var groupIds2 = menuInfo2.GroupIds ?? new List<int>();
                    if (menuInfo2.Disabled || menuInfo2.ParentId != menuInfo1.Id ||
                        groupIds2.Contains(user.GroupId)) continue;

                    children.Add(new Menu
                    {
                        Id = menuInfo2.Id.ToString(),
                        Text = menuInfo2.Text,
                        IconClass = menuInfo2.IconClass,
                        Link = menuInfo2.Link,
                        Target = menuInfo2.Target
                    });
                }

                menus.Add(new Menu
                {
                    Id = menuInfo1.Id.ToString(),
                    Text = menuInfo1.Text,
                    IconClass = menuInfo1.IconClass,
                    Link = menuInfo1.Link,
                    Target = menuInfo1.Target,
                    Children = children
                });
            }

            return new GetResult
            {
                User = user,
                HomeTitle = config.HomeTitle,
                HomeLogoUrl = config.HomeLogoUrl,
                Menus = menus
            };
        }
    }
}
