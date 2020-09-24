using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersLayerPasswordController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetByUserIdAsync(request.UserId);
            if (user == null) return NotFound();

            return new GetResult
            {
                User = user
            };
        }
    }
}
