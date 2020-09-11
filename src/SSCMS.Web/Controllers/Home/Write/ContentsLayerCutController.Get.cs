using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsLayerCutController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.Translate))
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

            var sites = new List<object>();
            var channels = new List<object>();

            var siteIdList = await _authManager.GetSiteIdsAsync();
            foreach (var permissionSiteId in siteIdList)
            {
                var permissionSite = await _siteRepository.GetAsync(permissionSiteId);
                sites.Add(new
                {
                    permissionSite.Id,
                    permissionSite.SiteName
                });
            }

            var channelIdList = await _authManager.GetChannelIdsAsync(site.Id,
                Types.ContentPermissions.Add);
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
    }
}
