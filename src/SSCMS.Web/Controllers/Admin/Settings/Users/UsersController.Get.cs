using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResults>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var groups = await _userGroupRepository.GetUserGroupsAsync();

            var count = await _userRepository.GetCountAsync(request.State, request.GroupId, request.LastActivityDate, request.Keyword);
            var users = await _userRepository.GetUsersAsync(request.State, request.GroupId, request.LastActivityDate, request.Keyword, request.Order, request.Offset, request.Limit);

            return new GetResults
            {
                Users = users,
                Count = count,
                Groups = groups
            };
        }
    }
}
