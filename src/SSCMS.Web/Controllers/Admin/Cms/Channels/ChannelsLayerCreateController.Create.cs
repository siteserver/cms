using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsLayerCreateController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<List<int>>> Create([FromBody] CreateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var expendedChannelIds = new List<int>
            {
                request.SiteId
            };

            foreach (var channelId in request.ChannelIds)
            {
                var channel = await _channelRepository.GetAsync(channelId);
                if (!expendedChannelIds.Contains(channel.ParentId))
                {
                    expendedChannelIds.Add(channel.ParentId);
                }

                await _createManager.CreateChannelAsync(request.SiteId, channelId);
                if (request.IsCreateContents)
                {
                    await _createManager.CreateAllContentAsync(request.SiteId, channelId);
                }
                if (request.IsIncludeChildren)
                {
                    var channelIds = await _channelRepository.GetChannelIdsAsync(request.SiteId, channelId, ScopeType.Descendant);

                    foreach (var childChannelId in channelIds)
                    {
                        await _createManager.CreateChannelAsync(request.SiteId, childChannelId);
                        if (request.IsCreateContents)
                        {
                            await _createManager.CreateAllContentAsync(request.SiteId, channelId);
                        }
                    }
                }
            }

            return expendedChannelIds;
        }
    }
}