using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class VideoController
    {
        [HttpGet, Route(RouteActionsDownload)]
        public async Task<ActionResult> ActionsDownload([FromQuery] DownloadRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialVideo))
            {
                return Unauthorized();
            }

            var video = await _materialVideoRepository.GetAsync(request.Id);
            if (video == null || string.IsNullOrEmpty(video.Url)) return NotFound();
            var filePath = PathUtils.Combine(_settingsManager.WebRootPath, video.Url);
            if (!FileUtils.IsFileExists(filePath)) return NotFound();

            return this.Download(filePath);
        }
    }
}