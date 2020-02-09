using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    public partial class PagesContentsController
    {
        [HttpPost, Route(RouteCreate)]
        public async Task<BoolResult> Create([FromBody] CreateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);

            foreach (var summary in summaries)
            {
                await CreateManager.CreateContentAsync(request.SiteId, summary.ChannelId, summary.Id);
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
