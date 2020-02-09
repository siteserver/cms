using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.CMS.Repositories
{
    public partial class TemplateRepository
    {
        public async Task<List<Template>> GetTemplateListByTypeAsync(int siteId, TemplateType templateType)
        {
            var list = await GetAllAsync(siteId);
            return list.Where(x => x.TemplateType == templateType).ToList();
        }

        public async Task<Template> GetTemplateByTemplateNameAsync(int siteId, TemplateType templateType, string templateName)
        {
            var list = await GetAllAsync(siteId);
            return list.FirstOrDefault(x => x.TemplateType == templateType && x.TemplateName == templateName);
        }

        public async Task<Template> GetDefaultTemplateAsync(int siteId, TemplateType templateType)
        {
            var list = await GetAllAsync(siteId);
            var template = list.FirstOrDefault(x => x.TemplateType == templateType && x.Default);

            return template ?? new Template
            {
                SiteId = siteId,
                TemplateType = templateType
            };
        }

        public async Task<int> GetDefaultTemplateIdAsync(int siteId, TemplateType templateType)
        {
            var list = await GetAllAsync(siteId);
            var template = list.FirstOrDefault(x => x.TemplateType == templateType && x.Default);
            return template?.Id ?? 0;
        }

        public async Task<int> GetTemplateIdByTemplateNameAsync(int siteId, TemplateType templateType, string templateName)
        {
            var list = await GetAllAsync(siteId);
            var template = list.FirstOrDefault(x => x.TemplateType == templateType && x.TemplateName == templateName);
            return template?.Id ?? 0;
        }

        public async Task<List<string>> GetTemplateNameListAsync(int siteId, TemplateType templateType)
        {
            var list = await GetAllAsync(siteId);
            return list.Where(x => x.TemplateType == templateType).Select(x => x.TemplateName).ToList();
        }

        public async Task<List<string>> GetRelatedFileNameListAsync(int siteId, TemplateType templateType)
        {
            var list = await GetAllAsync(siteId);
            return list.Where(x => x.TemplateType == templateType).Select(x => x.RelatedFileName).ToList();
        }

        public async Task<List<int>> GetAllFileTemplateIdListAsync(int siteId)
        {
            var list = await GetAllAsync(siteId);
            return list.Where(x => x.TemplateType == TemplateType.FileTemplate).Select(x => x.Id).ToList();
        }

        public async Task<string> GetCreatedFileFullNameAsync(int templateId)
        {
            var createdFileFullName = string.Empty;

            var template = await GetAsync(templateId);
            if (template != null)
            {
                createdFileFullName = template.CreatedFileFullName;
            }

            return createdFileFullName;
        }

        public async Task<string> GetTemplateNameAsync(int templateId)
        {
            var templateName = string.Empty;

            var template = await GetAsync(templateId);
            if (template != null)
            {
                templateName = template.TemplateName;
            }

            return templateName;
        }

        public string GetTemplateFilePath(Site site, Template template)
        {
            string filePath;
            if (template.TemplateType == TemplateType.IndexPageTemplate)
            {
                filePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, site.SiteDir, template.RelatedFileName);
            }
            else if (template.TemplateType == TemplateType.ContentTemplate)
            {
                filePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, site.SiteDir, DirectoryUtils.PublishmentSytem.Template, DirectoryUtils.PublishmentSytem.Content, template.RelatedFileName);
            }
            else
            {
                filePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, site.SiteDir, DirectoryUtils.PublishmentSytem.Template, template.RelatedFileName);
            }
            return filePath;
        }

        public async Task<Template> GetIndexPageTemplateAsync(int siteId)
        {
            var templateId = await GetDefaultTemplateIdAsync(siteId, TemplateType.IndexPageTemplate);
            Template template = null;
            if (templateId != 0)
            {
                template = await GetAsync(templateId);
            }

            return template ?? await GetDefaultTemplateAsync(siteId, TemplateType.IndexPageTemplate);
        }

        public async Task<Template> GetChannelTemplateAsync(int siteId, int channelId)
        {
            var templateId = 0;
            if (siteId == channelId)
            {
                templateId = await GetDefaultTemplateIdAsync(siteId, TemplateType.IndexPageTemplate);
            }
            else
            {
                var nodeInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
                if (nodeInfo != null)
                {
                    templateId = nodeInfo.ChannelTemplateId;
                }
            }

            Template template = null;
            if (templateId != 0)
            {
                template = await GetAsync(templateId);
            }

            return template ?? await GetDefaultTemplateAsync(siteId, TemplateType.ChannelTemplate);
        }

        public async Task<Template> GetContentTemplateAsync(int siteId, int channelId)
        {
            var templateId = 0;
            var nodeInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
            if (nodeInfo != null)
            {
                templateId = nodeInfo.ContentTemplateId;
            }

            Template template = null;
            if (templateId != 0)
            {
                template = await GetAsync(templateId);
            }

            return template ?? await GetDefaultTemplateAsync(siteId, TemplateType.ContentTemplate);
        }

        public async Task<Template> GetFileTemplateAsync(int siteId, int fileTemplateId)
        {
            var templateId = fileTemplateId;

            Template template = null;
            if (templateId != 0)
            {
                template = await GetAsync(templateId);
            }

            return template ?? await GetDefaultTemplateAsync(siteId, TemplateType.FileTemplate);
        }

        private async Task WriteContentToTemplateFileAsync(Site site, Template template, string content, string administratorName)
        {
            if (content == null) content = string.Empty;
            var filePath = GetTemplateFilePath(site, template);
            FileUtils.WriteText(filePath, content);

            if (template.Id > 0)
            {
                var logInfo = new TemplateLog
                {
                    Id = 0,
                    TemplateId = template.Id,
                    SiteId = template.SiteId,
                    AddDate = DateTime.Now,
                    AddUserName = administratorName,
                    ContentLength = content.Length,
                    TemplateContent = content
                };
                await DataProvider.TemplateLogRepository.InsertAsync(logInfo);
            }
        }

        public async Task<int> GetIndexTemplateIdAsync(int siteId)
        {
            return await GetDefaultTemplateIdAsync(siteId, TemplateType.IndexPageTemplate);
        }

        public string GetTemplateContent(Site site, Template template)
        {
            var filePath = GetTemplateFilePath(site, template);
            return GetContentByFilePath(filePath);
        }

        public string GetIncludeContent(Site site, string file)
        {
            var filePath = PathUtility.MapPath(site, PathUtility.AddVirtualToPath(file));
            return GetContentByFilePath(filePath);
        }

        public string GetContentByFilePath(string filePath)
        {
            try
            {
                var content = DataCacheManager.Get<string>(filePath);
                if (content != null) return content;

                if (FileUtils.IsFileExists(filePath))
                {
                    content = FileUtils.ReadText(filePath);
                }

                DataCacheManager.Insert(filePath, content, TimeSpan.FromHours(12), filePath);
                return content;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
