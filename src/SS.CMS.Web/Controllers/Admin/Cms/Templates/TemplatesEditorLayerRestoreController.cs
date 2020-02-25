using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Cms.Templates
{
    [Route("admin/cms/templates/templatesEditorLayerRestore")]
    public partial class TemplatesEditorLayerRestoreController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public TemplatesEditorLayerRestoreController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] TemplateRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var logs = await DataProvider.TemplateLogRepository.GetLogIdWithNameListAsync(request.SiteId, request.TemplateId);
            var logId = request.LogId;
            if (logId == 0 && logs.Any())
            {
                logId = logs.First().Key;
            }

            var original = logId == 0 ? string.Empty : await DataProvider.TemplateLogRepository.GetTemplateContentAsync(logId);

            var template = await DataProvider.TemplateRepository.GetAsync(request.TemplateId);
            var modified = await DataProvider.TemplateRepository.GetTemplateContentAsync(site, template);

            return new GetResult
            {
                Logs = logs,
                LogId = logId,
                Original = original,
                Modified = modified
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<GetResult>> Delete([FromBody] TemplateRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            await DataProvider.TemplateLogRepository.DeleteAsync(request.LogId);

            var logs = await DataProvider.TemplateLogRepository.GetLogIdWithNameListAsync(request.SiteId, request.TemplateId);
            var logId = 0;
            if (logs.Any())
            {
                logId = logs.First().Key;
            }

            var original = logId == 0 ? string.Empty : await DataProvider.TemplateLogRepository.GetTemplateContentAsync(logId);

            var template = await DataProvider.TemplateRepository.GetAsync(request.TemplateId);
            var modified = await DataProvider.TemplateRepository.GetTemplateContentAsync(site, template);

            return new GetResult
            {
                Logs = logs,
                LogId = logId,
                Original = original,
                Modified = modified
            };
        }
    }
}
