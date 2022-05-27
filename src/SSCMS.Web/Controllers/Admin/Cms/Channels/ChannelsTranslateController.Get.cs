using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsTranslateController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.ChannelsTranslate))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                return new
                {
                    Count = count,
                    summary.IndexName
                };
            });

            var transSites = await _siteRepository.GetSelectsAsync();
            var transChannels = await _channelRepository.GetAsync(request.SiteId);
            var transCascade = await _channelRepository.GetCascadeAsync(site, transChannels, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);

                return new
                {
                    summary.IndexName,
                    Count = count
                };
            });
            var translateTypes = ListUtils.GetEnums<ChannelTranslateType>().Select(x => new Select<string>(x));

            return new GetResult
            {
                Channels = cascade,
                TransSites = transSites,
                TransChannels = transCascade,
                TranslateTypes = translateTypes
            };
        }
    }
}
