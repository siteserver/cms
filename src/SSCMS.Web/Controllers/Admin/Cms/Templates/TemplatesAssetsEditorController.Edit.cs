using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesAssetsEditorController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<ContentResult>> Edit([FromBody] ContentRequest request)
        {
            if (request.FileType == "html")
            {
                if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.TemplatesIncludes))
                {
                    return Unauthorized();
                }
            }
            else
            {
                if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.TemplatesAssets))
                {
                    return Unauthorized();
                }
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            return await SaveFile(request, site, false);
        }
    }
}
