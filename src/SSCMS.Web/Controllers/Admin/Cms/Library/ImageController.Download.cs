using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Extensions;

namespace SSCMS.Web.Controllers.Admin.Cms.Library
{
    public partial class ImageController
    {
        [HttpGet, Route(RouteActionsDownload)]
        public async Task<ActionResult> ActionsDownload([FromQuery] DownloadRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryImage))
            {
                return Unauthorized();
            }

            var library = await _libraryImageRepository.GetAsync(request.Id);
            var filePath = _pathManager.GetLibraryFilePath(library.Url);
            return this.Download(filePath);
        }
    }
}
