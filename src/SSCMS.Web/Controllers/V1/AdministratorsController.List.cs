using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;

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

            var top = request.Top;
            if (top <= 0) top = 20;
            var skip = request.Skip;

            var administrators = await _administratorRepository.GetAdministratorsAsync(skip, top);
            var count = await _administratorRepository.GetCountAsync();

            return new ListResult
            {
                Count = count,
                Administrators = administrators
            };
        }
    }
}
