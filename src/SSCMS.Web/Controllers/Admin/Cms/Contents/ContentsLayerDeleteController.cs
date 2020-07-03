using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ContentsLayerDeleteController : ControllerBase
    {
        private const string Route = "cms/contents/contentsLayerDelete";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public ContentsLayerDeleteController(IAuthManager authManager, ICreateManager createManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _createManager = createManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Delete))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);

            var contents = new List<Content>();
            foreach (var summary in summaries)
            {
                var channel = await _channelRepository.GetAsync(summary.ChannelId);
                var content = await _contentRepository.GetAsync(site, channel, summary.Id);
                if (content == null) continue;

                var pageContent = content.Clone<Content>();
                pageContent.Set(ColumnsManager.CheckState, CheckManager.GetCheckState(site, content));
                contents.Add(pageContent);
            }

            return new GetResult
            {
                Contents = contents
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Delete))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);

            if (!request.IsRetainFiles)
            {
                foreach (var summary in summaries)
                {
                    await _createManager.DeleteContentAsync(site, summary.ChannelId, summary.Id);
                }
            }

            if (summaries.Count == 1)
            {
                var summary = summaries[0];

                var content = await _contentRepository.GetAsync(site, summary.ChannelId, summary.Id);
                if (content != null)
                {
                    await _authManager.AddSiteLogAsync(request.SiteId, summary.ChannelId, summary.Id, "删除内容",
                        $"栏目:{await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, summary.ChannelId)},内容标题:{content.Title}");
                }
            }
            else
            {
                await _authManager.AddSiteLogAsync(request.SiteId, "批量删除内容",
                    $"栏目:{await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, request.ChannelId)},内容条数:{summaries.Count}");
            }

            var adminId = _authManager.AdminId;
            foreach (var distinctChannelId in summaries.Select(x => x.ChannelId).Distinct())
            {
                var distinctChannel = await _channelRepository.GetAsync(distinctChannelId);
                var contentIdList = summaries.Where(x => x.ChannelId == distinctChannelId)
                    .Select(x => x.Id).ToList();
                await _contentRepository.TrashContentsAsync(site, distinctChannel, contentIdList, adminId);

                await _createManager.TriggerContentChangedEventAsync(request.SiteId, distinctChannelId);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
