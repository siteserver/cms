using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteImport)]
        public async Task<ActionResult<BoolResult>> Import([FromQuery] ImportRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            var extName = PathUtils.GetExtension(fileName);

            if (!StringUtils.EqualsIgnoreCase(".html", extName))
            {
                return this.Error("上传格式错误，请上传html文件!");
            }

            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            FileUtils.DeleteFileIfExists(filePath);
            await _pathManager.UploadAsync(file, filePath);

            // T_系统首页模板.html

            var templateName = StringUtils.ReplaceStartsWithIgnoreCase(fileName, "T_", string.Empty);
            templateName = PathUtils.RemoveExtension(templateName);
            templateName = await _templateRepository.GetImportTemplateNameAsync(request.SiteId, request.TemplateType, templateName);
            var relatedFileName = $"T_{templateName}{extName}";
            var createdFileFullName = request.TemplateType == TemplateType.FileTemplate ? $"@/{templateName}{extName}" : string.Empty;

            var templateNameList = await _templateRepository.GetTemplateNamesAsync(request.SiteId, request.TemplateType);
            if (templateNameList.Contains(templateName))
            {
                return this.Error("模板复制失败，模板名称已存在！");
            }
            var fileNameList = await _templateRepository.GetRelatedFileNamesAsync(request.SiteId, request.TemplateType);
            if (ListUtils.ContainsIgnoreCase(fileNameList, relatedFileName))
            {
                return this.Error("模板复制失败，模板文件已存在！");
            }

            var templateInfo = new Template
            {
                SiteId = request.SiteId,
                TemplateName = templateName,
                TemplateType = request.TemplateType,
                RelatedFileName = relatedFileName,
                CreatedFileExtName = extName,
                CreatedFileFullName = createdFileFullName,
                DefaultTemplate = false
            };

            var content = await FileUtils.ReadTextAsync(filePath);

            var adminId = _authManager.AdminId;
            templateInfo.Id = await _templateRepository.InsertAsync(templateInfo);
            await _pathManager.WriteContentToTemplateFileAsync(site, templateInfo, content, adminId);

            FileUtils.DeleteFileIfExists(filePath);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
