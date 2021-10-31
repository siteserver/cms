using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Core.Utils;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsUploadVideoController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.SettingsUploadVideo))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            var videoUploadExtensions = site.VideoUploadExtensions;
            if (_settingsManager.IsSafeMode)
            {
                videoUploadExtensions = Constants.DefaultVideoUploadExtensions;
            }

            return new GetResult
            {

                CSRFToken = _authManager.GetCSRFToken(),
                IsSafeMode = _settingsManager.IsSafeMode,
                VideoUploadDirectoryName = site.VideoUploadDirectoryName,
                VideoUploadDateFormatString = site.VideoUploadDateFormatString,
                IsVideoUploadChangeFileName = site.IsVideoUploadChangeFileName,
                VideoUploadExtensions = videoUploadExtensions,
                VideoUploadTypeMaxSize = site.VideoUploadTypeMaxSize,
            };
        }
    }
}