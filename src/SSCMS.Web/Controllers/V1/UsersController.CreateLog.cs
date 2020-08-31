using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [HttpPost, Route(RouteUserLogs)]
        public async Task<ActionResult<Log>> CreateLog(int id, [FromBody] Log log)
        {
            var isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers) ||
                         _authManager.IsUser &&
                         _authManager.UserId == id ||
                         _authManager.IsAdmin &&
                         await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUsers);
            if (!isAuth) return Unauthorized();

            var user = await _userRepository.GetByUserIdAsync(id);
            if (user == null) return NotFound();

            log.UserId = user.Id;
            await _logRepository.AddUserLogAsync(user, log.Action, log.Summary);

            return log;
        }
    }
}
