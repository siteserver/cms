using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersGroupController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUsersGroup))
            {
                return Unauthorized();
            }

            return new GetResult
            {
                Groups = await _userGroupRepository.GetUserGroupsAsync(),
                AdminNames = await _administratorRepository.GetUserNamesAsync()
            };
        }
    }
}