using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Core.Utils;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsUploadAudioController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.SettingsUploadAudio))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            var audioUploadExtensions = site.AudioUploadExtensions;
            if (_settingsManager.IsSafeMode)
            {
                audioUploadExtensions = Constants.DefaultAudioUploadExtensions;
            }
            var audioUploadTypeMaxSize = site.AudioUploadTypeMaxSize / 1024;

            return new GetResult
            {
                CSRFToken = _authManager.GetCSRFToken(),
                IsSafeMode = _settingsManager.IsSafeMode,
                AudioUploadDirectoryName = site.AudioUploadDirectoryName,
                AudioUploadDateFormatString = site.AudioUploadDateFormatString,
                IsAudioUploadChangeFileName = site.IsAudioUploadChangeFileName,
                AudioUploadExtensions = audioUploadExtensions,
                AudioUploadTypeMaxSize = audioUploadTypeMaxSize,
            };
        }
    }
}