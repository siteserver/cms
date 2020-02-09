using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    [RoutePrefix("pages/cms/settings/settingsCreate")]
    public partial class PagesSettingsCreateController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<ObjectResult<Site>> GetConfig([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigCreateRule))
            {
                return Request.Unauthorized<ObjectResult<Site>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            return new ObjectResult<Site>
            {
                Value = site
            };
        }

        [HttpPost, Route(Route)]
        public async Task<BoolResult> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigCreateRule))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            site.IsCreateContentIfContentChanged = request.IsCreateContentIfContentChanged;
            site.IsCreateChannelIfChannelChanged = request.IsCreateChannelIfChannelChanged;
            site.IsCreateShowPageInfo = request.IsCreateShowPageInfo;
            site.IsCreateIe8Compatible = request.IsCreateIe8Compatible;
            site.IsCreateBrowserNoCache = request.IsCreateBrowserNoCache;
            site.IsCreateJsIgnoreError = request.IsCreateJsIgnoreError;
            site.IsCreateWithJQuery = request.IsCreateWithJQuery;
            site.IsCreateDoubleClick = request.IsCreateDoubleClick;
            site.CreateStaticMaxPage = request.CreateStaticMaxPage;
            site.IsCreateUseDefaultFileName = request.IsCreateUseDefaultFileName;
            site.CreateDefaultFileName = request.CreateDefaultFileName;
            site.IsCreateStaticContentByAddDate = request.IsCreateStaticContentByAddDate;
            site.CreateStaticContentAddDate = request.CreateStaticContentAddDate;

            await DataProvider.SiteRepository.UpdateAsync(site);

            await auth.AddSiteLogAsync(request.SiteId, "修改页面生成设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
