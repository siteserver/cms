using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesLayerSelectController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var rootSiteId = await _siteRepository.GetIdByIsRootAsync();

            var sites = await _siteRepository.GetPureSitesWithChildrenAsync(0, async x => new
            {
                x.Id,
                x.Root,
                x.SiteName,
                SiteUrl = await _pathManager.GetSiteUrlAsync(x, false)
            });

            return new GetResult
            {
                Sites = sites,
            };
        }
    }
}