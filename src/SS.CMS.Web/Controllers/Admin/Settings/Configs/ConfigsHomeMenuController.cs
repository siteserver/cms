using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Settings.Configs
{
    [Route("admin/settings/configsHomeMenu")]
    public partial class ConfigsHomeMenuController : ControllerBase
    {
        private const string Route = "";
        private const string RouteReset = "actions/reset";

        private readonly IAuthManager _authManager;

        public ConfigsHomeMenuController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigsHomeMenu))
            {
                return Unauthorized();
            }

            return new GetResult
            {
                UserMenus = await DataProvider.UserMenuRepository.GetUserMenuListAsync(),
                Groups = await DataProvider.UserGroupRepository.GetUserGroupListAsync()
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<UserMenusResult>> Delete([FromBody]IdRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigsHomeMenu))
            {
                return Unauthorized();
            }

            await DataProvider.UserMenuRepository.DeleteAsync(request.Id);

            return new UserMenusResult
            {
                UserMenus = await DataProvider.UserMenuRepository.GetUserMenuListAsync()
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<UserMenusResult>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigsHomeMenu))
            {
                return Unauthorized();
            }

            if (request.Id == 0)
            {
                await DataProvider.UserMenuRepository.InsertAsync(new UserMenu
                {
                    IsGroup = request.IsGroup,
                    GroupIds = request.GroupIds,
                    Disabled = request.Disabled,
                    ParentId = request.ParentId,
                    Taxis = request.Taxis,
                    Text = request.Text,
                    IconClass = request.IconClass,
                    Href = request.Href,
                    Target = request.Target
                });

                await auth.AddAdminLogAsync("新增用户菜单", $"用户菜单:{request.Text}");
            }
            else if (request.Id > 0)
            {
                var userMenu = await DataProvider.UserMenuRepository.GetAsync(request.Id);
                userMenu.IsGroup = request.IsGroup;
                userMenu.GroupIds = request.GroupIds;
                userMenu.Disabled = request.Disabled;
                userMenu.ParentId = request.ParentId;
                userMenu.Taxis = request.Taxis;
                userMenu.Text = request.Text;
                userMenu.IconClass = request.IconClass;
                userMenu.Href = request.Href;
                userMenu.Target = request.Target;
                await DataProvider.UserMenuRepository.UpdateAsync(userMenu);

                await auth.AddAdminLogAsync("修改用户菜单", $"用户菜单:{request.Text}");
            }

            return new UserMenusResult
            {
                UserMenus = await DataProvider.UserMenuRepository.GetUserMenuListAsync()
            };
        }

        [HttpPost, Route(RouteReset)]
        public async Task<ActionResult<UserMenusResult>> Reset()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigsHomeMenu))
            {
                return Unauthorized();
            }

            foreach (var userMenuInfo in await DataProvider.UserMenuRepository.GetUserMenuListAsync())
            {
                await DataProvider.UserMenuRepository.DeleteAsync(userMenuInfo.Id);
            }

            await auth.AddAdminLogAsync("重置用户菜单");

            return new UserMenusResult
            {
                UserMenus = await DataProvider.UserMenuRepository.GetUserMenuListAsync()
            };
        }
    }
}
