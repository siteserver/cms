using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsUploadVideoController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ObjectResult<Site>>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.SettingsUploadVideo))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            return new ObjectResult<Site>
            {
                Value = site
            };
        }
    }
}