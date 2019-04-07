using System;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Caches.Content;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Home
{
    [RoutePrefix("home/contentsLayerAttributes")]
    public class HomeContentsLayerAttributesController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();

                var siteId = Request.GetPostInt("siteId");
                var channelId = Request.GetPostInt("channelId");
                var contentIdList = TranslateUtils.StringCollectionToIntList(Request.GetPostString("contentIds"));
                var pageType = Request.GetPostString("pageType");
                var isRecommend = Request.GetPostBool("isRecommend");
                var isHot = Request.GetPostBool("isHot");
                var isColor = Request.GetPostBool("isColor");
                var isTop = Request.GetPostBool("isTop");
                var hits = Request.GetPostInt("hits");

                if (!rest.IsUserLoggin ||
                    !rest.UserPermissions.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentEdit))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                if (pageType == "setAttributes")
                {
                    if (isRecommend || isHot || isColor || isTop)
                    {
                        foreach (var contentId in contentIdList)
                        {
                            var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
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
                            DataProvider.ContentRepository.Update(siteInfo, channelInfo, contentInfo);
                        }

                        LogUtils.AddSiteLog(siteId, rest.AdminName, "设置内容属性");
                    }
                }
                else if(pageType == "cancelAttributes")
                {
                    if (isRecommend || isHot || isColor || isTop)
                    {
                        foreach (var contentId in contentIdList)
                        {
                            var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
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
                            DataProvider.ContentRepository.Update(siteInfo, channelInfo, contentInfo);
                        }

                        LogUtils.AddSiteLog(siteId, rest.AdminName, "取消内容属性");
                    }
                }
                else if (pageType == "setHits")
                {
                    foreach (var contentId in contentIdList)
                    {
                        var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
                        if (contentInfo == null) continue;

                        contentInfo.Hits = hits;
                        DataProvider.ContentRepository.Update(siteInfo, channelInfo, contentInfo);
                    }

                    LogUtils.AddSiteLog(siteId, rest.AdminName, "设置内容点击量");
                }

                return Ok(new
                {
                    Value = contentIdList
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
