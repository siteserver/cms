using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsController
    {
        [HttpPost, Route(RouteCreate)]
        public async Task<ActionResult<BoolResult>> Create([FromBody] CreateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.Contents))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);

            foreach (var summary in summaries)
            {
                await _createManager.CreateContentAsync(request.SiteId, summary.ChannelId, summary.Id);
            }

            return new BoolResult
            {
                Value = true
            };
        }

        public class CreateRequest : SiteRequest
        {
            public string ChannelContentIds { get; set; }
        }
    }
}
