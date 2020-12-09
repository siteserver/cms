using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesSpecialController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> List([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.Specials))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var specials = await _specialRepository.GetSpecialsAsync(request.SiteId);

            return new ListResult
            {
                Specials = specials,
                SiteUrl = await _pathManager.GetSiteUrlAsync(site, true)
            };
        }
    }
}