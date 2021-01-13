using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class DashboardController
    {
        [HttpGet, Route(RouteUnCheckedList)]
        public async Task<ActionResult<GetUnCheckedListResult>> GetUnCheckedList()
        {
            var unCheckedList = new List<UnChecked>();

            if (await _authManager.IsSuperAdminAsync())
            {
                foreach (var site in await _siteRepository.GetSitesAsync())
                {
                    var count = await _contentRepository.GetCountCheckingAsync(site);
                    if (count > 0)
                    {
                        unCheckedList.Add(new UnChecked
                        {
                            SiteId = site.Id,
                            SiteName = site.SiteName,
                            Count = count
                        });
                    }
                }
            }
            else if (await _authManager.IsSiteAdminAsync())
            {
                var admin = await _authManager.GetAdminAsync();
                if (admin.SiteIds != null)
                {
                    foreach (var siteId in admin.SiteIds)
                    {
                        var site = await _siteRepository.GetAsync(siteId);
                        if (site == null) continue;

                        var count = await _contentRepository.GetCountCheckingAsync(site);
                        if (count > 0)
                        {
                            unCheckedList.Add(new UnChecked
                            {
                                SiteId = site.Id,
                                SiteName = site.SiteName,
                                Count = count
                            });
                        }
                    }
                }
            }

            var totalCount = unCheckedList.Sum(x => x.Count);

            return new GetUnCheckedListResult
            {
                UnCheckedList = unCheckedList,
                TotalCount = totalCount
            };
        }
    }
}