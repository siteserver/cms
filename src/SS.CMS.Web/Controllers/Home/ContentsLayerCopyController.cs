using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Home
{
    [Route("home/contentsLayerCopy")]
    public partial class ContentsLayerCopyController : ControllerBase
    {
        private const string Route = "";
        private const string RouteGetChannels = "actions/getChannels";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public ContentsLayerCopyController(IAuthManager authManager, IPathManager pathManager, ICreateManager createManager, IDatabaseManager databaseManager, IPluginManager pluginManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
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

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            var auth = await _authManager.GetUserAsync();
            if (!auth.IsUserLoggin ||
                !await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentTranslate))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return NotFound();

            var retVal = new List<IDictionary<string, object>>();
            foreach (var contentId in request.ContentIds)
            {
                var contentInfo = await _contentRepository.GetAsync(site, channelInfo, contentId);
                if (contentInfo == null) continue;

                var dict = contentInfo.ToDictionary();
                dict["checkState"] =
                    CheckManager.GetCheckState(site, contentInfo);
                retVal.Add(dict);
            }

            var sites = new List<object>();
            var channels = new List<object>();

            var siteIdList = await auth.UserPermissions.GetSiteIdListAsync();
            foreach (var permissionSiteId in siteIdList)
            {
                var permissionSite = await _siteRepository.GetAsync(permissionSiteId);
                sites.Add(new
                {
                    permissionSite.Id,
                    permissionSite.SiteName
                });
            }

            var channelIdList = await auth.UserPermissions.GetChannelIdListAsync(site.Id,
                Constants.ChannelPermissions.ContentAdd);
            foreach (var permissionChannelId in channelIdList)
            {
                var permissionChannelInfo = await _channelRepository.GetAsync(permissionChannelId);
                channels.Add(new
                {
                    permissionChannelInfo.Id,
                    ChannelName = await _channelRepository.GetChannelNameNavigationAsync(site.Id, permissionChannelId)
                });
            }

            return new GetResult
            {
                Value = retVal,
                Sites = sites,
                Channels = channels,
                Site = site
            };
        }

        [HttpGet, Route(RouteGetChannels)]
        public async Task<ActionResult<GetChannelsResult>> GetChannels([FromQuery]SiteRequest request)
        {
            var auth = await _authManager.GetUserAsync();
            var channels = new List<object>();
            var channelIdList = await auth.UserPermissions.GetChannelIdListAsync(request.SiteId,
                Constants.ChannelPermissions.ContentAdd);
            foreach (var permissionChannelId in channelIdList)
            {
                var permissionChannelInfo = await _channelRepository.GetAsync(permissionChannelId);
                channels.Add(new
                {
                    permissionChannelInfo.Id,
                    ChannelName = await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, permissionChannelId)
                });
            }

            return new GetChannelsResult
            {
                Channels = channels
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            var auth = await _authManager.GetUserAsync();
            if (!auth.IsUserLoggin ||
                !await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentTranslate))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return NotFound();

            foreach (var contentId in request.ContentIds)
            {
                await ContentUtility.TranslateAsync(_pathManager, _databaseManager, _pluginManager, site, request.ChannelId, contentId, request.TargetSiteId, request.TargetChannelId, request.CopyType, _createManager);
            }

            await auth.AddSiteLogAsync(request.SiteId, request.ChannelId, "复制内容", string.Empty);

            await _createManager.TriggerContentChangedEventAsync(request.SiteId, request.ChannelId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
