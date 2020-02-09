using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Home
{
    
    [RoutePrefix("home/contentsLayerView")]
    public class HomeContentsLayerViewController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");
                var contentId = request.GetQueryInt("contentId");

                if (!request.IsUserLoggin ||
                    !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentView))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, contentId);
                if (contentInfo == null) return BadRequest("无法确定对应的内容");

                contentInfo.Set(ContentAttribute.CheckState, CheckManager.GetCheckState(site, contentInfo));

                var channelName = await DataProvider.ChannelRepository.GetChannelNameNavigationAsync(siteId, channelId);

                var attributes = await ColumnsManager.GetContentListColumnsAsync(site, channelInfo, true);

                return Ok(new
                {
                    Value = contentInfo,
                    ChannelName = channelName,
                    Attributes = attributes
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
