using System;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [OpenApiIgnore]
    [RoutePrefix("pages/cms/contentsLayerState")]
    public class PagesContentsLayerStateController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");
                var contentId = request.GetQueryInt("contentId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentView))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
                if (contentInfo == null) return BadRequest("无法确定对应的内容");

                var title = WebUtils.GetContentTitle(siteInfo, contentInfo, string.Empty);
                var checkState = 
                    CheckManager.GetCheckState(siteInfo, contentInfo);

                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
                var contentChecks = DataProvider.ContentCheckDao.GetCheckInfoList(tableName, contentId);

                return Ok(new
                {
                    Value = contentChecks,
                    Title = title,
                    CheckState = checkState
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
