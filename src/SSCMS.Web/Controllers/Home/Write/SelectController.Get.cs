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
            foreach (var siteId in siteIds)
            {
                var permissionSite = await _siteRepository.GetAsync(siteId);
                sites.Add(new Select<int>
                {
                    Value = permissionSite.Id,
                    Label = permissionSite.SiteName
                });
            }

            var site = await _siteRepository.GetAsync(siteIds[0]);

            var channel = await _channelRepository.GetAsync(site.Id);
            var root = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                return new
                {
                    Count = count
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
