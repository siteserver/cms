using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersGroupController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<GetResult>> Delete([FromBody] IdRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUsersGroup))
            {
                return Unauthorized();
            }

            await _userGroupRepository.DeleteAsync(request.Id);

            return new GetResult
            {
                Groups = await _userGroupRepository.GetUserGroupsAsync()
            };
        }
    }
}