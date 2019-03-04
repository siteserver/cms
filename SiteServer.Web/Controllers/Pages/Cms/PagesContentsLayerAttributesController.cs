using System;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Caches.Content;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("pages/cms/contentsLayerAttributes")]
    public class PagesContentsLayerAttributesController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var rest = new Rest(Request);

                var siteId = rest.GetPostInt("siteId");
                var channelId = rest.GetPostInt("channelId");
                var contentIdList = TranslateUtils.StringCollectionToIntList(rest.GetPostString("contentIds"));
                var pageType = rest.GetPostString("pageType");
                var isRecommend = rest.GetPostBool("isRecommend");
                var isHot = rest.GetPostBool("isHot");
                var isColor = rest.GetPostBool("isColor");
                var isTop = rest.GetPostBool("isTop");
                var hits = rest.GetPostInt("hits");

                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
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

                        rest.AddSiteLog(siteId, "设置内容属性");
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

                        rest.AddSiteLog(siteId, "取消内容属性");
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

                    rest.AddSiteLog(siteId, "设置内容点击量");
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
