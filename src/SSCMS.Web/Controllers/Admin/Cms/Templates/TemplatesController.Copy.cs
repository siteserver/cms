using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesController
    {
        [HttpPost, Route(RouteCopy)]
        public async Task<ActionResult<GetResult>> Copy([FromBody] TemplateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var template = await _templateRepository.GetAsync(request.TemplateId);

            var templateName = template.TemplateName + "_复件";
            var relatedFileName = PathUtils.RemoveExtension(template.RelatedFileName) + "_复件";
            var createdFileFullName = PathUtils.RemoveExtension(template.CreatedFileFullName) + "_复件";

            var templateNameList = await _templateRepository.GetTemplateNamesAsync(request.SiteId, template.TemplateType);
            if (templateNameList.Contains(templateName))
            {
                return this.Error("模板复制失败，模板名称已存在！");
            }
            var fileNameList = await _templateRepository.GetRelatedFileNamesAsync(request.SiteId, template.TemplateType);
            if (ListUtils.ContainsIgnoreCase(fileNameList, relatedFileName))
            {
                return this.Error("模板复制失败，模板文件已存在！");
            }

            var templateInfo = new Template
            {
                SiteId = request.SiteId,
                TemplateName = templateName,
                TemplateType = template.TemplateType,
                RelatedFileName = relatedFileName + template.CreatedFileExtName,
                CreatedFileExtName = template.CreatedFileExtName,
                CreatedFileFullName = createdFileFullName + template.CreatedFileExtName,
                DefaultTemplate = false
            };

            var content = await _pathManager.GetTemplateContentAsync(site, template);

            var adminId = _authManager.AdminId;
            templateInfo.Id = await _templateRepository.InsertAsync(templateInfo);
            await _pathManager.WriteContentToTemplateFileAsync(site, templateInfo, content, adminId);

            return await GetResultAsync(site);
        }
    }
}
