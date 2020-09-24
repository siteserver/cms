using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersProfileController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] int userId)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetByUserIdAsync(userId);
            var userStyles = await _tableStyleRepository.GetUserStylesAsync();
            var styles = userStyles.Select(x => new InputStyle(x));

            return new GetResult
            {
                User = user,
                Styles = styles
            };
        }
    }
}
