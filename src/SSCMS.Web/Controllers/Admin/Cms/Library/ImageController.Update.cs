using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Library
{
    public partial class ImageController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<LibraryImage>> Update([FromBody] UpdateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryImage))
            {
                return Unauthorized();
            }

            var lib = await _libraryImageRepository.GetAsync(request.Id);
            lib.Title = request.Title;
            lib.GroupId = request.GroupId;
            await _libraryImageRepository.UpdateAsync(lib);

            return lib;
        }
    }
}
