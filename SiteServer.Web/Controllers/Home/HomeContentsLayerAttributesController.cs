using System;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Plugin.Impl;
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
                var request = new RequestImpl();

                var siteId = request.GetPostInt("siteId");
                var channelId = request.GetPostInt("channelId");
                var contentIdList = TranslateUtils.StringCollectionToIntList(request.GetPostString("contentIds"));
                var pageType = request.GetPostString("pageType");
                var isRecommend = request.GetPostBool("isRecommend");
                var isHot = request.GetPostBool("isHot");
                var isColor = request.GetPostBool("isColor");
                var isTop = request.GetPostBool("isTop");
                var hits = request.GetPostInt("hits");

                if (!request.IsUserLoggin ||
                    !request.UserPermissionsImpl.HasChannelPermissions(siteId, channelId,
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
                        foreach (var contentId in contentIdList)
                        {
                            var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
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
                    foreach (var contentId in contentIdList)
                    {
                        var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
                        if (contentInfo == null) continue;

                        contentInfo.Hits = hits;
                        DataProvider.ContentDao.Update(siteInfo, channelInfo, contentInfo);
                    }

                    request.AddSiteLog(siteId, "设置内容点击量");
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
