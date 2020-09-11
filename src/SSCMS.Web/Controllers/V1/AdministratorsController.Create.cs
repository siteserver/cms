using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class AdministratorsController
    {
        [OpenApiOperation("新增管理员 API", "注册新管理员，使用POST发起请求，请求地址为/api/v1/administrators")]
        [HttpPost, Route(Route)]
        public async Task<ActionResult<Administrator>> Create([FromBody] Administrator request)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeAdministrators))
            {
                return Unauthorized();
            }

            var (isValid, errorMessage) = await _administratorRepository.InsertAsync(request, request.Password);
            if (!isValid)
            {
                return this.Error(errorMessage);
            }

            return request;
        }
    }
}
