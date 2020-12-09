using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsController
    {
        [HttpPost, Route(RouteColumns)]
        public async Task<ActionResult<BoolResult>> Columns([FromBody] ColumnsRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            site.ChannelListColumns = request.AttributeNames;

            await _siteRepository.UpdateAsync(site);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
