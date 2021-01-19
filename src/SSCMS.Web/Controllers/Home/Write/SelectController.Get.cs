using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class SelectController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var siteIds = await _authManager.GetSiteIdsAsync();

            if (siteIds.Count == 0)
            {
                return new GetResult
                {
                    Unauthorized = true
                };
            }

            var sites = new List<Select<int>>();
            foreach (var id in siteIds)
            {
                var permissionSite = await _siteRepository.GetAsync(id);
                sites.Add(new Select<int>
                {
                    Value = permissionSite.Id,
                    Label = permissionSite.SiteName
                });
            }

            var siteId = siteIds[0];
            var site = await _siteRepository.GetAsync(siteId);

            var channel = await _channelRepository.GetAsync(siteId);
            var enabledChannelIds = await _authManager.GetContentPermissionsChannelIdsAsync(site.Id);
            var visibleChannelIds = await _authManager.GetVisibleChannelIdsAsync(enabledChannelIds);
            var root = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var visible = visibleChannelIds.Contains(summary.Id);
                var disabled = !enabledChannelIds.Contains(summary.Id);
                var current = await _contentRepository.GetSummariesAsync(site, summary);

                if (!visible) return null;

                return new
                {
                    current.Count,
                    Disabled = disabled
                };
            });

            return new GetResult
            {
                Unauthorized = false,
                Sites = sites,
                SiteId = site.Id,
                Root = root
            };
        }
    }
}
