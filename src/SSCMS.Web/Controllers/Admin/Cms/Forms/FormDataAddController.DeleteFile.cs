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
            var formPermission = MenuUtils.GetFormPermission(request.FormId);
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, formPermission))
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
