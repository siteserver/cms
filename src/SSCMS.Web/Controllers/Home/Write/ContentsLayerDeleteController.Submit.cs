using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsLayerDeleteController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.Delete))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            if (!request.IsRetainFiles)
            {
                await _createManager.DeleteContentsAsync(site, request.ChannelId, request.ContentIds);
            }

            if (request.ContentIds.Count == 1)
            {
                var contentId = request.ContentIds[0];
                var content = await _contentRepository.GetAsync(site, channel, contentId);
                if (content != null)
                {
                    await _authManager.AddSiteLogAsync(request.SiteId, request.ChannelId, contentId, "删除内容",
                        $"栏目:{await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, request.ChannelId)},内容标题:{content.Title}");
                }

            }
            else
            {
                await _authManager.AddSiteLogAsync(request.SiteId, "批量删除内容",
                    $"栏目:{await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, request.ChannelId)},内容条数:{request.ContentIds.Count}");
            }

            var adminId = _authManager.AdminId;
            await _contentRepository.TrashContentsAsync(site, channel, request.ContentIds, adminId);

            await _createManager.TriggerContentChangedEventAsync(request.SiteId, request.ChannelId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
