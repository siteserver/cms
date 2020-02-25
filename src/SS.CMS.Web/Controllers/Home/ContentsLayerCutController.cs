using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Home
{
    [Route("home/contentsLayerCut")]
    public partial class ContentsLayerCutController : ControllerBase
    {
        private const string Route = "";
        private const string RouteGetChannels = "actions/getChannels";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;

        public ContentsLayerCutController(IAuthManager authManager, ICreateManager createManager)
        {
            _authManager = authManager;
            _createManager = createManager;
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

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var retVal = new List<IDictionary<string, object>>();
            foreach (var contentId in request.ContentIds)
            {
                var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channel, contentId);
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
                var permissionSite = await DataProvider.SiteRepository.GetAsync(permissionSiteId);
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
                var permissionChannelInfo = await DataProvider.ChannelRepository.GetAsync(permissionChannelId);
                channels.Add(new
                {
                    permissionChannelInfo.Id,
                    ChannelName = await DataProvider.ChannelRepository.GetChannelNameNavigationAsync(site.Id, permissionChannelId)
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
            var channelIdList = await auth.UserPermissions.GetChannelIdListAsync(request.SiteId, Constants.ChannelPermissions.ContentAdd);
            foreach (var permissionChannelId in channelIdList)
            {
                var permissionChannelInfo = await DataProvider.ChannelRepository.GetAsync(permissionChannelId);
                channels.Add(new
                {
                    permissionChannelInfo.Id,
                    ChannelName = await DataProvider.ChannelRepository.GetChannelNameNavigationAsync(request.SiteId, permissionChannelId)
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
                !await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                    Constants.ChannelPermissions.ContentTranslate))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return NotFound();

            foreach (var contentId in request.ContentIds)
            {
                await ContentUtility.TranslateAsync(site, request.ChannelId, contentId, request.TargetSiteId, request.TargetChannelId, TranslateContentType.Cut, _createManager);
            }

            await auth.AddSiteLogAsync(request.SiteId, request.ChannelId, "转移内容", string.Empty);

            await _createManager.TriggerContentChangedEventAsync(request.SiteId, request.ChannelId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
