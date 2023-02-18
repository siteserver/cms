using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesMatchController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] MatchRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.TemplatesMatch))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            if (request.ChannelIds != null && request.ChannelIds.Count > 0)
            {
                if (request.IsChannelTemplate)
                {
                    foreach (var channelId in request.ChannelIds)
                    {
                        var channel = await _channelRepository.GetAsync(channelId);
                        channel.ChannelTemplateId = request.TemplateId;
                        await _channelRepository.UpdateChannelTemplateIdAsync(channel);
                    }
                }
                else
                {
                    foreach (var channelId in request.ChannelIds)
                    {
                        var channel = await _channelRepository.GetAsync(channelId);
                        channel.ContentTemplateId = request.TemplateId;
                        await _channelRepository.UpdateContentTemplateIdAsync(channel);
                    }
                }
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "匹配模板");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
