using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.V1
{
    public partial class AdministratorsController
    {
        [OpenApiOperation("删除管理员 API", "删除管理员，使用DELETE发起请求，请求地址为/api/v1/administrators/{id}")]
        [HttpDelete, Route(RouteAdministrator)]
        public async Task<ActionResult<Administrator>> Delete([FromRoute]int id)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeAdministrators))
            {
                return Unauthorized();
            }

            if (!await _administratorRepository.IsExistsAsync(id)) return NotFound();

            var administrator = await _administratorRepository.DeleteAsync(id);

            return administrator;
        }

        
    }
}
