using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.ToDel
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.User)]
    [Route(Constants.ApiHomePrefix + "todel/")]
    public partial class ContentsLayerStateController : ControllerBase
    {
        private const string Route = "contentsLayerState";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentCheckRepository _contentCheckRepository;

        public ContentsLayerStateController(IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IContentCheckRepository contentCheckRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentCheckRepository = contentCheckRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.View))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var content = await _contentRepository.GetAsync(site, channel, request.ContentId);
            if (content == null) return NotFound();

            var title = content.Title;
            var checkState = CheckManager.GetCheckState(site, content);

            var contentChecks =
                await _contentCheckRepository.GetCheckListAsync(content.SiteId, content.ChannelId, request.ContentId);

            return new GetResult
            {
                ContentChecks = contentChecks,
                Title = title,
                CheckState = checkState
            };
        }
    }
}
