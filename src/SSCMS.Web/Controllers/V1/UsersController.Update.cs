using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [OpenApiOperation("修改用户 API", "修改用户属性，使用POST发起请求，请求地址为/api/v1/users/{id}")]
        [HttpPost, Route(RouteUserUpdate)]
        public async Task<ActionResult<User>> Update([FromRoute]int id, [FromBody]User request)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers))
            {
                return Unauthorized();
            }
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            request.Id = id;

            var (success, errorMessage) = await _userRepository.UpdateAsync(request);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            return request;
        }
    }
}
