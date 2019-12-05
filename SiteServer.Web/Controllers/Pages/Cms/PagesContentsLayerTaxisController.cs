using System;
using System.Linq;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [OpenApiIgnore]
    [RoutePrefix("pages/cms/contentsLayerTaxis")]
    public class PagesContentsLayerTaxisController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var siteId = request.GetPostInt("siteId");
                var channelId = request.GetPostInt("channelId");
                var channelContentIds =
                    MinContentInfo.ParseMinContentInfoList(request.GetPostString("channelContentIds"));
                var isUp = request.GetPostBool("isUp");
                var taxis = request.GetPostInt("taxis");

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

                if (ETaxisTypeUtils.Equals(channelInfo.Additional.DefaultTaxisType, ETaxisType.OrderByTaxis))
                {
                    isUp = !isUp;
                }

                if (isUp == false)
                {
                    channelContentIds.Reverse();
                }

                foreach (var channelContentId in channelContentIds)
                {
                    var contentChannelInfo = ChannelManager.GetChannelInfo(siteId, channelContentId.ChannelId);
                    var tableName = ChannelManager.GetTableName(siteInfo, contentChannelInfo);
                    var contentInfo = ContentManager.GetContentInfo(siteInfo, contentChannelInfo, channelContentId.Id);
                    if (contentInfo == null) continue;

                    var isTop = contentInfo.IsTop;
                    for (var i = 1; i <= taxis; i++)
                    {
                        if (isUp)
                        {
                            if (DataProvider.ContentDao.SetTaxisToUp(siteId, tableName, channelContentId.ChannelId, channelContentId.Id, isTop) == false)
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (DataProvider.ContentDao.SetTaxisToDown(siteId, tableName, channelContentId.ChannelId, channelContentId.Id, isTop) == false)
                            {
                                break;
                            }
                        }
                    }
                }

                foreach (var distinctChannelId in channelContentIds.Select(x => x.ChannelId).Distinct())
                {
                    CreateManager.TriggerContentChangedEvent(siteId, distinctChannelId);
                }

                request.AddSiteLog(siteId, channelId, 0, "对内容排序", string.Empty);

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
