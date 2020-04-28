using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Configs
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ConfigsHomeMenuController : ControllerBase
    {
        private const string Route = "settings/configsHomeMenu";
        private const string RouteReset = "settings/configsHomeMenu/actions/reset";

        private readonly IAuthManager _authManager;
        private readonly IUserMenuRepository _userMenuRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public ConfigsHomeMenuController(IAuthManager authManager, IUserMenuRepository userMenuRepository, IUserGroupRepository userGroupRepository)
        {
            _authManager = authManager;
            _userMenuRepository = userMenuRepository;
            _userGroupRepository = userGroupRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsConfigsHomeMenu))
            {
                return Unauthorized();
            }

            return new GetResult
            {
                UserMenus = await _userMenuRepository.GetUserMenusAsync(),
                Groups = await _userGroupRepository.GetUserGroupListAsync()
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<UserMenusResult>> Delete([FromBody]IdRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsConfigsHomeMenu))
            {
                return Unauthorized();
            }

            await _userMenuRepository.DeleteAsync(request.Id);

            return new UserMenusResult
            {
                UserMenus = await _userMenuRepository.GetUserMenusAsync()
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<UserMenusResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsConfigsHomeMenu))
            {
                return Unauthorized();
            }

            if (request.Id == 0)
            {
                await _userMenuRepository.InsertAsync(new UserMenu
                {
                    IsGroup = request.IsGroup,
                    GroupIds = request.GroupIds,
                    Disabled = request.Disabled,
                    ParentId = request.ParentId,
                    Taxis = request.Taxis,
                    Text = request.Text,
                    IconClass = request.IconClass,
                    Link = request.Link,
                    Target = request.Target
                });

                await _authManager.AddAdminLogAsync("新增用户菜单", $"用户菜单:{request.Text}");
            }
            else if (request.Id > 0)
            {
                var userMenu = await _userMenuRepository.GetAsync(request.Id);
                userMenu.IsGroup = request.IsGroup;
                userMenu.GroupIds = request.GroupIds;
                userMenu.Disabled = request.Disabled;
                userMenu.ParentId = request.ParentId;
                userMenu.Taxis = request.Taxis;
                userMenu.Text = request.Text;
                userMenu.IconClass = request.IconClass;
                userMenu.Link = request.Link;
                userMenu.Target = request.Target;
                await _userMenuRepository.UpdateAsync(userMenu);

                await _authManager.AddAdminLogAsync("修改用户菜单", $"用户菜单:{request.Text}");
            }

            return new UserMenusResult
            {
                UserMenus = await _userMenuRepository.GetUserMenusAsync()
            };
        }

        [HttpPost, Route(RouteReset)]
        public async Task<ActionResult<UserMenusResult>> Reset()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsConfigsHomeMenu))
            {
                return Unauthorized();
            }

            await _userMenuRepository.ResetAsync();
            await _authManager.AddAdminLogAsync("重置用户菜单");

            return new UserMenusResult
            {
                UserMenus = await _userMenuRepository.GetUserMenusAsync()
            };
        }
    }
}
