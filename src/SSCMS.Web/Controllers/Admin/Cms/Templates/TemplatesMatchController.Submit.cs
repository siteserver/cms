using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesMatchController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<ObjectResult<Cascade<int>>>> Submit([FromBody] MatchRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.TemplatesMatch))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            if (request.ChannelIds != null && request.ChannelIds.Count > 0)
            {
                if (request.IsChannelTemplate)
                {
                    foreach (var channelId in request.ChannelIds)
                    {
                        var channelInfo = await _channelRepository.GetAsync(channelId);
                        channelInfo.ChannelTemplateId = request.TemplateId;
                        await _channelRepository.UpdateChannelTemplateIdAsync(channelInfo);
                    }
                }
                else
                {
                    foreach (var channelId in request.ChannelIds)
                    {
                        var channelInfo = await _channelRepository.GetAsync(channelId);
                        channelInfo.ContentTemplateId = request.TemplateId;
                        await _channelRepository.UpdateContentTemplateIdAsync(channelInfo);
                    }
                }
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "模板匹配");

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

            return new ObjectResult<Cascade<int>>
            {
                Value = cascade
            };
        }
    }
}
