using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class IndexController
    {
        [Authorize(Roles = AuthTypes.Roles.Administrator)]
        [HttpPost, Route(RouteActionsCache)]
        public async Task<ActionResult<IntResult>> Cache([FromBody] SiteRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            await _channelRepository.CacheAllAsync(site);
            var channelSummaries = await _channelRepository.GetSummariesAsync(site.Id);
            await _contentRepository.CacheAllListAndCountAsync(site, channelSummaries);
            await _contentRepository.CacheAllEntityAsync(site, channelSummaries);

            return new IntResult
            {
                Value = channelSummaries.Count
            };
        }
    }
}
