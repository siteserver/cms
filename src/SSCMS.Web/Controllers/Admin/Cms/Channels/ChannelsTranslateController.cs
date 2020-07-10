using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Extensions;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ChannelsTranslateController : ControllerBase
    {
        private const string Route = "cms/channels/channelsTranslate";
        private const string RouteOptions = "cms/channels/channelsTranslate/actions/options";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public ChannelsTranslateController(IAuthManager authManager, IPathManager pathManager, ICreateManager createManager, IDatabaseManager databaseManager, IPluginManager pluginManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        [HttpGet]
        [Route(Route)]
        public async Task<ActionResult<GetResult>> GetConfig([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.ChannelsTranslate))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                return new
                {
                    Count = count,
                    summary.IndexName
                };
            });

            var transSites = await _siteRepository.GetSelectsAsync();
            var translateTypes = ListUtils.GetEnums<TranslateType>().Select(x => new Select<string>(x));

            return new GetResult
            {
                Channels = cascade,
                TransSites = transSites,
                TranslateTypes = translateTypes
            };
        }

        [HttpPost, Route(RouteOptions)]
        public async Task<ActionResult<GetOptionsResult>> GetOptions([FromBody]GetOptionsRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.ChannelsTranslate))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var transChannels = await _channelRepository.GetAsync(request.TransSiteId);
            var transSite = await _siteRepository.GetAsync(request.TransSiteId);
            var cascade = await _channelRepository.GetCascadeAsync(transSite, transChannels, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);

                return new
                {
                    summary.IndexName,
                    Count = count
                };
            });

            return new GetOptionsResult
            {
                TransChannels = cascade
            };
        }

        [HttpPost]
        [Route(Route)]
        public async Task<ActionResult<BoolResult>> Translate([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.ChannelsTranslate))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var adminId = _authManager.AdminId;
            await TranslateAsync(site, request.TransSiteId, request.TransChannelId, request.TranslateType, request.ChannelIds, request.IsDeleteAfterTranslate, adminId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
