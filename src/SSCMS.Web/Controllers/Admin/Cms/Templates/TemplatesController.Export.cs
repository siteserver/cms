using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesController
    {
        [HttpGet, Route(RouteExport)]
        public async Task<FileResult> Export([FromQuery] TemplateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.Templates))
            {
                return null;
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return null;

            var template = await _templateRepository.GetAsync(request.TemplateId);
            if (template == null) return null;

            var filePath = _pathManager.GetTemporaryFilesPath(template.RelatedFileName);
            FileUtils.DeleteFileIfExists(filePath);

            var content = await _pathManager.GetTemplateContentAsync(site, template);
            await FileUtils.WriteTextAsync(filePath, content);

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, MediaTypeNames.Application.Octet, Path.GetFileName(filePath));
        }
    }
}
