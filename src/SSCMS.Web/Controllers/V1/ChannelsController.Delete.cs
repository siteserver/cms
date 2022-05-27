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
        [OpenApiOperation("删除栏目 API", "删除栏目，使用DELETE发起请求，请求地址为/api/v1/channels/{siteId}/{channelId}")]
        [HttpPost, Route(RouteChannelDelete)]
        public async Task<ActionResult<Channel>> Delete([FromRoute] int siteId, [FromRoute] int channelId)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeChannels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(channelId);
            if (channel == null) return this.Error(Constants.ErrorNotFound);

            var adminId = _authManager.AdminId;
            await _contentRepository.TrashContentsAsync(site, channelId, adminId);
            await _channelRepository.DeleteAsync(site, channelId, adminId);

            return channel;
        }
    }
}
