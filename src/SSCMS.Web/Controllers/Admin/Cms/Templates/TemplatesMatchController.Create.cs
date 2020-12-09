using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesMatchController
    {
        [HttpPost, Route(RouteCreate)]
        public async Task<ActionResult<GetResult>> Create([FromBody] CreateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.TemplatesMatch))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            if (request.IsChannelTemplate && request.IsChildren)
            {
                await CreateChannelChildrenTemplateAsync(site, request);
            }
            else if (request.IsChannelTemplate && !request.IsChildren)
            {
                await CreateChannelTemplateAsync(site, request);
            }
            else if (!request.IsChannelTemplate && request.IsChildren)
            {
                await CreateContentChildrenTemplateAsync(site, request);
            }
            else if (!request.IsChannelTemplate && !request.IsChildren)
            {
                await CreateContentTemplateAsync(site, request);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "生成并匹配栏目模版");

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                var entity = await _channelRepository.GetAsync(summary.Id);
                return new
                {
                    Count = count,
                    entity.ChannelTemplateId,
                    entity.ContentTemplateId
                };
            });

            var channelTemplates = await _templateRepository.GetTemplatesByTypeAsync(request.SiteId, TemplateType.ChannelTemplate);
            var contentTemplates = await _templateRepository.GetTemplatesByTypeAsync(request.SiteId, TemplateType.ContentTemplate);

            return new GetResult
            {
                Channels = cascade,
                ChannelTemplates = channelTemplates,
                ContentTemplates = contentTemplates
            };
        }
    }
}
