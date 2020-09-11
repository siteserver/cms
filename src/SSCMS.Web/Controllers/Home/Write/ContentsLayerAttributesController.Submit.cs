using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsLayerAttributesController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return NotFound();

            if (request.PageType == "setAttributes")
            {
                if (request.IsRecommend || request.IsHot || request.IsColor || request.IsTop)
                {
                    foreach (var contentId in request.ContentIds)
                    {
                        var contentInfo = await _contentRepository.GetAsync(site, channelInfo, contentId);
                        if (contentInfo == null) continue;

                        if (request.IsRecommend)
                        {
                            contentInfo.Recommend = true;
                        }
                        if (request.IsHot)
                        {
                            contentInfo.Hot = true;
                        }
                        if (request.IsColor)
                        {
                            contentInfo.Color = true;
                        }
                        if (request.IsTop)
                        {
                            contentInfo.Top = true;
                        }
                        await _contentRepository.UpdateAsync(site, channelInfo, contentInfo);
                    }

                    await _authManager.AddSiteLogAsync(request.SiteId, "设置内容属性");
                }
            }
            else if (request.PageType == "cancelAttributes")
            {
                if (request.IsRecommend || request.IsHot || request.IsColor || request.IsTop)
                {
                    foreach (var contentId in request.ContentIds)
                    {
                        var contentInfo = await _contentRepository.GetAsync(site, channelInfo, contentId);
                        if (contentInfo == null) continue;

                        if (request.IsRecommend)
                        {
                            contentInfo.Recommend = false;
                        }
                        if (request.IsHot)
                        {
                            contentInfo.Hot = false;
                        }
                        if (request.IsColor)
                        {
                            contentInfo.Color = false;
                        }
                        if (request.IsTop)
                        {
                            contentInfo.Top = false;
                        }
                        await _contentRepository.UpdateAsync(site, channelInfo, contentInfo);
                    }

                    await _authManager.AddSiteLogAsync(request.SiteId, "取消内容属性");
                }
            }
            else if (request.PageType == "setHits")
            {
                foreach (var contentId in request.ContentIds)
                {
                    var contentInfo = await _contentRepository.GetAsync(site, channelInfo, contentId);
                    if (contentInfo == null) continue;

                    contentInfo.Hits = request.Hits;
                    await _contentRepository.UpdateAsync(site, channelInfo, contentInfo);
                }

                await _authManager.AddSiteLogAsync(request.SiteId, "设置内容点击量");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
