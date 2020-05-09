using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Extensions;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class IndexController : ControllerBase
    {
        private const string Route = "index";

        private readonly IAuthManager _authManager;
        private readonly IOldPluginManager _pluginManager;
        private readonly IConfigRepository _configRepository;
        private readonly IUserMenuRepository _userMenuRepository;

        public IndexController(IAuthManager authManager, IOldPluginManager pluginManager, IConfigRepository configRepository, IUserMenuRepository userMenuRepository)
        {
            _authManager = authManager;
            _pluginManager = pluginManager;
            _configRepository = configRepository;
            _userMenuRepository = userMenuRepository;
        }

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
                        Text = menuInfo2.Text,
                        IconClass = menuInfo2.IconClass,
                        Link = menuInfo2.Link,
                        Target = menuInfo2.Target
                    });
                }

                menus.Add(new Menu
                {
                    Text = menuInfo1.Text,
                    IconClass = menuInfo1.IconClass,
                    Link = menuInfo1.Link,
                    Target = menuInfo1.Target,
                    Children = children
                });
            }

            var defaultPageUrl = await _pluginManager.GetHomeDefaultPageUrlAsync();

            return new GetResult
            {
                User = user,
                HomeTitle = config.HomeTitle,
                HomeLogoUrl = config.HomeLogoUrl,
                Menus = menus,
                DefaultPageUrl = defaultPageUrl
            };
        }
    }
}
