using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ChannelsResult>> List([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    Types.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                var groupNames = await _channelRepository.GetGroupNamesAsync(summary.Id);
                return new
                {
                    Count = count,
                    summary.IndexName,
                    GroupNames = groupNames,
                    summary.Taxis,
                    summary.ParentId
                };
            });

            var indexNames = await _channelRepository.GetChannelIndexNamesAsync(request.SiteId);
            var groupNameList = await _channelGroupRepository.GetGroupNamesAsync(request.SiteId);

            var channelTemplates = await _templateRepository.GetTemplatesByTypeAsync(request.SiteId, TemplateType.ChannelTemplate);
            var contentTemplates = await _templateRepository.GetTemplatesByTypeAsync(request.SiteId, TemplateType.ContentTemplate);

            return new ChannelsResult
            {
                Channel = cascade,
                IndexNames = indexNames,
                GroupNames = groupNameList,
                ChannelTemplates = channelTemplates,
                ContentTemplates = contentTemplates
            };
        }
    }
}
