using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [HttpDelete, Route(RouteUser)]
        public async Task<ActionResult<User>> Delete(int id)
        {
            var isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers) ||
                         _authManager.IsUser &&
                         _authManager.UserId == id ||
                         _authManager.IsAdmin &&
                         await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUsers);
            if (!isAuth) return Unauthorized();

            var user = await _userRepository.DeleteAsync(id);

            return user;
        }
    }
}
