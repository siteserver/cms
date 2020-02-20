using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Home
{
    
    [RoutePrefix("home/contentsLayerState")]
    public class HomeContentsLayerStateController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
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

            var channel = await DataProvider.ChannelRepository.GetAsync(channelId);
            if (channel == null) return BadRequest("无法确定内容对应的栏目");

            var content = await DataProvider.ContentRepository.GetAsync(site, channel, contentId);
            if (content == null) return BadRequest("无法确定对应的内容");

            var title = content.Title;
            var checkState = CheckManager.GetCheckState(site, content);

            var contentChecks =
                await DataProvider.ContentCheckRepository.GetCheckListAsync(content.SiteId, content.ChannelId,
                    contentId);

            return Ok(new
            {
                Value = contentChecks,
                Title = title,
                CheckState = checkState
            });
        }
    }
}
