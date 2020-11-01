using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesEditorController
	{
        [HttpPost, Route(RoutePreview)]
        public async Task<ActionResult<PreviewResult>> Preview([FromBody] PreviewRequest request)
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

            var baseUrl = string.Empty;
            if (template.TemplateType == TemplateType.IndexPageTemplate)
            {
                baseUrl = await _pathManager.GetIndexPageUrlAsync(site, false);
            }
            else if (template.TemplateType == TemplateType.ChannelTemplate)
            {
                var channel = await _channelRepository.GetAsync(request.ChannelId);
                baseUrl = await _pathManager.GetChannelUrlAsync(site, channel, false);
            }
            else if (template.TemplateType == TemplateType.ContentTemplate)
            {
                var content = await _contentRepository.GetAsync(site, request.ChannelId, request.ContentId);
                baseUrl = await _pathManager.GetContentUrlByIdAsync(site, content, false);
            }
            else if (template.TemplateType == TemplateType.FileTemplate)
            {
                baseUrl = await _pathManager.GetFileUrlAsync(site, template.Id, false);
            }

            return new PreviewResult
            {
                BaseUrl = baseUrl,
                Html = parsedContent
            };
        }
    }
}
