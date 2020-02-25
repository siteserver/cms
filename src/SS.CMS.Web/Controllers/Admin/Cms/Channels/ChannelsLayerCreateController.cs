using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Cms.Channels
{
    [Route("admin/cms/channels/channelsLayerCreate")]
    public partial class ChannelsLayerCreateController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;

        public ChannelsLayerCreateController(IAuthManager authManager, ICreateManager createManager)
        {
            _authManager = authManager;
            _createManager = createManager;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<List<int>>> Create([FromBody] CreateRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var expendedChannelIds = new List<int>
            {
                request.SiteId
            };

            foreach (var channelId in request.ChannelIds)
            {
                var channel = await DataProvider.ChannelRepository.GetAsync(channelId);
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
                    var channelIds = await DataProvider.ChannelRepository.GetChannelIdsAsync(request.SiteId, channelId, ScopeType.Descendant);

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
