using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Templates
{
    [RoutePrefix("pages/cms/templates/templatesEditorLayerRestore")]
    public partial class PagesTemplateEditorLayerRestoreController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] TemplateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Templates))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

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
        public async Task<GetResult> Delete([FromBody] TemplateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Templates))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

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
