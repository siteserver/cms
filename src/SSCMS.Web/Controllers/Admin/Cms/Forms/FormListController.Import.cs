using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormListController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteImport)]
        public async Task<ActionResult<BoolResult>> Import([FromQuery] SiteRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.FormList))
            {
                return Unauthorized();
            }

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            var extName = PathUtils.GetExtension(fileName);

            if (!FileUtils.IsZip(extName))
            {
                return this.Error("上传格式错误，请上传zip压缩包!");
            }

            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            FileUtils.DeleteFileIfExists(filePath);
            await _pathManager.UploadAsync(file, filePath);

            var directoryPath = _pathManager.GetTemporaryFilesPath("form");
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            _pathManager.ExtractZip(filePath, directoryPath);

            await _formManager.ImportFormAsync(request.SiteId, directoryPath, false);

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            //TODO
            //foreach (string name in HttpContext.Current.Request.Files)
            //{
            //    var postFile = HttpContext.Current.Request.Files[name];

            //    if (postFile == null)
            //    {
            //        return this.Error("Could not read zip from body");
            //    }

            //    var filePath = Context.UtilsApi.GetTemporaryFilesPath("form.zip");
            //    FormUtils.DeleteFileIfExists(filePath);

            //    if (!FormUtils.EqualsIgnoreCase(Path.GetExtension(postFile.FileName), ".zip"))
            //    {
            //        return this.Error("zip file extension is not correct");
            //    }

            //    postFile.SaveAs(filePath);

            //    var directoryPath = Context.UtilsApi.GetTemporaryFilesPath("form");
            //    FormUtils.DeleteDirectoryIfExists(directoryPath);
            //    Context.UtilsApi.ExtractZip(filePath, directoryPath);

            //    FormBox.ImportForm(siteId, directoryPath, true);
            //}

            return Ok(new
            {
                Value = true
            });
        }
    }
}
