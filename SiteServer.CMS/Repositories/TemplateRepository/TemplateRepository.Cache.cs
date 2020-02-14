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
        public async Task<List<TemplateSummary>> GetSummariesAsync(int siteId)
        {
            var list = new List<TemplateSummary>();

            var templateTypes = TranslateUtils.GetEnums<TemplateType>();
            foreach (var templateType in templateTypes)
            {
                var summaries = await GetSummariesAsync(siteId, templateType);
                list.AddRange(summaries);
            }

            return list;
        }

        public async Task<List<Template>> GetTemplateListByTypeAsync(int siteId, TemplateType templateType)
        {
            var list = new List<Template>();

            var summaries = await GetSummariesAsync(siteId, templateType);
            foreach (var summary in summaries)
            {
                list.Add(await GetAsync(summary.Id));
            }

            return list;
        }

        public async Task<Template> GetTemplateByTemplateNameAsync(int siteId, TemplateType templateType, string templateName)
        {
            var summaries = await GetSummariesAsync(siteId, templateType);
            var summary = summaries
                .FirstOrDefault(x => x.TemplateName == templateName);
            return summary != null ? await GetAsync(summary.Id) : null;
        }

        public async Task<bool> ExistsAsync(int siteId, TemplateType templateType, string templateName)
        {
            var summaries = await GetSummariesAsync(siteId, templateType);
            return summaries
                .Exists(x => x.TemplateName == templateName);
        }

        public async Task<Template> GetDefaultTemplateAsync(int siteId, TemplateType templateType)
        {
            var summaries = await GetSummariesAsync(siteId, templateType);
            var summary = summaries
                .FirstOrDefault(x => x.Default);
            return summary != null ? await GetAsync(summary.Id) : new Template
            {
                SiteId = siteId,
                TemplateType = templateType
            };
        }

        public async Task<int> GetDefaultTemplateIdAsync(int siteId, TemplateType templateType)
        {
            var summaries = await GetSummariesAsync(siteId, templateType);
            var summary = summaries
                .FirstOrDefault(x => x.Default);
            return summary?.Id ?? 0;
        }

        public async Task<int> GetTemplateIdByTemplateNameAsync(int siteId, TemplateType templateType, string templateName)
        {
            var summaries = await GetSummariesAsync(siteId, templateType);
            var summary = summaries
                .FirstOrDefault(x => x.TemplateName == templateName);
            return summary?.Id ?? 0;
        }

        public async Task<List<string>> GetTemplateNameListAsync(int siteId, TemplateType templateType)
        {
            var summaries = await GetSummariesAsync(siteId, templateType);
            return summaries.Select(x => x.TemplateName).ToList();
        }

        public async Task<List<string>> GetRelatedFileNameListAsync(int siteId, TemplateType templateType)
        {
            var list = new List<string>();

            var summaries = await GetSummariesAsync(siteId, templateType);
            foreach (var summary in summaries)
            {
                var template = await GetAsync(summary.Id);
                list.Add(template.RelatedFileName);
            }

            return list;
        }

        public async Task<List<int>> GetAllFileTemplateIdListAsync(int siteId)
        {
            var summaries = await GetSummariesAsync(siteId, TemplateType.FileTemplate);
            return summaries.Select(x => x.Id).ToList();
        }

        public async Task<string> GetCreatedFileFullNameAsync(int templateId)
        {
            var template = await GetAsync(templateId);
            return template != null ? template.CreatedFileFullName : string.Empty;
        }

        public async Task<string> GetTemplateNameAsync(int templateId)
        {
            var template = await GetAsync(templateId);
            return template != null ? template.TemplateName : string.Empty;
        }

        public async Task<string> GetTemplateFilePathAsync(Site site, Template template)
        {
            string filePath;
            if (template.TemplateType == TemplateType.IndexPageTemplate)
            {
                filePath = await PathUtility.GetSitePathAsync(site, template.RelatedFileName);
            }
            else if (template.TemplateType == TemplateType.ContentTemplate)
            {
                filePath = await PathUtility.GetSitePathAsync(site, DirectoryUtils.PublishmentSytem.Template, DirectoryUtils.PublishmentSytem.Content, template.RelatedFileName);
            }
            else
            {
                filePath = await PathUtility.GetSitePathAsync(site, DirectoryUtils.PublishmentSytem.Template, template.RelatedFileName);
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

        private async Task WriteContentToTemplateFileAsync(Site site, Template template, string content, int adminId)
        {
            if (content == null) content = string.Empty;
            var filePath = await GetTemplateFilePathAsync(site, template);
            FileUtils.WriteText(filePath, content);

            if (template.Id > 0)
            {
                var logInfo = new TemplateLog
                {
                    Id = 0,
                    TemplateId = template.Id,
                    SiteId = template.SiteId,
                    AddDate = DateTime.Now,
                    AdminId = adminId,
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

        public async Task<string> GetTemplateContentAsync(Site site, Template template)
        {
            var filePath = await GetTemplateFilePathAsync(site, template);
            return await GetContentByFilePathAsync(filePath);
        }

        public async Task<string> GetIncludeContentAsync(Site site, string file)
        {
            var filePath = await PathUtility.MapPathAsync(site, PathUtility.AddVirtualToPath(file));
            return await GetContentByFilePathAsync(filePath);
        }

        public async Task<string> GetContentByFilePathAsync(string filePath)
        {
            try
            {
                var content = DataCacheManager.Get<string>(filePath);
                if (content != null) return content;

                if (FileUtils.IsFileExists(filePath))
                {
                    content = await FileUtils.ReadTextAsync(filePath);
                }

                DataCacheManager.Insert(filePath, content, TimeSpan.FromHours(12), filePath);
                return content;
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<string> GetImportTemplateNameAsync(int siteId, TemplateType templateType, string templateName)
        {
            string importTemplateName;
            if (templateName.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                var templateNameCount = 0;
                var lastTemplateName = templateName.Substring(templateName.LastIndexOf("_", StringComparison.Ordinal) + 1);
                var firstTemplateName = templateName.Substring(0, templateName.Length - lastTemplateName.Length);
                try
                {
                    templateNameCount = int.Parse(lastTemplateName);
                }
                catch
                {
                    // ignored
                }
                templateNameCount++;
                importTemplateName = firstTemplateName + templateNameCount;
            }
            else
            {
                importTemplateName = templateName + "_1";
            }

            var exists = await ExistsAsync(siteId, templateType, importTemplateName);
            if (exists)
            {
                importTemplateName = await GetImportTemplateNameAsync(siteId, templateType, importTemplateName);
            }

            return importTemplateName;
        }
    }
}
