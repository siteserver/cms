using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesEditorLayerRestoreController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<GetResult>> Delete([FromBody] TemplateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.Templates))
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
