using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsLayerCopyController
    {
        [HttpGet, Route(RouteGetChannels)]
        public async Task<ActionResult<GetChannelsResult>> GetChannels([FromQuery] SiteRequest request)
        {
            var channels = new List<object>();
            var channelIdList = await _authManager.GetChannelIdsAsync(request.SiteId,
                Types.ContentPermissions.Add);
            foreach (var permissionChannelId in channelIdList)
            {
                var permissionChannelInfo = await _channelRepository.GetAsync(permissionChannelId);
                channels.Add(new
                {
                    permissionChannelInfo.Id,
                    ChannelName = await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, permissionChannelId)
                });
            }

            return new GetChannelsResult
            {
                Channels = channels
            };
        }
    }
}
