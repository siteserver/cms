using System;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [OpenApiIgnore]
    [RoutePrefix("pages/cms/contentsLayerAttributes")]
    public class PagesContentsLayerAttributesController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = new AuthenticatedRequest();

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

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                if (pageType == "setAttributes")
                {
                    if (isRecommend || isHot || isColor || isTop)
                    {
                        foreach (var channelContentId in channelContentIds)
                        {
                            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelContentId.ChannelId);
                            var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, channelContentId.Id);
                            if (contentInfo == null) continue;

                            if (isRecommend)
                            {
                                contentInfo.IsRecommend = true;
                            }
                            if (isHot)
                            {
                                contentInfo.IsHot = true;
                            }
                            if (isColor)
                            {
                                contentInfo.IsColor = true;
                            }
                            if (isTop)
                            {
                                contentInfo.IsTop = true;
                            }
                            DataProvider.ContentDao.Update(siteInfo, channelInfo, contentInfo);
                        }

                        request.AddSiteLog(siteId, "设置内容属性");
                    }
                }
                else if(pageType == "cancelAttributes")
                {
                    if (isRecommend || isHot || isColor || isTop)
                    {
                        foreach (var channelContentId in channelContentIds)
                        {
                            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelContentId.ChannelId);
                            var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, channelContentId.Id);
                            if (contentInfo == null) continue;

                            if (isRecommend)
                            {
                                contentInfo.IsRecommend = false;
                            }
                            if (isHot)
                            {
                                contentInfo.IsHot = false;
                            }
                            if (isColor)
                            {
                                contentInfo.IsColor = false;
                            }
                            if (isTop)
                            {
                                contentInfo.IsTop = false;
                            }
                            DataProvider.ContentDao.Update(siteInfo, channelInfo, contentInfo);
                        }

                        request.AddSiteLog(siteId, "取消内容属性");
                    }
                }
                else if (pageType == "setHits")
                {
                    foreach (var channelContentId in channelContentIds)
                    {
                        var channelInfo = ChannelManager.GetChannelInfo(siteId, channelContentId.ChannelId);
                        var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, channelContentId.Id);
                        if (contentInfo == null) continue;

                        contentInfo.Hits = hits;
                        DataProvider.ContentDao.Update(siteInfo, channelInfo, contentInfo);
                    }

                    request.AddSiteLog(siteId, "设置内容点击量");
                }

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
