using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Create
{
    public partial class CreatePageController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var permission = string.Empty;
            if (request.Type == CreateType.Index)
            {
                permission = MenuUtils.SitePermissions.CreateIndex;
            }
            else if (request.Type == CreateType.Channel)
            {
                permission = MenuUtils.SitePermissions.CreateChannels;
            }
            else if (request.Type == CreateType.Content)
            {
                permission = MenuUtils.SitePermissions.CreateContents;
            }
            else if (request.Type == CreateType.All)
            {
                permission = MenuUtils.SitePermissions.CreateAll;
            }

            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, permission))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var allChannelIds = new List<int>();
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                allChannelIds.Add(summary.Id);
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
                AllChannelIds = allChannelIds,
                ChannelTemplates = channelTemplates,
                ContentTemplates = contentTemplates
            };
        }
    }
}