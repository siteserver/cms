using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [HttpGet, Route(RouteUserLogs)]
        public async Task<ActionResult<GetLogsResult>> GetLogs(int id, [FromQuery]GetLogsRequest request)
        {
            var isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers) ||
                         _authManager.IsUser &&
                         _authManager.UserId == id ||
                         _authManager.IsAdmin &&
                         await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUsers);
            if (!isAuth) return Unauthorized();

            var user = await _userRepository.GetByUserIdAsync(id);
            if (user == null) return NotFound();

            var top = request.Top;
            if (top <= 0)
            {
                top = 20;
            }
            var skip = request.Skip;

            var logs = await _logRepository.GetUserLogsAsync(user.Id, skip, top);

            return new GetLogsResult
            {
                Count = await _userRepository.GetCountAsync(),
                Logs = logs
            };
        }
    }
}
