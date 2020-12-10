using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [OpenApiOperation("获取用户 API", "获取用户，使用GET发起请求，请求地址为/api/v1/users/{id}")]
        [HttpGet, Route(RouteUser)]
        public async Task<ActionResult<User>> Get([FromRoute] int id)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers) &&
                !(_authManager.IsUser && (id == 0 || id == _authManager.UserId)))
            {
                return Unauthorized();
            }

            if (_authManager.IsUser && id == 0)
            {
                id = _authManager.UserId;
            }
            else if (!await _userRepository.IsExistsAsync(id)) return NotFound();

            var user = await _userRepository.GetByUserIdAsync(id);

            return user;
        }
    }
}
