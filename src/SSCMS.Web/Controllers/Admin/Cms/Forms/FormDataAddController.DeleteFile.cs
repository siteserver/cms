using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormDataAddController
    {
        [HttpPost, Route(RouteDeleteFile)]
        public async Task<ActionResult<BoolResult>> DeleteFile([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.FormList))
            {
                return Unauthorized();
            }

            var filePath = PathUtils.Combine(_pathManager.ContentRootPath, request.FileUrl);
            FileUtils.DeleteFileIfExists(filePath);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
