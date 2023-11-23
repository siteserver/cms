using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerViewController
    {
        [HttpPost, Route(RoutePreview)]
        public async Task<ActionResult<PreviewResult>> Preview([FromBody] PreviewRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.View))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var channel = await _channelRepository.GetAsync(request.ChannelId);
            var content = await _contentRepository.GetAsync(request.SiteId, request.ChannelId, request.ContentId);

            if (site == null)
            {
                return this.Error("指定的站点不存在");
            }
            if (channel == null)
            {
                return this.Error("指定的栏目不存在");
            }
            if (content == null)
            {
                return this.Error("指定的内容不存在");
            }

            content = content.Clone<Models.Content>();
            content.Checked = true;

            content.Id = await _contentRepository.InsertPreviewAsync(site, channel, content);

            return new PreviewResult
            {
                Url = _pathManager.GetPreviewContentUrl(request.SiteId, request.ChannelId, content.Id, true)
            };
        }
    }
}
