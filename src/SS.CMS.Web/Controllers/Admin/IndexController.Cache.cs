using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;

namespace SS.CMS.Web.Controllers.Admin
{
    public partial class IndexController
    {
        [HttpPost, Route(RouteActionsCache)]
        public async Task<ActionResult<IntResult>> Cache([FromBody] SiteRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin || auth.Administrator == null)
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            await _channelRepository.CacheAllAsync(site);
            var channelSummaries = await _channelRepository.GetSummaryAsync(site.Id);
            await _contentRepository.CacheAllListAndCountAsync(site, channelSummaries);
            await _contentRepository.CacheAllEntityAsync(site, channelSummaries);

            return new IntResult
            {
                Value = channelSummaries.Count
            };
        }
    }
}
