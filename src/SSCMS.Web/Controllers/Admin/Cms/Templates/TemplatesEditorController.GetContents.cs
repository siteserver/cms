using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesEditorController
	{
        [HttpPost, Route(RouteGetContents)]
        public async Task<ActionResult<GetContentsResult>> GetContents([FromBody] GetContentsRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();
            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var contentId = 0;
            var contents = new List<KeyValuePair<int, string>>();
            var contentIds = await _contentRepository.GetContentIdsCheckedAsync(site, channel);
            foreach (var id in contentIds.Take(30))
            {
                if (contentId == 0)
                {
                    contentId = id;
                }

                var content = await _contentRepository.GetAsync(site, channel, id);
                contents.Add(new KeyValuePair<int, string>(content.Id, content.Title));
            }

            return new GetContentsResult
            {
                ContentId = contentId,
                Contents = contents
            };
        }
    }
}
