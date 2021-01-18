using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsController
    {
        [HttpPost, Route(RouteWidth)]
        public async Task<ActionResult<BoolResult>> Width([FromBody] WidthRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (!string.IsNullOrEmpty(request.PrevAttributeName) && request.PrevWidth > 0)
            {
                var prevName = ColumnsManager.GetColumnWidthName(request.PrevAttributeName);
                channel.Set(prevName, request.PrevWidth);
            }
            if (!string.IsNullOrEmpty(request.NextAttributeName) && request.NextWidth > 0)
            {
                var nextName = ColumnsManager.GetColumnWidthName(request.NextAttributeName);
                channel.Set(nextName, request.NextWidth);
            }
            
            await _channelRepository.UpdateAsync(channel);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
