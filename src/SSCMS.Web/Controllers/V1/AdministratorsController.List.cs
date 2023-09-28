using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class AdministratorsController
    {
        [OpenApiOperation("获取管理员列表 API", "获取管理员列表，使用GET发起请求，请求地址为/api/v1/administrators")]
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> List([FromQuery] ListRequest request)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeAdministrators))
            {
                return Unauthorized();
            }
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            var page = request.Page;
            if (page <= 0)
            {
                page = 1;
            }
            var perPage = request.Page;
            if (perPage <= 0)
            {
                perPage = 20;
            }
            var offset = (page - 1) * perPage;
            var limit = perPage;

            var administrators = await _administratorRepository.GetAdministratorsAsync(offset, limit);
            var count = await _administratorRepository.GetCountAsync();

            return new ListResult
            {
                Count = count,
                Administrators = administrators
            };
        }
    }
}
