using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsController
    {
        [HttpPost, Route(RouteOrder)]
        public async Task<ActionResult<List<int>>> Order([FromBody] OrderRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            if (request.IsUp)
            {
                await _channelRepository.UpdateTaxisUpAsync(request.SiteId, request.ChannelId, request.ParentId, request.Taxis);
            }
            else
            {
                await _channelRepository.UpdateTaxisDownAsync(request.SiteId, request.ChannelId, request.ParentId, request.Taxis);
            }

            return new List<int>
            {
                request.SiteId,
                request.ChannelId
            };
        }
    }
}
