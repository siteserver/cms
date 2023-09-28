using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [OpenApiOperation("用户退出登录 API", "退出登录，使用POST发起请求，请求地址为/api/v1/users/actions/logout，此接口可以直接访问，无需通过身份验证。")]
        [HttpPost, Route(RouteActionsLogout)]
        public async Task<ActionResult<BoolResult>> Logout()
        {
            var cacheKey = Constants.GetSessionIdCacheKey(_authManager.UserId);
            await _dbCacheRepository.RemoveAsync(cacheKey);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
