using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsWaterMarkController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.SettingsWaterMark))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var families = FontUtils.GetFontFamilies();
            var imageUrl = await _pathManager.ParseSiteUrlAsync(site, site.WaterMarkImagePath, true);

            return new GetResult
            {
                Site = site,
                Families = families,
                ImageUrl = imageUrl
            };
        }
    }
}