using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesEditorController
	{
        [HttpPost, Route(RouteSettings)]
        public async Task<ActionResult<SettingsResult>> Settings([FromBody] SettingsRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.Settings.SiteId, MenuUtils.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.Settings.SiteId);
            if (site == null) return NotFound();

			if (request.Settings.TemplateType != TemplateType.ChannelTemplate)
			{
				if (!request.Settings.CreatedFileFullName.StartsWith("~") && !request.Settings.CreatedFileFullName.StartsWith("@"))
				{
                    request.Settings.CreatedFileFullName = PageUtils.Combine("@", request.Settings.CreatedFileFullName);
				}
			}
			else
			{
                request.Settings.CreatedFileFullName = request.Settings.CreatedFileFullName.TrimStart('~', '@');
                request.Settings.CreatedFileFullName = request.Settings.CreatedFileFullName.Replace("/", string.Empty);
			}

			Template template;

			if (request.Settings.TemplateId > 0)
			{
				var templateId = request.Settings.TemplateId;
				template = await _templateRepository.GetAsync(templateId);
				if (template.TemplateName != request.Settings.TemplateName)
				{
					var templateNameList = await _templateRepository.GetTemplateNamesAsync(request.Settings.SiteId, template.TemplateType);
					if (templateNameList.Contains(request.Settings.TemplateName))
					{
						return this.Error("模板修改失败，模板名称已存在！");
					}
				}
				Template previousTemplate = null;
				var isChanged = false;
				if (PathUtils.RemoveExtension(template.RelatedFileName) != PathUtils.RemoveExtension(request.Settings.RelatedFileName))//文件名改变
				{
					var fileNameList = await _templateRepository.GetRelatedFileNamesAsync(request.Settings.SiteId, template.TemplateType);
					foreach (var fileName in fileNameList)
					{
						var fileNameWithoutExtension = PathUtils.RemoveExtension(fileName);
						if (StringUtils.EqualsIgnoreCase(fileNameWithoutExtension, request.Settings.RelatedFileName))
						{
							return this.Error("模板修改失败，模板文件已存在！");
						}
					}

					isChanged = true;
				}

				if (GetTemplateFileExtension(template) != request.Settings.CreatedFileExtName)//文件后缀改变
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

				template.TemplateName = request.Settings.TemplateName;
				template.RelatedFileName = request.Settings.RelatedFileName + request.Settings.CreatedFileExtName;
				template.CreatedFileExtName = request.Settings.CreatedFileExtName;
				template.CreatedFileFullName = request.Settings.CreatedFileFullName + request.Settings.CreatedFileExtName;
                template.Content = request.Content;

                await _pathManager.WriteContentToTemplateFileAsync(site, template, request.Content, _authManager.AdminId);

				await _templateRepository.UpdateAsync(template);
				if (previousTemplate != null)
				{
					FileUtils.DeleteFileIfExists(await _pathManager.GetTemplateFilePathAsync(site, previousTemplate));
				}

				await _authManager.AddSiteLogAsync(request.Settings.SiteId,
					$"修改{template.TemplateType.GetDisplayName()}",
					$"模板名称:{template.TemplateName}");
			}
			else
			{
				var templateNameList = await _templateRepository.GetTemplateNamesAsync(request.Settings.SiteId, request.Settings.TemplateType);
				if (templateNameList.Contains(request.Settings.TemplateName))
				{
					return this.Error("模板添加失败，模板名称已存在！");
				}
				var fileNameList = await _templateRepository.GetRelatedFileNamesAsync(request.Settings.SiteId, request.Settings.TemplateType);
				if (ListUtils.ContainsIgnoreCase(fileNameList, request.Settings.RelatedFileName))
				{
					return this.Error("模板添加失败，模板文件已存在！");
				}

				template = new Template
				{
					SiteId = request.Settings.SiteId,
					TemplateName = request.Settings.TemplateName,
					TemplateType = request.Settings.TemplateType,
					RelatedFileName = request.Settings.RelatedFileName + request.Settings.CreatedFileExtName,
					CreatedFileExtName = request.Settings.CreatedFileExtName,
					CreatedFileFullName = request.Settings.CreatedFileFullName + request.Settings.CreatedFileExtName,
					DefaultTemplate = false
				};

				template.Id = await _templateRepository.InsertAsync(template);
				await _authManager.AddSiteLogAsync(request.Settings.SiteId,
					$"添加{template.TemplateType.GetDisplayName()}",
					$"模板名称:{template.TemplateName}");

                await _pathManager.WriteContentToTemplateFileAsync(site, template, request.Content, _authManager.AdminId);
			}

			var settings = await GetSettingsAsync(template, site);

            return new SettingsResult
            {
                Settings = settings
            };
        }
    }
}
