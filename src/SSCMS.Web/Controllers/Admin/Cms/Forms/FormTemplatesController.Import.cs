using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Services;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormTemplatesController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteImport)]
        public async Task<ActionResult<BoolResult>> Import([FromQuery] SiteRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.FormTemplates))
            {
                return Unauthorized();
            }

            if (file == null)
            {
                return this.Error(Constants.ErrorUpload);
            }

            var fileName = PathUtils.GetFileName(file.FileName);

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".zip"))
            {
                return this.Error("表单模板导入失败，导入文件为Zip格式，请选择有效的文件上传");
            }

            var name = PathUtils.GetFileNameWithoutExtension(fileName);
            var site = await _siteRepository.GetAsync(request.SiteId);
            var templates = await _formManager.GetFormTemplatesAsync(site);
            if (templates.Any(x => StringUtils.EqualsIgnoreCase(name, x.Name)))
            {
                return this.Error($"表单模板导入失败，标识为 {name} 的模板已存在，请修改压缩包文件名称！");
            }

            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            await _pathManager.UploadAsync(file, filePath);

            var directoryPath = await _formManager.GetTemplateDirectoryPathAsync(site, false, name);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);
            _pathManager.ExtractZip(filePath, directoryPath);

            if (!FileUtils.IsFileExists(PathUtils.Combine(directoryPath, FormManager.MAIN_FILE_NAME)))
            {
                DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
                return this.Error($"表单模板导入失败，表单模板压缩包内必须包含index.html文件！");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
