using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsLayerGroupController
    {
        [HttpPost, Route(RouteAdd)]
        public async Task<ActionResult<List<int>>> Add([FromBody] AddRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var group = new ChannelGroup
            {
                GroupName = request.GroupName,
                SiteId = request.SiteId,
                Description = request.Description
            };

            if (await _channelGroupRepository.IsExistsAsync(request.SiteId, group.GroupName))
            {
                await _channelGroupRepository.UpdateAsync(group);
                await _authManager.AddSiteLogAsync(request.SiteId, "修改栏目组", $"栏目组:{group.GroupName}");
            }
            else
            {
                await _channelGroupRepository.InsertAsync(group);
                await _authManager.AddSiteLogAsync(request.SiteId, "添加栏目组", $"栏目组:{group.GroupName}");
            }

            var expendedChannelIds = new List<int>
            {
                request.SiteId
            };
            foreach (var channelId in request.ChannelIds)
            {
                var channel = await _channelRepository.GetAsync(channelId);
                if (channel == null) continue;

                if (!expendedChannelIds.Contains(channel.ParentId))
                {
                    expendedChannelIds.Add(channel.ParentId);
                }

                var list = ListUtils.GetStringList(channel.GroupNames);
                if (!list.Contains(group.GroupName)) list.Add(group.GroupName);
                channel.GroupNames = list;

                await _channelRepository.UpdateAsync(channel);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "批量设置栏目组", $"栏目组:{group.GroupName}");

            return expendedChannelIds;
        }
    }
}