using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [HttpGet, Route(RouteUser)]
        public async Task<ActionResult<User>> Get(int id)
        {
            var isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers) ||
                         _authManager.IsUser &&
                         _authManager.UserId == id ||
                         _authManager.IsAdmin &&
                         await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUsers);
            if (!isAuth) return Unauthorized();

            if (!await _userRepository.IsExistsAsync(id)) return NotFound();

            var user = await _userRepository.GetByUserIdAsync(id);

            return user;
        }
    }
}
