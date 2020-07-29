using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class FileController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.MaterialFile))
            {
                return Unauthorized();
            }

            await _materialFileRepository.DeleteAsync(request.Id);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
