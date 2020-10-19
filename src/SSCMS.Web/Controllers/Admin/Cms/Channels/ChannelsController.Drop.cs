using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsController
    {
        [HttpPost, Route(RouteDrop)]
        public async Task<ActionResult<BoolResult>> Drop([FromBody] DropRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            await _channelRepository.DropAsync(request.SiteId, request.SourceId, request.TargetId, request.DropType);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
