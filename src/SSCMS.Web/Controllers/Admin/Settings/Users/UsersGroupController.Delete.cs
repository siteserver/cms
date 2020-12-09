using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersGroupController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<GetResult>> Delete([FromBody] IdRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsersGroup))
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