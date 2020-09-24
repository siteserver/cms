using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesEditorController
	{
        [HttpPost, Route(RouteCreate)]
        public async Task<ActionResult<BoolResult>> Create([FromBody] TemplateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            await _createManager.CreateByTemplateAsync(request.SiteId, request.TemplateId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
