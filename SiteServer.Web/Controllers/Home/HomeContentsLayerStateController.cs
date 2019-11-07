using System;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;

namespace SiteServer.API.Controllers.Home
{
    [OpenApiIgnore]
    [RoutePrefix("home/contentsLayerState")]
    public class HomeContentsLayerStateController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");
                var contentId = request.GetQueryInt("contentId");

                if (!request.IsUserLoggin ||
                    !request.UserPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentView))
                {
                    return Unauthorized();
                }

                var site = await SiteManager.GetSiteAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var contentInfo = ContentManager.GetContentInfo(site, channelInfo, contentId);
                if (contentInfo == null) return BadRequest("无法确定对应的内容");

                var title = contentInfo.Title;
                var checkState = 
                    CheckManager.GetCheckState(site, contentInfo);

                var tableName = ChannelManager.GetTableName(site, channelInfo);
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
