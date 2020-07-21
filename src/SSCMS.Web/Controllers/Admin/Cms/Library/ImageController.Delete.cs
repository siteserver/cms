using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Library
{
    public partial class ImageController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryImage))
            {
                return Unauthorized();
            }

            var library = await _libraryImageRepository.GetAsync(request.Id);
            var filePath = _pathManager.GetLibraryFilePath(library.Url);
            FileUtils.DeleteFileIfExists(filePath);

            await _libraryImageRepository.DeleteAsync(request.Id);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
