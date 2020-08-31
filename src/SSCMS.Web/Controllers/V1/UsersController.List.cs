using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> List([FromQuery]ListRequest request)
        {
            var isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers) ||
                         _authManager.IsAdmin &&
                         await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUsers);
            if (!isAuth) return Unauthorized();

            var top = request.Top;
            if (top <= 0)
            {
                top = 20;
            }

            var skip = request.Skip;

            var users = await _userRepository.GetUsersAsync(null, 0, 0, null, null, skip, top);
            var count = await _userRepository.GetCountAsync();

            return new ListResult
            {
                Count = count,
                Users = users
            };
        }
    }
}
