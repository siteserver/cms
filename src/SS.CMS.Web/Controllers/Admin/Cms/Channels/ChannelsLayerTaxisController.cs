using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Cms.Channels
{
    [Route("admin/cms/channels/channelsLayerTaxis")]
    public partial class ChannelsLayerTaxisController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public ChannelsLayerTaxisController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<List<int>>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Channels))
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

                await auth.AddSiteLogAsync(request.SiteId, channelId, 0, "栏目排序" + (request.IsUp ? "上升" : "下降"), $"栏目:{DataProvider.ChannelRepository.GetChannelNameAsync(request.SiteId, channelId)}");
            }

            return expendedChannelIds;
        }
    }
}
