using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class SelectController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SiteRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);

            var channel = await _channelRepository.GetAsync(site.Id);
            var root = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                return new
                {
                    Count = count
                };
            });

            return new SubmitResult
            {
                Root = root
            };
        }
    }
}
