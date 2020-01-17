using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;


namespace SiteServer.CMS.DataCache
{
	public static class TemplateManager
	{
        private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(TemplateManager));
        private static readonly object SyncRoot = new object();

        public static async Task<Template> GetTemplateAsync(int siteId, int templateId)
        {
            Template template = null;
            var templateDictionary = await GetTemplateDictionaryBySiteIdAsync(siteId);

            if (templateDictionary != null && templateDictionary.ContainsKey(templateId))
            {
                template = templateDictionary[templateId];
            }
            return template;
        }

        public static async Task<string> GetCreatedFileFullNameAsync(int siteId, int templateId)
        {
            var createdFileFullName = string.Empty;

            var template = await GetTemplateAsync(siteId, templateId);
            if (template != null)
            {
                createdFileFullName = template.CreatedFileFullName;
            }

            return createdFileFullName;
        }

        public static async Task<string> GetTemplateNameAsync(int siteId, int templateId)
        {
            var templateName = string.Empty;

            var template = await GetTemplateAsync(siteId, templateId);
            if (template != null)
            {
                templateName = template.TemplateName;
            }

            return templateName;
        }

        public static async Task<Template> GetTemplateByTemplateNameAsync(int siteId, TemplateType templateType, string templateName)
        {
            Template info = null;

            var templateDictionary = await GetTemplateDictionaryBySiteIdAsync(siteId);
            if (templateDictionary != null)
            {
                foreach (var template in templateDictionary.Values)
                {
                    if (template.TemplateType == templateType && template.TemplateName == templateName)
                    {
                        info = template;
                        break;
                    }
                }
            }

            return info;
        }

        public static async Task<Template> GetDefaultTemplateAsync(int siteId, TemplateType templateType)
        {
            Template info = null;

            var templateDictionary = await GetTemplateDictionaryBySiteIdAsync(siteId);
            if (templateDictionary != null)
            {
                foreach (var template in templateDictionary.Values)
                {
                    if (template.TemplateType == templateType && template.Default)
                    {
                        info = template;
                        break;
                    }
                }
            }

            return info ?? new Template
            {
                SiteId = siteId,
                TemplateType = templateType
            };
        }

        public static async Task<int> GetDefaultTemplateIdAsync(int siteId, TemplateType templateType)
        {
            var id = 0;

            var templateDictionary = await GetTemplateDictionaryBySiteIdAsync(siteId);
            if (templateDictionary != null)
            {
                foreach (var template in templateDictionary.Values)
                {
                    if (template.TemplateType == templateType && template.Default)
                    {
                        id = template.Id;
                        break;
                    }
                }
            }

            return id;
        }

        public static async Task<int> GetTemplateIdByTemplateNameAsync(int siteId, TemplateType templateType, string templateName)
        {
            var id = 0;

            var templateDictionary = await GetTemplateDictionaryBySiteIdAsync(siteId);
            if (templateDictionary != null)
            {
                foreach (var template in templateDictionary.Values)
                {
                    if (template.TemplateType == templateType && template.TemplateName == templateName)
                    {
                        id = template.Id;
                        break;
                    }
                }
            }

            return id;
        }

        public static async Task<List<int>> GetAllFileTemplateIdListAsync(int siteId)
        {
            var list = new List<int>();

            var templateDictionary = await GetTemplateDictionaryBySiteIdAsync(siteId);
            if (templateDictionary == null) return list;

            foreach (var template in templateDictionary.Values)
            {
                if (template.TemplateType == TemplateType.FileTemplate)
                {
                    list.Add(template.Id);
                }
            }

            return list;
        }

	    private static async Task<Dictionary<int, Template>> GetTemplateDictionaryBySiteIdAsync(int siteId, bool flush = false)
        {
            var dictionary = GetCacheDictionary();

            Dictionary<int, Template> templateDictionary = null;

            if (!flush && dictionary.ContainsKey(siteId))
            {
                templateDictionary = dictionary[siteId];
            }

            if (templateDictionary == null)
            {
                templateDictionary = await DataProvider.TemplateRepository.GetTemplateDictionaryBySiteIdAsync(siteId);

                if (templateDictionary != null)
                {
                    UpdateCache(dictionary, templateDictionary, siteId);
                }
            }
            return templateDictionary;
        }

