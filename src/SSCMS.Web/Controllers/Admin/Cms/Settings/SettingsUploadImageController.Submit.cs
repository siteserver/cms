using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsUploadImageController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.SettingsUploadImage))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            site.ImageUploadDirectoryName = request.ImageUploadDirectoryName;
            site.ImageUploadDateFormatString = request.ImageUploadDateFormatString;
            site.IsImageUploadChangeFileName = request.IsImageUploadChangeFileName;
            site.ImageUploadExtensions = request.ImageUploadExtensions.Replace("|", ",");
            site.ImageUploadTypeMaxSize = request.ImageUploadTypeMaxSize;
            site.PhotoSmallWidth = request.PhotoSmallWidth;
            site.PhotoMiddleWidth = request.PhotoMiddleWidth;

            await _siteRepository.UpdateAsync(site);

            await _authManager.AddSiteLogAsync(request.SiteId, "修改图片上传设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}