using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Extensions;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ChannelsController
    {
        [HttpGet, Route(RouteChannel)]
        public async Task<ActionResult<Channel>> Get(int siteId, int channelId)
        {
            var isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeChannels) ||
                         _authManager.IsAdmin;
            if (!isAuth) return Unauthorized();

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var channel = await _channelRepository.GetAsync(channelId);
            if (channel == null) return this.Error("无法确定内容对应的栏目");

            channel.Children = await _channelRepository.GetChildrenAsync(siteId, channelId);

            return channel;
        }
    }
}
