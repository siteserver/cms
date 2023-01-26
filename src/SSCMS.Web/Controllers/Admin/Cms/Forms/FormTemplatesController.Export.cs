using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormTemplatesController
    {
        [HttpPost, Route(RouteExport)]
        public async Task<ActionResult<StringResult>> Export([FromBody] ExportRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.FormTemplates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);


            var directoryPath = await _formManager.GetTemplateDirectoryPathAsync(site, request.IsSystem, request.Name);
            if (!DirectoryUtils.IsDirectoryExists(directoryPath))
            {
                return NotFound();
            }

            var fileName = $"{request.Name}.zip";
            var zipFilePath = _pathManager.GetTemporaryFilesPath(fileName);
            FileUtils.DeleteFileIfExists(zipFilePath);

            _pathManager.CreateZip(zipFilePath, directoryPath);
            var downloadUrl = _pathManager.GetTemporaryFilesUrl(fileName);

            return new StringResult
            {
                Value = downloadUrl
            };
        }
    }
}
