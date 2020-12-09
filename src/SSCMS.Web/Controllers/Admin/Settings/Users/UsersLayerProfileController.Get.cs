using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersLayerProfileController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] int userId)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetByUserIdAsync(userId);
            var userStyles = await _tableStyleRepository.GetUserStylesAsync();
            var groups = await _userGroupRepository.GetUserGroupsAsync();
            var styles = userStyles.Select(x => new InputStyle(x));

            return new GetResult
            {
                User = user,
                Groups = groups,
                Styles = styles
            };
        }
    }
}
