using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class FormsController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] UploadRequest request, [FromForm] IFormFile file)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var form = await _formRepository.GetAsync(request.SiteId, request.FormId);
            if (form == null) return NotFound();

            var styles = await _formRepository.GetTableStylesAsync(form.Id);
            var style = styles.FirstOrDefault(x => x.AttributeName == request.AttributeName);
            if (style == null) return NotFound();

            var virtualUrl = string.Empty;
            var fileUrl = string.Empty;

            if (style.InputType == InputType.Image)
            {
                var (success, filePath, errorMessage) = await _pathManager.UploadImageAsync(site, file);
                if (!success)
                {
                    return this.Error(errorMessage);
                }

                virtualUrl = await _pathManager.GetVirtualUrlByPhysicalPathAsync(site, filePath);
                fileUrl = await _pathManager.ParseSiteUrlAsync(site, virtualUrl, true);
            }
            else if (style.InputType == InputType.File)
            {
                var (success, filePath, errorMessage) = await _pathManager.UploadFileAsync(site, file);
                if (!success)
                {
                    return this.Error(errorMessage);
                }

                virtualUrl = await _pathManager.GetVirtualUrlByPhysicalPathAsync(site, filePath);
                fileUrl = await _pathManager.ParseSiteUrlAsync(site, virtualUrl, true);
            }
            else
            {
                return NotFound();
            }

            return new UploadResult
            {
                AttributeName = style.AttributeName,
                VirtualUrl = virtualUrl,
                FileUrl = fileUrl,
            };
        }
    }
}
