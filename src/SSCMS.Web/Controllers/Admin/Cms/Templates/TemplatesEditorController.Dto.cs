using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Core.Extensions;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesEditorController
	{
        public class GetResult
        {
            public Template Template { get; set; }
        }

        public class TemplateRequest
        {
            public int SiteId { get; set; }
            public int TemplateId { get; set; }
            public TemplateType TemplateType { get; set; }
        }

		private string GetTemplateFileExtension(Template template)
		{
			string extension;
			if (template.TemplateType == TemplateType.IndexPageTemplate || template.TemplateType == TemplateType.FileTemplate)
			{
				extension = PathUtils.GetExtension(template.CreatedFileFullName);
			}
			else
			{
				extension = template.CreatedFileExtName;
			}
			return extension;
		}

		private async Task CreatePagesAsync(Template template)
		{
			if (template.TemplateType == TemplateType.FileTemplate)
			{
				await _createManager.CreateFileAsync(template.SiteId, template.Id);
			}
			else if (template.TemplateType == TemplateType.IndexPageTemplate)
			{
				if (template.DefaultTemplate)
				{
					await _createManager.CreateChannelAsync(template.SiteId, template.SiteId);
				}
			}
		}

		private async Task<ActionResult<GetResult>> SaveAsync(Site site, Template request)
		{
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

            var adminId = await _authManager.GetAdminIdAsync();
			if (request.Id > 0)
			{
				var templateId = request.Id;
				template = await _templateRepository.GetAsync(templateId);
				if (template.TemplateName != request.TemplateName)
				{
					var templateNameList = await _templateRepository.GetTemplateNameListAsync(request.SiteId, template.TemplateType);
					if (templateNameList.Contains(request.TemplateName))
					{
						return this.Error("模板修改失败，模板名称已存在！");
					}
				}
				Template previousTemplate = null;
				var isChanged = false;
				if (PathUtils.RemoveExtension(template.RelatedFileName) != PathUtils.RemoveExtension(request.RelatedFileName))//文件名改变
				{
					var fileNameList = await _templateRepository.GetRelatedFileNameListAsync(request.SiteId, template.TemplateType);
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

				await _templateRepository.UpdateAsync(_pathManager, site, template, request.Content, adminId);
				if (previousTemplate != null)
				{
					FileUtils.DeleteFileIfExists(await _pathManager.GetTemplateFilePathAsync(site, previousTemplate));
				}
				await CreatePagesAsync(template);

				await _authManager.AddSiteLogAsync(request.SiteId,
					$"修改{template.TemplateType.GetDisplayName()}",
					$"模板名称:{template.TemplateName}");
			}
			else
			{
				var templateNameList = await _templateRepository.GetTemplateNameListAsync(request.SiteId, request.TemplateType);
				if (templateNameList.Contains(request.TemplateName))
				{
					return this.Error("模板添加失败，模板名称已存在！");
				}
				var fileNameList = await _templateRepository.GetRelatedFileNameListAsync(request.SiteId, request.TemplateType);
				if (StringUtils.ContainsIgnoreCase(fileNameList, request.RelatedFileName))
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

				template.Id = await _templateRepository.InsertAsync(_pathManager, site, template, request.Content, adminId);
				await CreatePagesAsync(template);
				await _authManager.AddSiteLogAsync(request.SiteId,
					$"添加{template.TemplateType.GetDisplayName()}",
					$"模板名称:{template.TemplateName}");
			}

			return new GetResult
			{
				Template = await GetTemplateResultAsync(template, site)
			};
		}

        private async Task<Template> GetTemplateResultAsync(Template templateInfo, Site site)
        {
            var template = new Template
            {
                Id = templateInfo.Id,
                SiteId = site.Id,
                DefaultTemplate = templateInfo.DefaultTemplate,
                TemplateType = templateInfo.TemplateType,
                TemplateName = templateInfo.TemplateName
            };
            if (templateInfo.Id > 0)
            {
                template.CreatedFileExtName = GetTemplateFileExtension(templateInfo);
                template.RelatedFileName = PathUtils.RemoveExtension(templateInfo.RelatedFileName);
                template.CreatedFileFullName = PathUtils.RemoveExtension(templateInfo.CreatedFileFullName);
                template.Content = await _pathManager.GetTemplateContentAsync(site, templateInfo);
            }
            else
            {
                template.RelatedFileName = "T_";
                template.CreatedFileFullName = template.TemplateType == TemplateType.ChannelTemplate ? "index" : "@/";
                template.CreatedFileExtName = ".html";
            }

            return template;
        }
	}
}
