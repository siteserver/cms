using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class ImageController
    {
        [HttpGet, Route(RouteActionsDownload)]
        public async Task<ActionResult> ActionsDownload([FromQuery] DownloadRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialImage))
            {
                return Unauthorized();
            }

            var image = await _materialImageRepository.GetAsync(request.Id);
            if (image == null || string.IsNullOrEmpty(image.Url)) return NotFound();
            var filePath = PathUtils.Combine(_settingsManager.WebRootPath, image.Url);
            if (!FileUtils.IsFileExists(filePath)) return NotFound();

            return this.Download(filePath);
        }
    }
}
