using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsUploadImageController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.SettingsUploadImage))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            var imageUploadExtensions = site.ImageUploadExtensions;
            if (_settingsManager.IsSafeMode)
            {
                imageUploadExtensions = Constants.DefaultImageUploadExtensions;
            }
            var imageUploadTypeMaxSize = site.ImageUploadTypeMaxSize / 1024;

            return new GetResult
            {
                CSRFToken = _authManager.GetCSRFToken(),
                IsSafeMode = _settingsManager.IsSafeMode,
                ImageUploadDirectoryName = site.ImageUploadDirectoryName,
                ImageUploadDateFormatString = site.ImageUploadDateFormatString,
                IsImageUploadChangeFileName = site.IsImageUploadChangeFileName,
                ImageUploadExtensions = imageUploadExtensions,
                ImageUploadTypeMaxSize = imageUploadTypeMaxSize,
                IsImageAutoResize = site.IsImageAutoResize,
                ImageAutoResizeWidth = site.ImageAutoResizeWidth,
            };
        }
    }
}