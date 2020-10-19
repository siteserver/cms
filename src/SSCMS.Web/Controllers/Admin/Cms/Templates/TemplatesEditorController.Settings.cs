using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesEditorController
	{
        [HttpPost, Route(RouteSettings)]
        public async Task<ActionResult<SettingsResult>> Settings([FromBody] SettingsForm request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

			if (request.TemplateType != TemplateType.ChannelTemplate)
			{
				if (!request.CreatedFileFullName.StartsWith("~") && !request.CreatedFileFullName.StartsWith("@"))
				{
					request.CreatedFileFullName = PageUtils.Combine("@", request.CreatedFileFullName);
				}
			}
			else
			{
				request.CreatedFileFullName = request.CreatedFileFullName.TrimStart('~', '@');
				request.CreatedFileFullName = request.CreatedFileFullName.Replace("/", string.Empty);
			}

			Template template;

			if (request.TemplateId > 0)
			{
				var templateId = request.TemplateId;
				template = await _templateRepository.GetAsync(templateId);
				if (template.TemplateName != request.TemplateName)
				{
					var templateNameList = await _templateRepository.GetTemplateNamesAsync(request.SiteId, template.TemplateType);
					if (templateNameList.Contains(request.TemplateName))
					{
						return this.Error("模板修改失败，模板名称已存在！");
					}
				}
				Template previousTemplate = null;
				var isChanged = false;
				if (PathUtils.RemoveExtension(template.RelatedFileName) != PathUtils.RemoveExtension(request.RelatedFileName))//文件名改变
				{
					var fileNameList = await _templateRepository.GetRelatedFileNamesAsync(request.SiteId, template.TemplateType);
					foreach (var fileName in fileNameList)
					{
						var fileNameWithoutExtension = PathUtils.RemoveExtension(fileName);
						if (StringUtils.EqualsIgnoreCase(fileNameWithoutExtension, request.RelatedFileName))
						{
							return this.Error("模板修改失败，模板文件已存在！");
						}
					}

					isChanged = true;
				}

				if (GetTemplateFileExtension(template) != request.CreatedFileExtName)//文件后缀改变
				{
					isChanged = true;
				}

				if (isChanged)
				{
					previousTemplate = new Template
					{
						Id = template.Id,
						SiteId = template.SiteId,
						TemplateName = template.TemplateName,
						TemplateType = template.TemplateType,
						RelatedFileName = template.RelatedFileName,
						CreatedFileFullName = template.CreatedFileFullName,
						CreatedFileExtName = template.CreatedFileExtName,
						DefaultTemplate = template.DefaultTemplate
					};
				}

				template.TemplateName = request.TemplateName;
				template.RelatedFileName = request.RelatedFileName + request.CreatedFileExtName;
				template.CreatedFileExtName = request.CreatedFileExtName;
				template.CreatedFileFullName = request.CreatedFileFullName + request.CreatedFileExtName;

				await _templateRepository.UpdateAsync(template);
				if (previousTemplate != null)
				{
					FileUtils.DeleteFileIfExists(await _pathManager.GetTemplateFilePathAsync(site, previousTemplate));
				}

				await _authManager.AddSiteLogAsync(request.SiteId,
					$"修改{template.TemplateType.GetDisplayName()}",
					$"模板名称:{template.TemplateName}");
			}
			else
			{
				var templateNameList = await _templateRepository.GetTemplateNamesAsync(request.SiteId, request.TemplateType);
				if (templateNameList.Contains(request.TemplateName))
				{
					return this.Error("模板添加失败，模板名称已存在！");
				}
				var fileNameList = await _templateRepository.GetRelatedFileNamesAsync(request.SiteId, request.TemplateType);
				if (ListUtils.ContainsIgnoreCase(fileNameList, request.RelatedFileName))
				{
					return this.Error("模板添加失败，模板文件已存在！");
				}

				template = new Template
				{
					SiteId = request.SiteId,
					TemplateName = request.TemplateName,
					TemplateType = request.TemplateType,
					RelatedFileName = request.RelatedFileName + request.CreatedFileExtName,
					CreatedFileExtName = request.CreatedFileExtName,
					CreatedFileFullName = request.CreatedFileFullName + request.CreatedFileExtName,
					DefaultTemplate = false
				};

				template.Id = await _templateRepository.InsertAsync(template);
				await _authManager.AddSiteLogAsync(request.SiteId,
					$"添加{template.TemplateType.GetDisplayName()}",
					$"模板名称:{template.TemplateName}");
			}

			var settings = await GetSettingsAsync(template, site);

            return new SettingsResult
            {
                Settings = settings
            };
        }
    }
}
