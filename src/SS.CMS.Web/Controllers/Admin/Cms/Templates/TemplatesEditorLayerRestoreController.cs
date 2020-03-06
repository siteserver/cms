using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Cms.Templates
{
    [Route("admin/cms/templates/templatesEditorLayerRestore")]
    public partial class TemplatesEditorLayerRestoreController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ITemplateLogRepository _templateLogRepository;

        public TemplatesEditorLayerRestoreController(IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository, ITemplateRepository templateRepository, ITemplateLogRepository templateLogRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _templateRepository = templateRepository;
            _templateLogRepository = templateLogRepository;
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

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var logs = await _templateLogRepository.GetLogIdWithNameListAsync(request.SiteId, request.TemplateId);
            var logId = request.LogId;
            if (logId == 0 && logs.Any())
            {
                logId = logs.First().Key;
            }

            var original = logId == 0 ? string.Empty : await _templateLogRepository.GetTemplateContentAsync(logId);

            var template = await _templateRepository.GetAsync(request.TemplateId);
            var modified = await _pathManager.GetTemplateContentAsync(site, template);

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

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            await _templateLogRepository.DeleteAsync(request.LogId);

            var logs = await _templateLogRepository.GetLogIdWithNameListAsync(request.SiteId, request.TemplateId);
            var logId = 0;
            if (logs.Any())
            {
                logId = logs.First().Key;
            }

            var original = logId == 0 ? string.Empty : await _templateLogRepository.GetTemplateContentAsync(logId);

            var template = await _templateRepository.GetAsync(request.TemplateId);
            var modified = await _pathManager.GetTemplateContentAsync(site, template);

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
