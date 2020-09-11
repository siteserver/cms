using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsLayerTaxisController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            if (channel.DefaultTaxisType == TaxisType.OrderByTaxis)
            {
                request.IsUp = !request.IsUp;
            }

            if (request.IsUp == false)
            {
                request.ContentIds.Reverse();
            }

            foreach (var contentId in request.ContentIds)
            {
                var contentInfo = await _contentRepository.GetAsync(site, channel, contentId);
                if (contentInfo == null) continue;

                var isTop = contentInfo.Top;
                for (var i = 1; i <= request.Taxis; i++)
                {
                    if (request.IsUp)
                    {
                        if (await _contentRepository.SetTaxisToUpAsync(site, channel, contentId, isTop) == false)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (await _contentRepository.SetTaxisToDownAsync(site, channel, contentId, isTop) == false)
                        {
                            break;
                        }
                    }
                }
            }

            await _createManager.TriggerContentChangedEventAsync(request.SiteId, request.ChannelId);

            await _authManager.AddSiteLogAsync(request.SiteId, request.ChannelId, 0, "对内容排序", string.Empty);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
