using System;
using System.Web.Http;
using SiteServer.API.Common;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("pages/cms/contentsLayerAttributes")]
    public class PagesContentsLayerAttributesController : ControllerBase
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = GetRequest();

                var siteId = request.GetPostInt("siteId");
                var channelId = request.GetPostInt("channelId");
                var contentIdList = TranslateUtils.StringCollectionToIntList(request.GetPostString("contentIds"));
                var pageType = request.GetPostString("pageType");
                var isRecommend = request.GetPostBool("isRecommend");
                var isHot = request.GetPostBool("isHot");
                var isColor = request.GetPostBool("isColor");
                var isTop = request.GetPostBool("isTop");
                var hits = request.GetPostInt("hits");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
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
                            channelInfo.ContentDao.Update(siteInfo, channelInfo, contentInfo);
                        }

                        request.AddSiteLog(siteId, "设置内容属性");
                    }
                }
                else if (pageType == "cancelAttributes")
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
                            channelInfo.ContentDao.Update(siteInfo, channelInfo, contentInfo);
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
                        channelInfo.ContentDao.Update(siteInfo, channelInfo, contentInfo);
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
