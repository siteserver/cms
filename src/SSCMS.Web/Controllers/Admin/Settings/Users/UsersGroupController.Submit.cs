using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersGroupController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<GetResult>> Submit([FromBody] UserGroup request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsersGroup))
            {
                return Unauthorized();
            }

            if (request.Id == -1)
            {
                if (await _userGroupRepository.IsExistsAsync(request.GroupName))
                {
                    return this.Error("保存失败，已存在相同名称的用户组！");
                }

                var groupInfo = new UserGroup
                {
                    GroupName = request.GroupName,
                    AdminName = request.AdminName
                };

                await _userGroupRepository.InsertAsync(groupInfo);

                await _authManager.AddAdminLogAsync("新增用户组", $"用户组:{groupInfo.GroupName}");
            }
            else if (request.Id == 0)
            {
                var config = await _configRepository.GetAsync();

                config.UserDefaultGroupAdminName = request.AdminName;

                await _configRepository.UpdateAsync(config);
                await _userGroupRepository.ClearCache();

                await _authManager.AddAdminLogAsync("修改用户组", "用户组:默认用户组");
            }
            else if (request.Id > 0)
            {
                var groupInfo = await _userGroupRepository.GetUserGroupAsync(request.Id);

                if (groupInfo.GroupName != request.GroupName && await _userGroupRepository.IsExistsAsync(request.GroupName))
                {
                    return this.Error("保存失败，已存在相同名称的用户组！");
                }

                groupInfo.GroupName = request.GroupName;
                groupInfo.AdminName = request.AdminName;

                await _userGroupRepository.UpdateAsync(groupInfo);

                await _authManager.AddAdminLogAsync("修改用户组", $"用户组:{groupInfo.GroupName}");
            }

            return new GetResult
            {
                Groups = await _userGroupRepository.GetUserGroupsAsync()
            };
        }
    }
}