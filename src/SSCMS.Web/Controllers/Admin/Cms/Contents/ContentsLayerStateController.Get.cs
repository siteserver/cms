using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerStateController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.View))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var content = await _contentRepository.GetAsync(site, channel, request.ContentId);
            if (content == null) return NotFound();

            var contentChecks = await _contentCheckRepository.GetCheckListAsync(content.SiteId, content.ChannelId, request.ContentId);
            contentChecks.ForEach(async x =>
            {
                x.Set("State", CheckManager.GetCheckState(site, x.Checked, x.CheckedLevel));
                x.Set("AdminName", await _administratorRepository.GetDisplayAsync(x.AdminId));
            });

            return new GetResult
            {
                ContentChecks = contentChecks,
                Content = content,
                State = CheckManager.GetCheckState(site, content)
            };
        }
    }
}