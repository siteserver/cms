using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ChannelsController
    {
        [HttpDelete, Route(RouteChannel)]
        public async Task<ActionResult<Channel>> Delete(int siteId, int channelId)
        {
            var isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeChannels) ||
                         _authManager.IsAdmin &&
                         await _authManager.HasChannelPermissionsAsync(siteId, channelId,
                             Types.ChannelPermissions.Delete);
            if (!isAuth) return Unauthorized();

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(channelId);
            if (channel == null) return NotFound();

            var adminId = _authManager.AdminId;
            await _contentRepository.TrashContentsAsync(site, channelId, adminId);
            await _channelRepository.DeleteAsync(site, channelId, adminId);

            return channel;
        }
    }
}
