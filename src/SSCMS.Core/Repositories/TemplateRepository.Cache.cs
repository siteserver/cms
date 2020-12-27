using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public partial class TemplateRepository
    {
        public async Task<List<Template>> GetTemplatesByTypeAsync(int siteId, TemplateType templateType)
        {
            var list = new List<Template>();

            var summaries = await GetSummariesAsync(siteId);
            foreach (var summary in summaries.Where(x => x.TemplateType == templateType))
            {
                list.Add(await GetAsync(summary.Id));
            }

            return list;
        }

        public async Task<Template> GetTemplateByTemplateNameAsync(int siteId, TemplateType templateType, string templateName)
        {
            var summaries = await GetSummariesAsync(siteId);
            var summary = summaries
                .FirstOrDefault(x => x.TemplateType == templateType && x.TemplateName == templateName);
            return summary != null ? await GetAsync(summary.Id) : null;
        }

        public async Task<bool> ExistsAsync(int siteId, TemplateType templateType, string templateName)
        {
            var summaries = await GetSummariesAsync(siteId);
            return summaries
                .Exists(x => x.TemplateType == templateType && x.TemplateName == templateName);
        }

        public async Task<Template> GetDefaultTemplateAsync(int siteId, TemplateType templateType)
        {
            var summaries = await GetSummariesAsync(siteId);
            var summary = summaries
                .FirstOrDefault(x => x.TemplateType == templateType && x.DefaultTemplate);
            return summary != null ? await GetAsync(summary.Id) : new Template
            {
                SiteId = siteId,
                TemplateType = templateType
            };
        }

        public async Task<int> GetDefaultTemplateIdAsync(int siteId, TemplateType templateType)
        {
            var summaries = await GetSummariesAsync(siteId);
            var summary = summaries
                .FirstOrDefault(x => x.TemplateType == templateType && x.DefaultTemplate);
            return summary?.Id ?? 0;
        }

        public async Task<int> GetTemplateIdByTemplateNameAsync(int siteId, TemplateType templateType, string templateName)
        {
            var summaries = await GetSummariesAsync(siteId);
            var summary = summaries
                .FirstOrDefault(x => x.TemplateType == templateType && x.TemplateName == templateName);
            return summary?.Id ?? 0;
        }

        public async Task<List<string>> GetTemplateNamesAsync(int siteId, TemplateType templateType)
        {
            var summaries = await GetSummariesAsync(siteId);
            return summaries.Where(x => x.TemplateType == templateType).Select(x => x.TemplateName).ToList();
        }

        public async Task<List<string>> GetRelatedFileNamesAsync(int siteId, TemplateType templateType)
        {
            var list = new List<string>();

            var summaries = await GetSummariesAsync(siteId);
            foreach (var summary in summaries.Where(x => x.TemplateType == templateType))
            {
                var template = await GetAsync(summary.Id);
                list.Add(template.RelatedFileName);
            }

            return list;
        }

        public async Task<List<int>> GetAllFileTemplateIdsAsync(int siteId)
        {
            var summaries = await GetSummariesAsync(siteId);
            return summaries.Where(x => x.TemplateType == TemplateType.FileTemplate).Select(x => x.Id).ToList();
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

        public async Task<Template> GetChannelTemplateAsync(int siteId, Channel channel)
        {
            int templateId;
            if (siteId == channel.Id)
            {
                templateId = await GetDefaultTemplateIdAsync(siteId, TemplateType.IndexPageTemplate);
            }
            else
            {
                templateId = channel.ChannelTemplateId;
            }

            Template template = null;
            if (templateId != 0)
            {
                template = await GetAsync(templateId);
            }

            return template ?? await GetDefaultTemplateAsync(siteId, TemplateType.ChannelTemplate);
        }

        public async Task<Template> GetContentTemplateAsync(int siteId, Channel channel, int templateId)
        {
            Template template = null;
            if (templateId > 0)
            {
                template = await GetAsync(templateId);
            }
            if (template == null && channel.ContentTemplateId > 0)
            {
                template = await GetAsync(channel.ContentTemplateId);
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

        public async Task<int> GetIndexTemplateIdAsync(int siteId)
        {
            return await GetDefaultTemplateIdAsync(siteId, TemplateType.IndexPageTemplate);
        }

        public async Task<string> GetImportTemplateNameAsync(int siteId, TemplateType templateType, string templateName)
        {
            string importTemplateName;
            if (templateName.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                var lastTemplateName = templateName.Substring(templateName.LastIndexOf("_", StringComparison.Ordinal) + 1);
                var firstTemplateName = templateName.Substring(0, templateName.Length - lastTemplateName.Length);
                var templateNameCount = TranslateUtils.ToInt(lastTemplateName);
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
