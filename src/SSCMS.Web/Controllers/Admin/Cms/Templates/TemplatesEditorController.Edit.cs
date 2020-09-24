using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesEditorController
	{
        [HttpPut, Route(Route)]
        public async Task<ActionResult<GetResult>> Edit([FromBody] Template request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            return await SaveAsync(site, request);
        }
    }
}
