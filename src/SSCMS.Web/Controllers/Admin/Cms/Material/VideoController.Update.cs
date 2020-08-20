using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class VideoController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<MaterialVideo>> Update([FromBody] UpdateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.MaterialVideo))
            {
                return Unauthorized();
            }

            var lib = await _materialVideoRepository.GetAsync(request.Id);
            lib.Title = request.Title;
            lib.GroupId = request.GroupId;
            await _materialVideoRepository.UpdateAsync(lib);

            return lib;
        }
    }
}