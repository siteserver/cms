using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ContentsController
    {
        [OpenApiOperation("获取内容 API", "获取内容，使用GET发起请求，请求地址为/api/v1/contents/{siteId}/{channelId}/{id}")]
        [HttpGet, Route(RouteContent)]
        public async Task<ActionResult<Content>> Get([FromRoute]int siteId, [FromRoute] int channelId, [FromRoute] int id)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeContents))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(channelId);
            if (channelInfo == null) return NotFound();

            if (!await _authManager.HasContentPermissionsAsync(siteId, channelId, MenuUtils.ContentPermissions.View)) return Unauthorized();

            var content = await _contentRepository.GetAsync(site, channelInfo, id);
            if (content == null) return NotFound();

            return content;
        }
    }
}
