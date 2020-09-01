using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [OpenApiOperation("获取用户操作日志 API", "获取用户操作日志列表，使用GET发起请求，请求地址为/api/v1/users/{id}/logs")]
        [HttpGet, Route(RouteUserLogs)]
        public async Task<ActionResult<GetLogsResult>> GetLogs([FromRoute] int id, [FromQuery]GetLogsRequest request)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers))
            {
                return Unauthorized();
            }

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
