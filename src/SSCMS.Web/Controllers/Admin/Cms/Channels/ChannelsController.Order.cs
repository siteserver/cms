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

            await _channelRepository.UpdateTaxisAsync(request.SiteId, request.ParentId, request.ChannelId, request.IsUp);

            return new List<int>
            {
                request.SiteId,
                request.ChannelId
            };
        }
    }
}
