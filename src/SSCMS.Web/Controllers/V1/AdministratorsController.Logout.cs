using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.V1
{
    public partial class AdministratorsController
    {
        [OpenApiOperation("管理员退出登录 API", "退出登录，使用POST发起请求，请求地址为/api/v1/administrators/actions/logout，此接口可以直接访问，无需通过身份验证。")]
        [HttpPost, Route(RouteActionsLogout)]
        public async Task<ActionResult<BoolResult>> Logout()
        {
            var cacheKey = Constants.GetSessionIdCacheKey(_authManager.AdminId);
            await _dbCacheRepository.RemoveAsync(cacheKey);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
