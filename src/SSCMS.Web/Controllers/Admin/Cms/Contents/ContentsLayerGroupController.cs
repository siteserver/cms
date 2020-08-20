using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ContentsLayerGroupController : ControllerBase
    {
        private const string Route = "cms/contents/contentsLayerGroup";
        private const string RouteAdd = "cms/contents/contentsLayerGroup/actions/add";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentGroupRepository _contentGroupRepository;

        public ContentsLayerGroupController(IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IContentGroupRepository contentGroupRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentGroupRepository = contentGroupRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ObjectResult<IEnumerable<string>>>> Get([FromQuery] ChannelRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    Types.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var groupNames = await _contentGroupRepository.GetGroupNamesAsync(request.SiteId);

            return new ObjectResult<IEnumerable<string>>
            {
                Value = groupNames
            };
        }

        [HttpPost, Route(RouteAdd)]
        public async Task<ActionResult<BoolResult>> Add([FromBody] AddRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    Types.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelContentIds = ContentUtility.ParseSummaries(request.ChannelContentIds);

            var group = new ContentGroup
            {
                GroupName = request.GroupName,
                SiteId = request.SiteId,
                Description = request.Description
            };

            if (await _contentGroupRepository.IsExistsAsync(request.SiteId, group.GroupName))
            {
                await _contentGroupRepository.UpdateAsync(group);
                await _authManager.AddSiteLogAsync(request.SiteId, "修改内容组", $"内容组:{group.GroupName}");
            }
            else
            {
                await _contentGroupRepository.InsertAsync(group);
                await _authManager.AddSiteLogAsync(request.SiteId, "添加内容组", $"内容组:{group.GroupName}");
            }

            foreach (var channelContentId in channelContentIds)
            {
                var channel = await _channelRepository.GetAsync(channelContentId.ChannelId);
                var content = await _contentRepository.GetAsync(site, channel, channelContentId.Id);
                if (content == null) continue;

                var list = ListUtils.GetStringList(content.GroupNames);
                if (!list.Contains(group.GroupName)) list.Add(group.GroupName);
                content.GroupNames = list;

                await _contentRepository.UpdateAsync(site, channel, content);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "批量设置内容组", $"内容组:{group.GroupName}");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    Types.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var allGroupNames = await _contentGroupRepository.GetGroupNamesAsync(request.SiteId);

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);
            foreach (var summary in summaries)
            {
                var channel = await _channelRepository.GetAsync(summary.ChannelId);
                var content = await _contentRepository.GetAsync(site, channel, summary.Id);
                if (content == null) continue;

                var list = new List<string>();
                foreach (var groupNames in ListUtils.GetStringList(content.GroupNames))
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

                await _contentRepository.UpdateAsync(site, channel, content);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, request.IsCancel ? "批量取消内容组" : "批量设置内容组");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
