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
            if (string.IsNullOrEmpty(request.SecurityKey))
            {
                return this.Error("系统参数不足");
            }
            if (_settingsManager.SecurityKey != request.SecurityKey)
            {
                return this.Error("SecurityKey不正确");
            }

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