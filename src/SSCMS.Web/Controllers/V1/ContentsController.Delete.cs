using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ContentsController
    {
        [OpenApiOperation("删除内容 API", "删除内容，使用DELETE发起请求，请求地址为/api/v1/contents/{siteId}/{channelId}/{id}")]
        [HttpPost, Route(RouteContentDelete)]
        public async Task<ActionResult<Content>> Delete([FromRoute] int siteId, [FromRoute] int channelId, [FromRoute] int id)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeContents))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(channelId);
            if (channel == null) return this.Error(Constants.ErrorNotFound);

            if (!await _authManager.HasContentPermissionsAsync(siteId, channelId, MenuUtils.ContentPermissions.Delete)) return Unauthorized();

            var content = await _contentRepository.GetAsync(site, channel, id);
            if (content == null) return this.Error(Constants.ErrorNotFound);

            await _authManager.AddSiteLogAsync(site.Id, channel.Id, id, "删除内容",
                    $"栏目:{await _channelRepository.GetChannelNameNavigationAsync(site.Id, channel.Id)},内容标题:{content.Title}");
                    
            await _createManager.DeleteContentAsync(site, content.ChannelId, content.Id);
            await _contentRepository.TrashContentAsync(site, channel, id, _authManager.AdminId);
            await _createManager.TriggerContentChangedEventAsync(site.Id, channel.Id);

            return content;
        }
    }
}
