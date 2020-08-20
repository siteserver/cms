using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class VideoController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    Types.SitePermissions.MaterialVideo))
            {
                return Unauthorized();
            }

            await _materialVideoRepository.DeleteAsync(request.Id);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}