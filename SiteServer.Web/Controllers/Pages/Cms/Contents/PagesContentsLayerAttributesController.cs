using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    
    [RoutePrefix("pages/cms/contentsLayerAttributes")]
    public class PagesContentsLayerAttributesController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var siteId = request.GetPostInt("siteId");
                var channelContentIds =
                    MinContentInfo.ParseMinContentInfoList(request.GetPostString("channelContentIds"));
                var pageType = request.GetPostString("pageType");
                var isRecommend = request.GetPostBool("isRecommend");
                var isHot = request.GetPostBool("isHot");
                var isColor = request.GetPostBool("isColor");
                var isTop = request.GetPostBool("isTop");
                var hits = request.GetPostInt("hits");

                if (!request.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                if (pageType == "setAttributes")
                {
                    if (isRecommend || isHot || isColor || isTop)
                    {
                        foreach (var channelContentId in channelContentIds)
                        {
                            var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelContentId.ChannelId);
                            var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, channelContentId.Id);
                            if (contentInfo == null) continue;

                            if (isRecommend)
                            {
                                contentInfo.Recommend = true;
                            }
                            if (isHot)
                            {
                                contentInfo.Hot = true;
                            }
                            if (isColor)
                            {
                                contentInfo.Color = true;
                            }
                            if (isTop)
                            {
                                contentInfo.Top = true;
                            }
                            await DataProvider.ContentRepository.UpdateAsync(site, channelInfo, contentInfo);
                        }

                        await request.AddSiteLogAsync(siteId, "设置内容属性");
                    }
                }
                else if(pageType == "cancelAttributes")
                {
                    if (isRecommend || isHot || isColor || isTop)
                    {
                        foreach (var channelContentId in channelContentIds)
                        {
                            var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelContentId.ChannelId);
                            var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, channelContentId.Id);
                            if (contentInfo == null) continue;

                            if (isRecommend)
                            {
                                contentInfo.Recommend = false;
                            }
                            if (isHot)
                            {
                                contentInfo.Hot = false;
                            }
                            if (isColor)
                            {
                                contentInfo.Color = false;
                            }
                            if (isTop)
                            {
                                contentInfo.Top = false;
                            }
                            await DataProvider.ContentRepository.UpdateAsync(site, channelInfo, contentInfo);
                        }

                        await request.AddSiteLogAsync(siteId, "取消内容属性");
                    }
                }
                else if (pageType == "setHits")
                {
                    foreach (var channelContentId in channelContentIds)
                    {
                        var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelContentId.ChannelId);
                        var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, channelContentId.Id);
                        if (contentInfo == null) continue;

                        contentInfo.Hits = hits;
                        await DataProvider.ContentRepository.UpdateAsync(site, channelInfo, contentInfo);
                    }

                    await request.AddSiteLogAsync(siteId, "设置内容点击量");
                }

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }
    }
}
