using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;

namespace SS.CMS.Web.Controllers.Admin.Cms.Settings
{
    [Route("admin/cms/settings/settingsContent")]
    public partial class SettingsContentController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentRepository _contentRepository;

        public SettingsContentController(IAuthManager authManager, IPluginManager pluginManager, ISiteRepository siteRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _contentRepository = contentRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<Site>> Get([FromQuery] SiteRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigContents))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            return site;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigContents))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            site.IsSaveImageInTextEditor = request.IsSaveImageInTextEditor;

            var isReCalculate = false;
            if (request.IsAutoPageInTextEditor)
            {
                if (site.IsAutoPageInTextEditor == false)
                {
                    isReCalculate = true;
                }
                else if (site.AutoPageWordNum != request.AutoPageWordNum)
                {
                    isReCalculate = true;
                }
            }

            site.IsAutoPageInTextEditor = request.IsAutoPageInTextEditor;
            site.AutoPageWordNum = request.AutoPageWordNum;
            site.IsContentTitleBreakLine = request.IsContentTitleBreakLine;
            site.IsContentSubTitleBreakLine = request.IsContentSubTitleBreakLine;
            site.IsAutoCheckKeywords = request.IsAutoCheckKeywords;

            site.CheckContentLevel = request.CheckContentLevel;
            site.CheckContentDefaultLevel = request.CheckContentDefaultLevel;

            await _siteRepository.UpdateAsync(site);

            if (isReCalculate)
            {
                await _contentRepository.SetAutoPageContentToSiteAsync(_pluginManager, site);
            }

            await auth.AddSiteLogAsync(request.SiteId, "修改内容设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
