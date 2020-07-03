using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.ToDel
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.User)]
    [Route(Constants.ApiHomePrefix + "todel/")]
    public partial class ContentsLayerDeleteController : ControllerBase
    {
        private const string Route = "contentsLayerDelete";

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
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Delete))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var retVal = new List<IDictionary<string, object>>();
            foreach (var contentId in request.ContentIds)
            {
                var contentInfo = await _contentRepository.GetAsync(site, channel, contentId);
                if (contentInfo == null) continue;

                var dict = contentInfo.ToDictionary();
                dict["checkState"] =
                    CheckManager.GetCheckState(site, contentInfo);
                retVal.Add(dict);
            }

            return new GetResult
            {
                Value = retVal
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Delete))
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
