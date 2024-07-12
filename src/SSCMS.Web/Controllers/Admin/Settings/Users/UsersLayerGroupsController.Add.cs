using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersLayerGroupsController
    {
        [HttpPost, Route(RouteAdd)]
        public async Task<ActionResult<BoolResult>> Add([FromBody] AddRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            if (await _userGroupRepository.IsExistsAsync(request.GroupName))
            {
                return this.Error("保存失败，已存在相同名称的用户组！");
            }

            var groupId = await _userGroupRepository.InsertAsync(new UserGroup
            {
                GroupName  = request.GroupName,
                Description = request.Description,
            });

            var userIds = ListUtils.GetIntList(request.UserIds);
            foreach (var userId in userIds)
            {
                await _usersInGroupsRepository.InsertIfNotExistsAsync(groupId, userId);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}