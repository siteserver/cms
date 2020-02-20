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

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    [RoutePrefix("pages/cms/contents/contentsLayerGroup")]
    public partial class PagesContentsLayerGroupController : ApiController
    {
        private const string Route = "";
        private const string RouteAdd = "actions/add";

        [HttpGet, Route(Route)]
        public async Task<ObjectResult<IEnumerable<string>>> Get([FromUri] ChannelRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentEdit))
            {
                return Request.Unauthorized<ObjectResult<IEnumerable<string>>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<ObjectResult<IEnumerable<string>>>();

            var groupNames = await DataProvider.ContentGroupRepository.GetGroupNamesAsync(request.SiteId);

            return new ObjectResult<IEnumerable<string>>
            {
                Value = groupNames
            };
        }

        [HttpPost, Route(RouteAdd)]
        public async Task<BoolResult> Add([FromBody] AddRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentEdit))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            var channelContentIds = ContentUtility.ParseSummaries(request.ChannelContentIds);

            var group = new ContentGroup
            {
                GroupName = request.GroupName,
                SiteId = request.SiteId,
                Description = request.Description
            };

            if (await DataProvider.ContentGroupRepository.IsExistsAsync(request.SiteId, group.GroupName))
            {
                await DataProvider.ContentGroupRepository.UpdateAsync(group);
                await auth.AddSiteLogAsync(request.SiteId, "修改内容组", $"内容组:{group.GroupName}");
            }
            else
            {
                await DataProvider.ContentGroupRepository.InsertAsync(group);
                await auth.AddSiteLogAsync(request.SiteId, "添加内容组", $"内容组:{group.GroupName}");
            }

            foreach (var channelContentId in channelContentIds)
            {
                var channel = await DataProvider.ChannelRepository.GetAsync(channelContentId.ChannelId);
                var content = await DataProvider.ContentRepository.GetAsync(site, channel, channelContentId.Id);
                if (content == null) continue;

                var list = Utilities.GetStringList(content.GroupNames);
                if (!list.Contains(group.GroupName)) list.Add(group.GroupName);
                content.GroupNames = list;

                await DataProvider.ContentRepository.UpdateAsync(site, channel, content);
            }

            await auth.AddSiteLogAsync(request.SiteId, "批量设置内容组", $"内容组:{group.GroupName}");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(Route)]
        public async Task<BoolResult> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentEdit))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            var allGroupNames = await DataProvider.ContentGroupRepository.GetGroupNamesAsync(request.SiteId);

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);
            foreach (var summary in summaries)
            {
                var channel = await DataProvider.ChannelRepository.GetAsync(summary.ChannelId);
                var content = await DataProvider.ContentRepository.GetAsync(site, channel, summary.Id);
                if (content == null) continue;

                var list = new List<string>();
                foreach (var groupNames in Utilities.GetStringList(content.GroupNames))
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
                content.GroupNames = list;

                await DataProvider.ContentRepository.UpdateAsync(site, channel, content);
            }

            await auth.AddSiteLogAsync(request.SiteId, request.IsCancel ? "批量取消内容组" : "批量设置内容组");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
