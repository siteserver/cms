using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [OpenApiOperation("新增用户操作日志 API", "新增用户操作日志，使用POST发起请求，请求地址为/api/v1/users/{id}/logs")]
        [HttpPost, Route(RouteUserLogs)]
        public async Task<ActionResult<Log>> CreateLog([FromRoute] int id, [FromBody] Log log)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers))
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetByUserIdAsync(id);
            if (user == null) return NotFound();

            log.UserId = user.Id;
            await _logRepository.AddUserLogAsync(user, PageUtils.GetIpAddress(Request), log.Action, log.Summary);

            return log;
        }
    }
}
