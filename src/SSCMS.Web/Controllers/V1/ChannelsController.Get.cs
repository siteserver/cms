using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ChannelsController
    {
        [OpenApiOperation("获取栏目 API", "获取栏目，使用GET发起请求，请求地址为/api/v1/channels/{siteId}/{channelId}")]
        [HttpGet, Route(RouteChannel)]
        public async Task<ActionResult<Channel>> Get([FromRoute] int siteId, [FromRoute] int channelId)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeChannels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var channel = await _channelRepository.GetAsync(channelId);
            if (channel == null) return this.Error("无法确定内容对应的栏目");

            channel.Children = await _channelRepository.GetChildrenAsync(siteId, channelId);

            return channel;
        }
    }
}
