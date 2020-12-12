using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsLayerDeleteController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);

            var contents = new List<Content>();
            foreach (var summary in summaries)
            {
                var channel = await _channelRepository.GetAsync(summary.ChannelId);
                var content = await _contentRepository.GetAsync(site, channel, summary.Id);
                if (content == null || content.UserId != _authManager.UserId) continue;

                var pageContent = content.Clone<Content>();
                pageContent.Set(ColumnsManager.CheckState, CheckManager.GetCheckState(site, content));
                contents.Add(pageContent);
            }

            return new GetResult
            {
                Contents = contents
            };
        }
    }
}
