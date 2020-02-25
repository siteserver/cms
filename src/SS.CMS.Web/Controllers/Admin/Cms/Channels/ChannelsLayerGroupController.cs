using System.Collections.Generic;
using System.Threading.Tasks;
using Datory.Utils;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Cms.Channels
{
    [Route("admin/cms/channels/channelsLayerGroup")]
    public partial class ChannelsLayerGroupController : ControllerBase
    {
        private const string Route = "";
        private const string RouteAdd = "actions/add";

        private readonly IAuthManager _authManager;

        public ChannelsLayerGroupController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ObjectResult<IEnumerable<string>>>> Get([FromQuery] ChannelRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var groupNames = await DataProvider.ChannelGroupRepository.GetGroupNamesAsync(request.SiteId);

            return new ObjectResult<IEnumerable<string>>
            {
                Value = groupNames
            };
        }

        [HttpPost, Route(RouteAdd)]
        public async Task<ActionResult<List<int>>> Add([FromBody] AddRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var group = new ChannelGroup
            {
                GroupName = request.GroupName,
                SiteId = request.SiteId,
                Description = request.Description
            };

            if (await DataProvider.ChannelGroupRepository.IsExistsAsync(request.SiteId, group.GroupName))
            {
                await DataProvider.ChannelGroupRepository.UpdateAsync(group);
                await auth.AddSiteLogAsync(request.SiteId, "修改栏目组", $"栏目组:{group.GroupName}");
            }
            else
            {
                await DataProvider.ChannelGroupRepository.InsertAsync(group);
                await auth.AddSiteLogAsync(request.SiteId, "添加栏目组", $"栏目组:{group.GroupName}");
            }

            var expendedChannelIds = new List<int>
            {
                request.SiteId
            };
            foreach (var channelId in request.ChannelIds)
            {
                var channel = await DataProvider.ChannelRepository.GetAsync(channelId);
                if (channel == null) continue;

                if (!expendedChannelIds.Contains(channel.ParentId))
                {
                    expendedChannelIds.Add(channel.ParentId);
                }

                var list = Utilities.GetStringList(channel.GroupNames);
                if (!list.Contains(group.GroupName)) list.Add(group.GroupName);
                channel.GroupNames = list;

                await DataProvider.ChannelRepository.UpdateAsync(channel);
            }

            await auth.AddSiteLogAsync(request.SiteId, "批量设置栏目组", $"栏目组:{group.GroupName}");

            return expendedChannelIds;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<List<int>>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var allGroupNames = await DataProvider.ChannelGroupRepository.GetGroupNamesAsync(request.SiteId);

            var expendedChannelIds = new List<int>
            {
                request.SiteId
            };
            foreach (var channelId in request.ChannelIds)
            {
                var channel = await DataProvider.ChannelRepository.GetAsync(channelId);
                if (channel == null) continue;

                if (!expendedChannelIds.Contains(channel.ParentId))
                {
                    expendedChannelIds.Add(channel.ParentId);
                }

                var list = new List<string>();
                foreach (var groupNames in Utilities.GetStringList(channel.GroupNames))
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

                await DataProvider.ChannelRepository.UpdateAsync(channel);
            }

            await auth.AddSiteLogAsync(request.SiteId, request.IsCancel ? "批量取消栏目组" : "批量设置栏目组");

            return expendedChannelIds;
        }
    }
}
