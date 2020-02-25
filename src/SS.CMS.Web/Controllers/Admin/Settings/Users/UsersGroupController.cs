using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Framework;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Users
{
    [Route("admin/settings/usersGroup")]
    public partial class UsersGroupController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public UsersGroupController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUsersGroup))
            {
                return Unauthorized();
            }

            return new GetResult
            {
                Groups = await DataProvider.UserGroupRepository.GetUserGroupListAsync(),
                AdminNames = await DataProvider.AdministratorRepository.GetUserNameListAsync()
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<GetResult>> Delete([FromBody]IdRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUsersGroup))
            {
                return Unauthorized();
            }

            await DataProvider.UserGroupRepository.DeleteAsync(request.Id);

            return new GetResult
            {
                Groups = await DataProvider.UserGroupRepository.GetUserGroupListAsync()
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<GetResult>> Submit([FromBody] UserGroup request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUsersGroup))
            {
                return Unauthorized();
            }

            if (request.Id == -1)
            {
                if (await DataProvider.UserGroupRepository.IsExistsAsync(request.GroupName))
                {
                    return this.Error("保存失败，已存在相同名称的用户组！");
                }

                var groupInfo = new UserGroup
                {
                    GroupName = request.GroupName,
                    AdminName = request.AdminName
                };

                await DataProvider.UserGroupRepository.InsertAsync(groupInfo);

                await auth.AddAdminLogAsync("新增用户组", $"用户组:{groupInfo.GroupName}");
            }
            else if (request.Id == 0)
            {
                var config = await DataProvider.ConfigRepository.GetAsync();

                config.UserDefaultGroupAdminName = request.AdminName;

                await DataProvider.ConfigRepository.UpdateAsync(config);

                await auth.AddAdminLogAsync("修改用户组", "用户组:默认用户组");
            }
            else if (request.Id > 0)
            {
                var groupInfo = await DataProvider.UserGroupRepository.GetUserGroupAsync(request.Id);

                if (groupInfo.GroupName != request.GroupName && await DataProvider.UserGroupRepository.IsExistsAsync(request.GroupName))
                {
                    return this.Error("保存失败，已存在相同名称的用户组！");
                }

                groupInfo.GroupName = request.GroupName;
                groupInfo.AdminName = request.AdminName;

                await DataProvider.UserGroupRepository.UpdateAsync(groupInfo);

                await auth.AddAdminLogAsync("修改用户组", $"用户组:{groupInfo.GroupName}");
            }

            return new GetResult
            {
                Groups = await DataProvider.UserGroupRepository.GetUserGroupListAsync()
            };
        }
    }
}
