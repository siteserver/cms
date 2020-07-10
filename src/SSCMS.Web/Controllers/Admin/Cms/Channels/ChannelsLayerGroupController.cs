using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ChannelsLayerGroupController : ControllerBase
    {
        private const string Route = "cms/channels/channelsLayerGroup";
        private const string RouteAdd = "cms/channels/channelsLayerGroup/actions/add";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IChannelGroupRepository _channelGroupRepository;

        public ChannelsLayerGroupController(IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IChannelGroupRepository channelGroupRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _channelGroupRepository = channelGroupRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ObjectResult<IEnumerable<string>>>> Get([FromQuery] ChannelRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var groupNames = await _channelGroupRepository.GetGroupNamesAsync(request.SiteId);

            return new ObjectResult<IEnumerable<string>>
            {
                Value = groupNames
            };
        }

        [HttpPost, Route(RouteAdd)]
        public async Task<ActionResult<List<int>>> Add([FromBody] AddRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.Channels))
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

        [HttpPost, Route(Route)]
        public async Task<ActionResult<List<int>>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.Channels))
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
