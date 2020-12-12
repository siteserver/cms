using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsLayerDeleteController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);

            if (summaries.Count == 1)
            {
                var summary = summaries[0];

                var content = await _contentRepository.GetAsync(site, summary.ChannelId, summary.Id);
                if (content != null)
                {
                    await _authManager.AddSiteLogAsync(request.SiteId, summary.ChannelId, summary.Id, "删除内容",
                        $"栏目:{await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, summary.ChannelId)},内容标题:{content.Title}");
                }
            }
            else
            {
                await _authManager.AddSiteLogAsync(request.SiteId, "批量删除内容",
                    $"栏目:{await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, request.ChannelId)},内容条数:{summaries.Count}");
            }

            await _authManager.AddUserLogAsync("批量删除内容", $"内容条数:{summaries.Count}");

            var adminId = _authManager.AdminId;
            foreach (var distinctChannelId in summaries.Select(x => x.ChannelId).Distinct())
            {
                var distinctChannel = await _channelRepository.GetAsync(distinctChannelId);
                var contentIdList = summaries.Where(x => x.ChannelId == distinctChannelId)
                    .Select(x => x.Id).ToList();

                var contentIds = new List<int>();
                foreach (var contentId in contentIdList)
                {
                    var content = await _contentRepository.GetAsync(site, distinctChannel, contentId);
                    if (content == null || content.UserId != _authManager.UserId) continue;

                    contentIds.Add(contentId);
                }

                await _contentRepository.TrashContentsAsync(site, distinctChannel, contentIds, adminId);

                await _createManager.TriggerContentChangedEventAsync(request.SiteId, distinctChannelId);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
