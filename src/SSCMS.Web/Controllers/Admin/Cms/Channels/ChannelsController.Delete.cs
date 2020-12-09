using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<List<int>>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ChannelPermissions.Delete))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return this.Error("无法确定父栏目");

            var channelIdList = await _channelRepository.GetChannelIdsAsync(request.SiteId, request.ChannelId, ScopeType.All);

            if (request.DeleteFiles)
            {
                await _createManager.DeleteChannelsAsync(site, channelIdList);
            }

            var adminId = _authManager.AdminId;

            foreach (var channelId in channelIdList)
            {
                await _contentRepository.TrashContentsAsync(site, channelId, adminId);
            }

            foreach (var channelId in channelIdList)
            {
                await _channelRepository.DeleteAsync(site, channelId, adminId);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "删除栏目", $"栏目:{channel.ChannelName}");

            return new List<int>
            {
                request.SiteId,
                channel.ParentId
            };
        }
    }
}
