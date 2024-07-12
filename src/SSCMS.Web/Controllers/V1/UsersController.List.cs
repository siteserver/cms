using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;

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
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var page = request.Page;
            if (page <= 0)
            {
                page = 1;
            }
            var perPage = request.PerPage;
            if (perPage <= 0)
            {
                perPage = 20;
            }
            var offset = (page - 1) * perPage;
            var limit = perPage;

            var users = await _userRepository.GetUsersAsync(null, -1, 0, null, null, offset, limit);
            var count = await _userRepository.GetCountAsync();

            return new ListResult
            {
                Count = count,
                Users = users
            };
        }
    }
}
