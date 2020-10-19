using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesEditorController
	{
        [HttpPost, Route(RoutePreview)]
        public async Task<ActionResult<StringResult>> Preview([FromBody] PreviewRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var template = await _templateRepository.GetAsync(request.TemplateId);
            await _parseManager.InitAsync(site, request.ChannelId, request.ContentId, template);
            var parsedContent = await _parseManager.ParseTemplatePreviewAsync(request.Content);

            return new StringResult
            {
                Value = parsedContent
            };
        }
    }
}
