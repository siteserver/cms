using System;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Caches.Content;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Enumerations;
using SiteServer.CMS.Database.Core;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Home
{
    [RoutePrefix("home/contentsLayerTaxis")]
    public class HomeContentsLayerTaxisController : ApiController
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
                var isUp = rest.GetPostBool("isUp");
                var taxis = rest.GetPostInt("taxis");

                if (!rest.IsUserLoggin ||
                    !rest.UserPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentEdit))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                if (ETaxisTypeUtils.Equals(channelInfo.DefaultTaxisType, ETaxisType.OrderByTaxis))
                {
                    isUp = !isUp;
                }

                if (isUp == false)
                {
                    contentIdList.Reverse();
                }

                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

                foreach (var contentId in contentIdList)
                {
                    var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
                    if (contentInfo == null) continue;

                    var isTop = contentInfo.Top;
                    for (var i = 1; i <= taxis; i++)
                    {
                        if (isUp)
                        {
                            if (DataProvider.ContentRepository.SetTaxisToUp(tableName, channelId, contentId, isTop) == false)
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (DataProvider.ContentRepository.SetTaxisToDown(tableName, channelId, contentId, isTop) == false)
                            {
                                break;
                            }
                        }
                    }
                }

                CreateManager.TriggerContentChangedEvent(siteId, channelId);

                rest.AddSiteLog(siteId, channelId, 0, "对内容排序", string.Empty);

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
