using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Models;
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

            return new GetResult
            {
                CSRFToken = _authManager.GetCSRFToken(),
                IsSafeMode = _settingsManager.IsSafeMode,
                ImageUploadDirectoryName = site.ImageUploadDirectoryName,
                ImageUploadDateFormatString = site.ImageUploadDateFormatString,
                IsImageUploadChangeFileName = site.IsImageUploadChangeFileName,
                ImageUploadExtensions = imageUploadExtensions,
                ImageUploadTypeMaxSize = site.ImageUploadTypeMaxSize,
                PhotoSmallWidth = site.PhotoSmallWidth,
                PhotoMiddleWidth = site.PhotoMiddleWidth,
            };
        }
    }
}