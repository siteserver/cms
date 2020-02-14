using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    public partial class PagesContentsSearchController
    {
        [HttpPost, Route(RouteColumns)]
        public async Task<BoolResult> Columns([FromBody] ColumnsRequest request)
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

            site.SearchListColumns = request.AttributeNames;

            await DataProvider.SiteRepository.UpdateAsync(site);

            return new BoolResult
            {
                Value = true
            };
        }

        public class ColumnsRequest : SiteRequest
        {
            public List<string> AttributeNames { get; set; }
        }
    }
}
