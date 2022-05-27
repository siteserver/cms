using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class VideoController
    {
        [HttpGet, Route(RouteDownload)]
        public async Task<ActionResult> ActionsDownload([FromQuery] DownloadRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialVideo))
            {
                return Unauthorized();
            }

            var video = await _materialVideoRepository.GetAsync(request.Id);
            if (video == null || string.IsNullOrEmpty(video.Url)) return this.Error(Constants.ErrorNotFound);
            var filePath = PathUtils.Combine(_settingsManager.WebRootPath, video.Url);
            if (!FileUtils.IsFileExists(filePath)) return this.Error(Constants.ErrorNotFound);

            return this.Download(filePath);
        }
    }
}