using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Framework;

namespace SiteServer.API.Controllers.Home
{
    [RoutePrefix("home/contentsLayerArrange")]
    public class HomeContentsLayerArrangeController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();

            var siteId = request.GetPostInt("siteId");
            var channelId = request.GetPostInt("channelId");
            var attributeName = request.GetPostString("attributeName");
            var isDesc = request.GetPostBool("isDesc");

            if (!request.IsUserLoggin ||
                !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                    Constants.ChannelPermissions.ContentEdit))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            if (site == null) return BadRequest("无法确定内容对应的站点");

            var channel = await DataProvider.ChannelRepository.GetAsync(channelId);
            if (channel == null) return BadRequest("无法确定内容对应的栏目");

            await DataProvider.ContentRepository.UpdateArrangeTaxisAsync(site, channel, attributeName, isDesc);

            await request.AddSiteLogAsync(siteId, "批量整理", string.Empty);

            return Ok(new
            {
                Value = true
            });
        }
    }
}
