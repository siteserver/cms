using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class AudioController
    {
        [HttpGet, Route(RouteDownload)]
        public async Task<ActionResult> ActionsDownload([FromQuery] DownloadRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialAudio))
            {
                return Unauthorized();
            }

            var audio = await _materialAudioRepository.GetAsync(request.Id);
            if (audio == null || string.IsNullOrEmpty(audio.Url)) return this.Error(Constants.ErrorNotFound);
            var filePath = PathUtils.Combine(_settingsManager.WebRootPath, audio.Url);
            if (!FileUtils.IsFileExists(filePath)) return this.Error(Constants.ErrorNotFound);

            return this.Download(filePath);
        }
    }
}
