using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsLayerGroupController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<List<int>>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var allGroupNames = await _channelRepository.GetGroupNamesAsync(request.SiteId);

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

                var list = new List<string>();
                foreach (var groupNames in ListUtils.GetStringList(channel.GroupNames))
                {
                    if (allGroupNames.Contains(groupNames))
                    {
                        list.Add(groupNames);
                    }
                }

                foreach (var name in request.GroupNames)
                {
                    if (request.IsCancel)
                    {
                        if (list.Contains(name)) list.Remove(name);
                    }
                    else
                    {
                        if (!list.Contains(name)) list.Add(name);
                    }
                }
                channel.GroupNames = list;

                await _channelRepository.UpdateAsync(channel);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, request.IsCancel ? "批量取消栏目组" : "批量设置栏目组");

            return expendedChannelIds;
        }
    }
}