using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Channels
{
    [RoutePrefix("pages/cms/channels/channelsLayerTaxis")]
    public partial class PagesChannelsLayerTaxisController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public async Task<List<int>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Channels))
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
                for (var num = 0; num < request.Taxis; num++)
                {
                    var channel = await DataProvider.ChannelRepository.GetAsync(channelId);
                    if (!expendedChannelIds.Contains(channel.ParentId))
                    {
                        expendedChannelIds.Add(channel.ParentId);
                    }
                    if (request.IsUp)
                    {
                        await DataProvider.ChannelRepository.UpdateTaxisUpAsync(request.SiteId, channelId, channel.ParentId, channel.Taxis);
                    }
                    else
                    {
                        await DataProvider.ChannelRepository.UpdateTaxisDownAsync(request.SiteId, channelId, channel.ParentId, channel.Taxis);
                    }
                }

                await auth.AddSiteLogAsync(request.SiteId, channelId, 0, "栏目排序" + (request.IsUp ? "上升" : "下降"), $"栏目:{DataProvider.ChannelRepository.GetChannelNameAsync(request.SiteId, channelId).GetAwaiter().GetResult()}");
            }

            return expendedChannelIds;
        }
    }
}
