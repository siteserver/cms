using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Framework;

namespace SiteServer.API.Controllers.Pages.Cms.Channels
{
    [RoutePrefix("pages/cms/channels/channelsLayerCreate")]
    public partial class PagesChannelsLayerCreateController : ApiController
    {
        private const string Route = "";

        private readonly ICreateManager _createManager;

        public PagesChannelsLayerCreateController(ICreateManager createManager)
        {
            _createManager = createManager;
        }

        [HttpPost, Route(Route)]
        public async Task<List<int>> Create([FromBody] CreateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Channels))
            {
                return Request.Unauthorized<List<int>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<List<int>>();

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
