using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class IndexController : ControllerBase
    {
        private const string Route = "index";

        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;
        private readonly IUserMenuRepository _userMenuRepository;
        private readonly IUsersInGroupsRepository _usersInGroupsRepository;

        public IndexController(
            IAuthManager authManager,
            IConfigRepository configRepository,
            IUserMenuRepository userMenuRepository,
            IUsersInGroupsRepository usersInGroupsRepository
        )
        {
            _authManager = authManager;
            _configRepository = configRepository;
            _userMenuRepository = userMenuRepository;
            _usersInGroupsRepository  = usersInGroupsRepository;
        }

        public class GetResult
        {
            public User User { get; set; }
            public string HomeTitle { get; set; }
            public string HomeLogoUrl { get; set; }
            public List<Menu> Menus { get; set; }
        }

        private async Task<List<Menu>> GetMenusAsync(User user)
        {
            var userMenus = await _userMenuRepository.GetUserMenusAsync();
            var menus = new List<Menu>();
            var groups = await _usersInGroupsRepository.GetGroupsAsync(user);

            foreach (var menuInfo1 in userMenus)
            {
                if (menuInfo1.Disabled || menuInfo1.ParentId != 0) continue;
                if (menuInfo1.IsGroup)
                {
                    bool isGroup = false;
                    foreach (var group in groups)
                    {
                        if (ListUtils.Contains(menuInfo1.GroupIds, group.Id))
                        {
                            isGroup = true;
                            break;
                        }
                    }
                    if (!isGroup) continue;
                }
                var children = new List<Menu>();
                foreach (var menuInfo2 in userMenus)
                {
                    if (menuInfo2.Disabled || menuInfo2.ParentId != menuInfo1.Id) continue;
                    if (menuInfo2.IsGroup)
                    {
                        bool isGroup = false;
                        foreach (var group in groups)
                        {
                            if (ListUtils.Contains(menuInfo2.GroupIds, group.Id))
                            {
                                isGroup = true;
                                break;
                            }
                        }
                        if (!isGroup) continue;
                    }

                    children.Add(new Menu
                    {
                        Id = menuInfo2.Id.ToString(),
                        Text = menuInfo2.Text,
                        IconClass = menuInfo2.IconClass,
                        Link = menuInfo2.Link,
                        Target = menuInfo2.Target,
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

            return menus;
        }
    }
}
