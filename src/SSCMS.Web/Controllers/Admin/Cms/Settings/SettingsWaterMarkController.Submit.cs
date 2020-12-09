using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsWaterMarkController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.SettingsWaterMark))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            site.IsWaterMark = request.IsWaterMark;
            site.WaterMarkPosition = request.WaterMarkPosition;
            site.WaterMarkTransparency = request.WaterMarkTransparency;
            site.WaterMarkMinWidth = request.WaterMarkMinWidth;
            site.WaterMarkMinHeight = request.WaterMarkMinHeight;
            site.IsImageWaterMark = request.IsImageWaterMark;
            site.WaterMarkFormatString = request.WaterMarkFormatString;
            site.WaterMarkFontName = request.WaterMarkFontName;
            site.WaterMarkFontSize = request.WaterMarkFontSize;
            site.WaterMarkImagePath = _pathManager.GetVirtualUrl(site, request.WaterMarkImagePath);

            await _siteRepository.UpdateAsync(site);

            await _authManager.AddSiteLogAsync(request.SiteId, "修改图片水印设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}