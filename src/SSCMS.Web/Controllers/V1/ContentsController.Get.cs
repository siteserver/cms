using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ContentsController
    {
        [OpenApiOperation("获取内容API", "")]
        [HttpGet, Route(RouteContent)]
        public async Task<ActionResult<Content>> Get(int siteId, int channelId, int id)
        {
            var isUserAuth = _authManager.IsUser && await _authManager.HasContentPermissionsAsync(siteId, channelId, Types.ContentPermissions.View);
            var isApiAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeContents) ||
                            _authManager.IsUser &&
                            await _authManager.HasContentPermissionsAsync(siteId, channelId, Types.ContentPermissions.View) ||
                            _authManager.IsAdmin &&
                            await _authManager.HasContentPermissionsAsync(siteId, channelId, Types.ContentPermissions.View);
            if (!isUserAuth && !isApiAuth) return Unauthorized();

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(channelId);
            if (channelInfo == null) return NotFound();

            if (!await _authManager.HasContentPermissionsAsync(siteId, channelId, Types.ContentPermissions.View)) return Unauthorized();

            var content = await _contentRepository.GetAsync(site, channelInfo, id);
            if (content == null) return NotFound();

            return content;
        }
    }
}
