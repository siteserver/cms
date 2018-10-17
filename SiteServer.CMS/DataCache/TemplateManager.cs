using System;
using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.DataCache
{
	public static class TemplateManager
	{
        private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(TemplateManager));
        private static readonly object SyncRoot = new object();

        public static TemplateInfo GetTemplateInfo(int siteId, int templateId)
        {
            TemplateInfo templateInfo = null;
            var templateInfoDictionary = GetTemplateInfoDictionaryBySiteId(siteId);

            if (templateInfoDictionary != null && templateInfoDictionary.ContainsKey(templateId))
            {
                templateInfo = templateInfoDictionary[templateId];
            }
            return templateInfo;
        }

        public static string GetCreatedFileFullName(int siteId, int templateId)
        {
            var createdFileFullName = string.Empty;

            var templateInfo = GetTemplateInfo(siteId, templateId);
            if (templateInfo != null)
            {
                createdFileFullName = templateInfo.CreatedFileFullName;
            }

            return createdFileFullName;
        }

        public static string GetTemplateName(int siteId, int templateId)
        {
            var templateName = string.Empty;

            var templateInfo = GetTemplateInfo(siteId, templateId);
            if (templateInfo != null)
            {
                templateName = templateInfo.TemplateName;
            }

            return templateName;
        }

        public static TemplateInfo GetTemplateInfoByTemplateName(int siteId, TemplateType templateType, string templateName)
        {
            TemplateInfo info = null;

            var templateInfoDictionary = GetTemplateInfoDictionaryBySiteId(siteId);
            if (templateInfoDictionary != null)
            {
                foreach (var templateInfo in templateInfoDictionary.Values)
                {
                    if (templateInfo.TemplateType == templateType && templateInfo.TemplateName == templateName)
                    {
                        info = templateInfo;
                        break;
                    }
                }
            }

            return info;
        }

        public static TemplateInfo GetDefaultTemplateInfo(int siteId, TemplateType templateType)
        {
            TemplateInfo info = null;

            var templateInfoDictionary = GetTemplateInfoDictionaryBySiteId(siteId);
            if (templateInfoDictionary != null)
            {
                foreach (var templateInfo in templateInfoDictionary.Values)
                {
                    if (templateInfo.TemplateType == templateType && templateInfo.IsDefault)
                    {
                        info = templateInfo;
                        break;
                    }
                }
            }

            return info ?? new TemplateInfo
            {
                SiteId = siteId,
                TemplateType = templateType
            };
        }

        public static int GetDefaultTemplateId(int siteId, TemplateType templateType)
        {
            var id = 0;

            var templateInfoDictionary = GetTemplateInfoDictionaryBySiteId(siteId);
            if (templateInfoDictionary != null)
            {
                foreach (var templateInfo in templateInfoDictionary.Values)
                {
                    if (templateInfo.TemplateType == templateType && templateInfo.IsDefault)
                    {
                        id = templateInfo.Id;
                        break;
                    }
                }
            }

            return id;
        }

        public static int GetTemplateIdByTemplateName(int siteId, TemplateType templateType, string templateName)
        {
            var id = 0;

            var templateInfoDictionary = GetTemplateInfoDictionaryBySiteId(siteId);
            if (templateInfoDictionary != null)
            {
                foreach (var templateInfo in templateInfoDictionary.Values)
                {
                    if (templateInfo.TemplateType == templateType && templateInfo.TemplateName == templateName)
                    {
                        id = templateInfo.Id;
                        break;
                    }
                }
            }

            return id;
        }

        public static List<int> GetAllFileTemplateIdList(int siteId)
        {
            var list = new List<int>();

            var templateInfoDictionary = GetTemplateInfoDictionaryBySiteId(siteId);
            if (templateInfoDictionary == null) return list;

            foreach (var templateInfo in templateInfoDictionary.Values)
            {
                if (templateInfo.TemplateType == TemplateType.FileTemplate)
                {
                    list.Add(templateInfo.Id);
                }
            }

            return list;
        }

	    private static Dictionary<int, TemplateInfo> GetTemplateInfoDictionaryBySiteId(int siteId, bool flush = false)
        {
            var dictionary = GetCacheDictionary();

            Dictionary<int, TemplateInfo> templateInfoDictionary = null;

            if (!flush && dictionary.ContainsKey(siteId))
            {
                templateInfoDictionary = dictionary[siteId];
            }

            if (templateInfoDictionary == null)
            {
                templateInfoDictionary = DataProvider.TemplateDao.GetTemplateInfoDictionaryBySiteId(siteId);

                if (templateInfoDictionary != null)
                {
                    UpdateCache(dictionary, templateInfoDictionary, siteId);
                }
            }
            return templateInfoDictionary;
        }

        private static void UpdateCache(Dictionary<int, Dictionary<int, TemplateInfo>> dictionary, Dictionary<int, TemplateInfo> templateInfoDictionary, int siteId)
        {
            lock (SyncRoot)
            {
                dictionary[siteId] = templateInfoDictionary;
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

        private static Dictionary<int, Dictionary<int, TemplateInfo>> GetCacheDictionary()
        {
            var dictionary = DataCacheManager.Get<Dictionary<int, Dictionary<int, TemplateInfo>>>(CacheKey);
            if (dictionary == null)
            {
                dictionary = new Dictionary<int, Dictionary<int, TemplateInfo>>();
                DataCacheManager.InsertHours(CacheKey, dictionary, 24);
            }
            return dictionary;
        }

        public static string GetTemplateFilePath(SiteInfo siteInfo, TemplateInfo templateInfo)
        {
            string filePath;
            if (templateInfo.TemplateType == TemplateType.IndexPageTemplate)
            {
                filePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, siteInfo.SiteDir, templateInfo.RelatedFileName);
            }
            else if (templateInfo.TemplateType == TemplateType.ContentTemplate)
            {
                filePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, siteInfo.SiteDir, DirectoryUtils.PublishmentSytem.Template, DirectoryUtils.PublishmentSytem.Content, templateInfo.RelatedFileName);
            }
            else
            {
                filePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, siteInfo.SiteDir, DirectoryUtils.PublishmentSytem.Template, templateInfo.RelatedFileName);
            }
            return filePath;
        }

	    public static TemplateInfo GetIndexPageTemplateInfo(int siteId)
	    {
	        var templateId = GetDefaultTemplateId(siteId, TemplateType.IndexPageTemplate);
            TemplateInfo templateInfo = null;
            if (templateId != 0)
            {
                templateInfo = GetTemplateInfo(siteId, templateId);
            }

            return templateInfo ?? GetDefaultTemplateInfo(siteId, TemplateType.IndexPageTemplate);
        }

        public static TemplateInfo GetChannelTemplateInfo(int siteId, int channelId)
        {
            var templateId = 0;
            if (siteId == channelId)
            {
                templateId = GetDefaultTemplateId(siteId, TemplateType.IndexPageTemplate);
            }
            else
            {
                var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (nodeInfo != null)
                {
                    templateId = nodeInfo.ChannelTemplateId;
                }
            }

            TemplateInfo templateInfo = null;
            if (templateId != 0)
            {
                templateInfo = GetTemplateInfo(siteId, templateId);
            }

            return templateInfo ?? GetDefaultTemplateInfo(siteId, TemplateType.ChannelTemplate);
        }

        public static TemplateInfo GetContentTemplateInfo(int siteId, int channelId)
        {
            var templateId = 0;
            var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            if (nodeInfo != null)
            {
                templateId = nodeInfo.ContentTemplateId;
            }

            TemplateInfo templateInfo = null;
            if (templateId != 0)
            {
                templateInfo = GetTemplateInfo(siteId, templateId);
            }

            return templateInfo ?? GetDefaultTemplateInfo(siteId, TemplateType.ContentTemplate);
        }

        public static TemplateInfo GetFileTemplateInfo(int siteId, int fileTemplateId)
        {
            var templateId = fileTemplateId;

            TemplateInfo templateInfo = null;
            if (templateId != 0)
            {
                templateInfo = GetTemplateInfo(siteId, templateId);
            }

            return templateInfo ?? GetDefaultTemplateInfo(siteId, TemplateType.FileTemplate);
        }

        public static void WriteContentToTemplateFile(SiteInfo siteInfo, TemplateInfo templateInfo, string content, string administratorName)
        {
            if (content == null) content = string.Empty;
            var filePath = GetTemplateFilePath(siteInfo, templateInfo);
            FileUtils.WriteText(filePath, templateInfo.Charset, content);

            if (templateInfo.Id > 0)
            {
                var logInfo = new TemplateLogInfo(0, templateInfo.Id, templateInfo.SiteId, DateTime.Now, administratorName, content.Length, content);
                DataProvider.TemplateLogDao.Insert(logInfo);
            }
        }

        public static int GetIndexTempalteId(int siteId)
        {
            return GetDefaultTemplateId(siteId, TemplateType.IndexPageTemplate);
        }

        public static int GetChannelTempalteId(int siteId, int channelId)
        {
            var templateId = 0;

            var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            if (nodeInfo != null)
            {
                templateId = nodeInfo.ChannelTemplateId;
            }

            if (templateId == 0)
            {
                templateId = GetDefaultTemplateId(siteId, TemplateType.ChannelTemplate);
            }

            return templateId;
        }

        public static int GetContentTempalteId(int siteId, int channelId)
        {
            var templateId = 0;

            var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            if (nodeInfo != null)
            {
                templateId = nodeInfo.ContentTemplateId;
            }

            if (templateId == 0)
            {
                templateId = GetDefaultTemplateId(siteId, TemplateType.ContentTemplate);
            }

            return templateId;
        }

        public static string GetTemplateContent(SiteInfo siteInfo, TemplateInfo templateInfo)
        {
            var filePath = GetTemplateFilePath(siteInfo, templateInfo);
            return GetContentByFilePath(filePath, templateInfo.Charset);
        }

        public static string GetIncludeContent(SiteInfo siteInfo, string file, ECharset charset)
        {
            var filePath = PathUtility.MapPath(siteInfo, PathUtility.AddVirtualToPath(file));
            return GetContentByFilePath(filePath, charset);
        }

        public static string GetContentByFilePath(string filePath, ECharset charset = ECharset.utf_8)
        {
            try
            {
                var content = DataCacheManager.Get<string>(filePath);
                if (content != null) return content;
                
                if (FileUtils.IsFileExists(filePath))
                {
                    content = FileUtils.ReadText(filePath, charset);
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
