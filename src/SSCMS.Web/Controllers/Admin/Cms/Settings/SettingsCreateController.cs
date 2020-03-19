using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Dto.Request;
using SSCMS.Dto.Result;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [Route("admin/cms/settings/settingsCreate")]
    public partial class SettingsCreateController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;

        public SettingsCreateController(IAuthManager authManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ObjectResult<Site>>> GetConfig([FromQuery] SiteRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigCreateRule))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            return new ObjectResult<Site>
            {
                Value = site
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigCreateRule))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

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

            await _siteRepository.UpdateAsync(site);

            await _authManager.AddSiteLogAsync(request.SiteId, "修改页面生成设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
