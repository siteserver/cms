using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesEditorController
	{
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Default([FromQuery] TemplateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            Template template;
            if (request.TemplateId > 0)
            {
                template = await _templateRepository.GetAsync(request.TemplateId);
            }
            else
            {
                template = new Template
                {
                    TemplateType = request.TemplateType
                };
            }

            return new GetResult
            {
                Template = await GetTemplateResultAsync(template, site)
            };
        }
    }
}
