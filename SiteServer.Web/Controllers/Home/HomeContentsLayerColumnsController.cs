using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;

namespace SiteServer.API.Controllers.Home
{
    [RoutePrefix("home/contentsLayerColumns")]
    public class HomeContentsLayerColumnsController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();

            var siteId = request.GetQueryInt("siteId");
            var channelId = request.GetQueryInt("channelId");

            if (!request.IsUserLoggin ||
                !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                    Constants.ChannelPermissions.ChannelEdit))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            if (site == null) return BadRequest("无法确定内容对应的站点");

            var channel = await DataProvider.ChannelRepository.GetAsync(channelId);
            if (channel == null) return BadRequest("无法确定内容对应的栏目");

            var attributes = await ColumnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);

            return Ok(new
            {
                Value = attributes
            });
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();

            var siteId = request.GetPostInt("siteId");
            var channelId = request.GetPostInt("channelId");
            var attributeNames = request.GetPostString("attributeNames");

            if (!request.IsUserLoggin ||
                !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                    Constants.ChannelPermissions.ChannelEdit))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            if (site == null) return BadRequest("无法确定内容对应的站点");

            var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
            if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

            channelInfo.ListColumns = attributeNames;

            await DataProvider.ChannelRepository.UpdateAsync(channelInfo);

            await request.AddSiteLogAsync(siteId, "设置内容显示项", $"显示项:{attributeNames}");

            return Ok(new
            {
                Value = attributeNames
            });
        }
    }
}
