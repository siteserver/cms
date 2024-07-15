using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromBody] GetRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var allGroups = await _userGroupRepository.GetUserGroupsAsync(true);

            var departmentId = request.DepartmentId > 0 ? request.DepartmentId : -1;
            var count = await _userRepository.GetCountAsync(request.State, null, departmentId, request.GroupId, request.LastActivityDate, request.Keyword);
            var offset = request.PageSize * (request.Page - 1);
            var users = await _userRepository.GetUsersAsync(request.State, null, departmentId, request.GroupId, request.LastActivityDate, request.Keyword, request.Order, offset, request.PageSize);

            foreach (var user in users)
            {
                var groups = await _usersInGroupsRepository.GetGroupsAsync(user);
                user.DepartmentName = await _departmentRepository.GetFullNameAsync(user.DepartmentId);
                user.Set("groups", groups);
            }

            return new GetResult
            {
                Users = users,
                Total = count,
                Groups = allGroups,
            };
        }
    }
}
