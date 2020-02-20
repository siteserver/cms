using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    [RoutePrefix("pages/cms/settings/settingsCrossSiteTrans")]
    public partial class PagesSettingsCrossSiteTransController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<ObjectResult<Site>> GetConfig([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigCrossSiteTrans))
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
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigCrossSiteTrans))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            site.IsCrossSiteTransChecked = request.IsCrossSiteTransChecked;

            await DataProvider.SiteRepository.UpdateAsync(site);

            await auth.AddSiteLogAsync(request.SiteId, "修改默认跨站转发设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
