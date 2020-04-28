using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsContentController : ControllerBase
    {
        private const string Route = "cms/settings/settingsContent";

        private readonly IAuthManager _authManager;
        private readonly IOldPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentRepository _contentRepository;

        public SettingsContentController(IAuthManager authManager, IOldPluginManager pluginManager, ISiteRepository siteRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _contentRepository = contentRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<Site>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.SettingsContent))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            return site;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.SettingsContent))
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

            site.PageSize = request.PageSize;
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

            await _authManager.AddSiteLogAsync(request.SiteId, "修改内容设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
