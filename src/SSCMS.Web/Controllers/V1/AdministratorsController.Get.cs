using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.V1
{
    public partial class AdministratorsController
    {
        [OpenApiOperation("获取管理员 API", "获取管理员，使用GET发起请求，请求地址为/api/v1/administrators/{id}")]
        [HttpGet, Route(RouteAdministrator)]
        public async Task<ActionResult<Administrator>> Get([FromRoute] int id)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeAdministrators))
            {
                return Unauthorized();
            }

            if (!await _administratorRepository.IsExistsAsync(id)) return NotFound();

            var administrator = await _administratorRepository.GetByUserIdAsync(id);

            return administrator;
        }
    }
}