        private static void UpdateCache(Dictionary<int, Dictionary<int, Template>> dictionary, Dictionary<int, Template> templateDictionary, int siteId)
        {
            lock (SyncRoot)
            {
                dictionary[siteId] = templateDictionary;
            }
        }

        public static void RemoveCache(int siteId)
        {
            var dictionary = GetCacheDictionary();

            lock (SyncRoot)
            {
                dictionary.Remove(siteId);
            }
        }

        private static Dictionary<int, Dictionary<int, Template>> GetCacheDictionary()
        {
            var dictionary = DataCacheManager.Get<Dictionary<int, Dictionary<int, Template>>>(CacheKey);
            if (dictionary == null)
            {
                dictionary = new Dictionary<int, Dictionary<int, Template>>();
                DataCacheManager.InsertHours(CacheKey, dictionary, 24);
            }
            return dictionary;
        }

        public static string GetTemplateFilePath(Site site, Template template)
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

	    public static async Task<Template> GetIndexPageTemplateAsync(int siteId)
	    {
	        var templateId = await GetDefaultTemplateIdAsync(siteId, TemplateType.IndexPageTemplate);
            Template template = null;
            if (templateId != 0)
            {
                template = await GetTemplateAsync(siteId, templateId);
            }

            return template ?? await GetDefaultTemplateAsync(siteId, TemplateType.IndexPageTemplate);
        }

        public static async Task<Template> GetChannelTemplateAsync(int siteId, int channelId)
        {
            var templateId = 0;
            if (siteId == channelId)
            {
                templateId = await GetDefaultTemplateIdAsync(siteId, TemplateType.IndexPageTemplate);
            }
            else
            {
                var nodeInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (nodeInfo != null)
                {
                    templateId = nodeInfo.ChannelTemplateId;
                }
            }

            Template template = null;
            if (templateId != 0)
            {
                template = await GetTemplateAsync(siteId, templateId);
            }

            return template ?? await GetDefaultTemplateAsync(siteId, TemplateType.ChannelTemplate);
        }

        public static async Task<Template> GetContentTemplateAsync(int siteId, int channelId)
        {
            var templateId = 0;
            var nodeInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
            if (nodeInfo != null)
            {
                templateId = nodeInfo.ContentTemplateId;
            }

            Template template = null;
            if (templateId != 0)
            {
                template = await GetTemplateAsync(siteId, templateId);
            }

            return template ?? await GetDefaultTemplateAsync(siteId, TemplateType.ContentTemplate);
        }

        public static async Task<Template> GetFileTemplateAsync(int siteId, int fileTemplateId)
        {
            var templateId = fileTemplateId;

            Template template = null;
            if (templateId != 0)
            {
                template = await GetTemplateAsync(siteId, templateId);
            }

            return template ?? await GetDefaultTemplateAsync(siteId, TemplateType.FileTemplate);
        }

        public static async Task WriteContentToTemplateFileAsync(Site site, Template template, string content, string administratorName)
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

        public static async Task<int> GetIndexTemplateIdAsync(int siteId)
        {
            return await GetDefaultTemplateIdAsync(siteId, TemplateType.IndexPageTemplate);
        }

        public static async Task<int> GetChannelTemplateIdAsync(int siteId, int channelId)
        {
            var templateId = 0;

            var nodeInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
            if (nodeInfo != null)
            {
                templateId = nodeInfo.ChannelTemplateId;
            }

            if (templateId == 0)
            {
                templateId = await GetDefaultTemplateIdAsync(siteId, TemplateType.ChannelTemplate);
            }

            return templateId;
        }

        public static async Task<int> GetContentTemplateIdAsync(int siteId, int channelId)
        {
            var templateId = 0;

            var nodeInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
            if (nodeInfo != null)
            {
                templateId = nodeInfo.ContentTemplateId;
            }

            if (templateId == 0)
            {
                templateId = await GetDefaultTemplateIdAsync(siteId, TemplateType.ContentTemplate);
            }

            return templateId;
        }

        public static string GetTemplateContent(Site site, Template template)
        {
            var filePath = GetTemplateFilePath(site, template);
            return GetContentByFilePath(filePath);
        }

        public static string GetIncludeContent(Site site, string file)
        {
            var filePath = PathUtility.MapPath(site, PathUtility.AddVirtualToPath(file));
            return GetContentByFilePath(filePath);
        }

        public static string GetContentByFilePath(string filePath)
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
