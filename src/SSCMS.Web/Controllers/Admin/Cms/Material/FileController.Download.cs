using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class FileController
    {
        [HttpGet, Route(RouteActionsDownload)]
        public async Task<ActionResult> ActionsDownload([FromQuery] DownloadRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialFile))
            {
                return Unauthorized();
            }

            var file = await _materialFileRepository.GetAsync(request.Id);
            if (file == null || string.IsNullOrEmpty(file.Url)) return NotFound();
            var filePath = PathUtils.Combine(_settingsManager.WebRootPath, file.Url);
            if (!FileUtils.IsFileExists(filePath)) return NotFound();

            return this.Download(filePath);
        }
    }
}
