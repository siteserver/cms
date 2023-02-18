using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormListController
    {
        [HttpPost, Route(RouteExport)]
        public async Task<ActionResult<StringResult>> Export([FromBody] FormRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.FormList))
            {
                return Unauthorized();
            }

            var form = await _formRepository.GetAsync(request.SiteId, request.FormId);
            if (form == null) return NotFound();

            var fileName = $"{form.Title}.zip";
            var directoryPath = _pathManager.GetTemporaryFilesPath("form");
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            await _formManager.ExportFormAsync(form.SiteId, directoryPath, form.Id);

            _pathManager.CreateZip(_pathManager.GetTemporaryFilesPath(fileName), directoryPath);

            var url = _pathManager.GetTemporaryFilesUrl($"{fileName}");

            return new StringResult
            {
                Value = url
            };
        }
    }
}
