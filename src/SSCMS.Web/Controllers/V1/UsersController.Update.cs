using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Extensions;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [HttpPut, Route(RouteUser)]
        public async Task<ActionResult<User>> Update([FromRoute]int id, [FromBody]User request)
        {
            var isAuth = await
                             _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers) ||
                         _authManager.IsUser &&
                         _authManager.UserId == id ||
                         _authManager.IsAdmin &&
                         await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUsers);
            if (!isAuth) return Unauthorized();

            var (success, errorMessage) = await _userRepository.UpdateAsync(request);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            return request;
        }
    }
}
