using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class ImageController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<MaterialImage>> Update([FromBody] UpdateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.MaterialImage))
            {
                return Unauthorized();
            }

            var lib = await _materialImageRepository.GetAsync(request.Id);
            lib.Title = request.Title;
            lib.GroupId = request.GroupId;
            await _materialImageRepository.UpdateAsync(lib);

            return lib;
        }
    }
}
