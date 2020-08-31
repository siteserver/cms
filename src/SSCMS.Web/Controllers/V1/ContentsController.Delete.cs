using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ContentsController
    {
        [OpenApiOperation("删除内容API", "")]
        [HttpDelete, Route(RouteContent)]
        public async Task<ActionResult<Content>> Delete(int siteId, int channelId, int id)
        {
            var isUserAuth = _authManager.IsUser && await _authManager.HasContentPermissionsAsync(siteId, channelId, Types.ContentPermissions.Delete);
            var isApiAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeContents) ||
                            _authManager.IsUser &&
                            await _authManager.HasContentPermissionsAsync(siteId, channelId, Types.ContentPermissions.Delete) ||
                            _authManager.IsAdmin &&
                            await _authManager.HasContentPermissionsAsync(siteId, channelId, Types.ContentPermissions.Delete);
            if (!isUserAuth && !isApiAuth) return Unauthorized();

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(channelId);
            if (channel == null) return NotFound();

            if (!await _authManager.HasContentPermissionsAsync(siteId, channelId, Types.ContentPermissions.Delete)) return Unauthorized();

            var content = await _contentRepository.GetAsync(site, channel, id);
            if (content == null) return NotFound();

            await _contentRepository.TrashContentAsync(site, channel, id, _authManager.AdminId);

            return content;
        }
    }
}
