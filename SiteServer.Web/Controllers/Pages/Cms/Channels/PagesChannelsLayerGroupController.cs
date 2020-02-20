using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Channels
{
    [RoutePrefix("pages/cms/channels/channelsLayerGroup")]
    public partial class PagesChannelsLayerGroupController : ApiController
    {
        private const string Route = "";
        private const string RouteAdd = "actions/add";

        [HttpGet, Route(Route)]
        public async Task<ObjectResult<IEnumerable<string>>> Get([FromUri] ChannelRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Channels))
            {
                return Request.Unauthorized<ObjectResult<IEnumerable<string>>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<ObjectResult<IEnumerable<string>>>();

            var groupNames = await DataProvider.ChannelGroupRepository.GetGroupNamesAsync(request.SiteId);

            return new ObjectResult<IEnumerable<string>>
            {
                Value = groupNames
            };
        }

        [HttpPost, Route(RouteAdd)]
        public async Task<List<int>> Add([FromBody] AddRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Channels))
            {
                return Request.Unauthorized<List<int>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<List<int>>();

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
        public async Task<List<int>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Channels))
            {
                return Request.Unauthorized<List<int>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<List<int>>();

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
