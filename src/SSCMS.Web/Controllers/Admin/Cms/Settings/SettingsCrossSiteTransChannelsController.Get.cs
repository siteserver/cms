using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCrossSiteTransChannelsController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.SettingsCrossSiteTransChannels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var translates = await _translateRepository.GetTranslatesAsync(channel.SiteId, true);
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                var entity = await _channelRepository.GetAsync(summary.Id);

                var channelTranslates = translates.Where(x => x.ChannelId == summary.Id && !string.IsNullOrEmpty(x.Summary)).ToList();

                return new
                {
                    entity.IndexName,
                    Count = count,
                    Translates = channelTranslates
                };
            });

            return new GetResult
            {
                Channels = cascade
            };
        }
    }
}