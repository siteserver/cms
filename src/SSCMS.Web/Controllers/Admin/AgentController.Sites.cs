using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class AgentController
    {
        [HttpGet, Route(RouteSites)]
        public async Task<ActionResult<SitesResult>> Sites([FromQuery] AgentRequest request)
        {
            var agentCheck = CheckAgent(request.SecurityKey);
            if (agentCheck != null) return agentCheck;

            var sites = await _siteRepository.GetSitesWithChildrenAsync(0, async x => new
            {
                LocalUrl = await _pathManager.GetSiteUrlAsync(x, true),
                SiteUrl = await _pathManager.GetSiteUrlAsync(x, false)
            });

            var rootSiteId = await _siteRepository.GetIdByIsRootAsync();

            return new SitesResult
            {
                Sites = sites,
                RootSiteId = rootSiteId,
            };
        }
    }
}