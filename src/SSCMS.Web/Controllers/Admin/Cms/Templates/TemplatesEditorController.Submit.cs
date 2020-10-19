using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesEditorController
	{
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] PreviewRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var template = await _templateRepository.GetAsync(request.TemplateId);

            await _pathManager.WriteContentToTemplateFileAsync(site, template, request.Content, _authManager.AdminId);
            await CreatePagesAsync(template);

            await _authManager.AddSiteLogAsync(site.Id,
                $"修改{template.TemplateType.GetDisplayName()}",
                $"模板名称:{template.TemplateName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
