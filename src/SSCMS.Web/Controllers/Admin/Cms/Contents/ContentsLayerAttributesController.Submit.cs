using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerAttributesController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);

            foreach (var summary in summaries)
            {
                var channel = await _channelRepository.GetAsync(summary.ChannelId);
                var content = await _contentRepository.GetAsync(site, channel, summary.Id);
                if (content == null) continue;

                if (request.IsTop)
                {
                    content.Top = !request.IsCancel;
                }
                if (request.IsRecommend)
                {
                    content.Recommend = !request.IsCancel;
                }
                if (request.IsHot)
                {
                    content.Hot = !request.IsCancel;
                }
                if (request.IsColor)
                {
                    content.Color = !request.IsCancel;
                }

                await _contentRepository.UpdateAsync(site, channel, content);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, request.IsCancel ? "取消内容属性" : "设置内容属性");

            //else if (pageType == "setHits")
            //{
            //    foreach (var channelContentId in channelContentIds)
            //    {
            //        var channelInfo = await _channelRepository.GetAsync(channelContentId.ChannelId);
            //        var contentInfo = await _contentRepository.GetAsync(site, channelInfo, channelContentId.Id);
            //        if (contentInfo == null) continue;

            //        contentInfo.Hits = hits;
            //        await _contentRepository.UpdateAsync(site, channelInfo, contentInfo);
            //    }

            //    await request.AddSiteLogAsync(siteId, "设置内容点击量");
            //}

            return new BoolResult
            {
                Value = true
            };
        }
    }
}