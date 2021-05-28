using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [OpenApiOperation("获取用户列表 API", "获取用户列表，使用GET发起请求，请求地址为/api/v1/users")]
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> List([FromQuery]ListRequest request)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers))
            {
                return Unauthorized();
            }

            var top = request.Top;
            if (top <= 0)
            {
                top = 20;
            }

            var skip = request.Skip;

            var users = await _userRepository.GetUsersAsync(null, -1, 0, null, null, skip, top);
            var count = await _userRepository.GetCountAsync();

            return new ListResult
            {
                Count = count,
                Users = users
            };
        }
    }
}
