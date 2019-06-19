using System;
using System.Collections.Generic;
using System.Linq;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.Repositories
{
    public partial class TemplateRepository
    {
        private readonly object _syncRoot = new object();

        public TemplateInfo GetTemplateInfo(int siteId, int templateId)
        {
            TemplateInfo templateInfo = null;
            var templateInfoDictionary = GetTemplateInfoDictionaryBySiteId(siteId);

            if (templateInfoDictionary != null && templateInfoDictionary.ContainsKey(templateId))
            {
                templateInfo = templateInfoDictionary[templateId];
            }
            return templateInfo;
        }

        public string GetCreatedFileFullName(int siteId, int templateId)
        {
            var createdFileFullName = string.Empty;

            var templateInfo = GetTemplateInfo(siteId, templateId);
            if (templateInfo != null)
            {
                createdFileFullName = templateInfo.CreatedFileFullName;
            }

            return createdFileFullName;
        }

        public string GetTemplateName(int siteId, int templateId)
        {
            var templateName = string.Empty;

            var templateInfo = GetTemplateInfo(siteId, templateId);
            if (templateInfo != null)
            {
                templateName = templateInfo.TemplateName;
            }

            return templateName;
        }

        public TemplateInfo GetTemplateInfoByTemplateName(int siteId, TemplateType templateType, string templateName)
        {
            TemplateInfo info = null;

            var templateInfoDictionary = GetTemplateInfoDictionaryBySiteId(siteId);
            if (templateInfoDictionary != null)
            {
                foreach (var templateInfo in templateInfoDictionary.Values)
                {
                    if (templateInfo.Type == templateType && templateInfo.TemplateName == templateName)
                    {
                        info = templateInfo;
                        break;
                    }
                }
            }

            return info;
        }

        public TemplateInfo GetDefaultTemplateInfo(int siteId, TemplateType templateType)
        {
            TemplateInfo info = null;

            var templateInfoDictionary = GetTemplateInfoDictionaryBySiteId(siteId);
            if (templateInfoDictionary != null)
            {
                foreach (var templateInfo in templateInfoDictionary.Values)
                {
                    if (templateInfo.Type == templateType && templateInfo.IsDefault)
                    {
                        info = templateInfo;
                        break;
                    }
                }
            }

            return info ?? new TemplateInfo
            {
                SiteId = siteId,
                Type = templateType
            };
        }

        public int GetDefaultTemplateId(int siteId, TemplateType templateType)
        {
            var id = 0;

            var templateInfoDictionary = GetTemplateInfoDictionaryBySiteId(siteId);
            if (templateInfoDictionary != null)
            {
                foreach (var templateInfo in templateInfoDictionary.Values)
                {
                    if (templateInfo.Type == templateType && templateInfo.IsDefault)
                    {
                        id = templateInfo.Id;
                        break;
                    }
                }
            }

            return id;
        }

        public int GetTemplateIdByTemplateName(int siteId, TemplateType templateType, string templateName)
        {
            var id = 0;

            var templateInfoDictionary = GetTemplateInfoDictionaryBySiteId(siteId);
            if (templateInfoDictionary != null)
            {
                foreach (var templateInfo in templateInfoDictionary.Values)
                {
                    if (templateInfo.Type == templateType && templateInfo.TemplateName == templateName)
                    {
                        id = templateInfo.Id;
                        break;
                    }
                }
            }

            return id;
        }

        public List<int> GetAllFileTemplateIdList(int siteId)
        {
            var list = new List<int>();

            var templateInfoDictionary = GetTemplateInfoDictionaryBySiteId(siteId);
            if (templateInfoDictionary == null) return list;

            foreach (var templateInfo in templateInfoDictionary.Values)
            {
                if (templateInfo.Type == TemplateType.FileTemplate)
                {
                    list.Add(templateInfo.Id);
                }
            }

            return list;
        }

        private Dictionary<int, TemplateInfo> GetTemplateInfoDictionaryBySiteId(int siteId, bool flush = false)
        {
            var dictionary = GetCacheDictionary();

            Dictionary<int, TemplateInfo> templateInfoDictionary = null;

            if (!flush && dictionary.ContainsKey(siteId))
            {
                templateInfoDictionary = dictionary[siteId];
            }

            if (templateInfoDictionary == null)
            {
                templateInfoDictionary = GetTemplateInfoDictionaryBySiteIdToCache(siteId);

                if (templateInfoDictionary != null)
                {
                    UpdateCache(dictionary, templateInfoDictionary, siteId);
                }
            }
            return templateInfoDictionary;
        }

        public List<TemplateInfo> GetTemplateInfoList(int siteId, TemplateType type)
        {
            var list = new List<TemplateInfo>();

            var templateInfoDictionary = GetTemplateInfoDictionaryBySiteId(siteId);
            if (templateInfoDictionary == null) return list;

            foreach (var templateInfo in templateInfoDictionary.Values)
            {
                if (templateInfo.Type == type)
                {
                    list.Add(templateInfo);
                }
            }

            return list.OrderBy(x => x.RelatedFileName).ToList();
        }

        public List<TemplateInfo> GetTemplateInfoList(int siteId, string searchText, string templateTypeString)
        {
            var list = new List<TemplateInfo>();

            var templateInfoDictionary = GetTemplateInfoDictionaryBySiteId(siteId);
            if (templateInfoDictionary == null) return list;

            var isSearch = !string.IsNullOrEmpty(searchText);
            var isTemplateType = !string.IsNullOrEmpty(templateTypeString);
            var tempalteType = TemplateType.Parse(templateTypeString);

            foreach (var templateInfo in templateInfoDictionary.Values)
            {
                if (isSearch && isTemplateType)
                {
                    if (templateInfo.Type == tempalteType && (StringUtils.ContainsIgnoreCase(templateInfo.TemplateName, searchText) || StringUtils.ContainsIgnoreCase(templateInfo.CreatedFileFullName, searchText)))
                    {
                        list.Add(templateInfo);
                    }
                }
                else if (isSearch)
                {
                    if (StringUtils.ContainsIgnoreCase(templateInfo.TemplateName, searchText) || StringUtils.ContainsIgnoreCase(templateInfo.CreatedFileFullName, searchText))
                    {
                        list.Add(templateInfo);
                    }
                }
                else if (isTemplateType)
                {
                    if (templateInfo.Type == tempalteType)
                    {
                        list.Add(templateInfo);
                    }
                }
                else
                {
                    list.Add(templateInfo);
                }


            }

            return list.OrderBy(x => x.RelatedFileName).ToList();
        }

        private void UpdateCache(Dictionary<int, Dictionary<int, TemplateInfo>> dictionary, Dictionary<int, TemplateInfo> templateInfoDictionary, int siteId)
        {
            lock (_syncRoot)
            {
                dictionary[siteId] = templateInfoDictionary;
            }
        }

        public void RemoveCache(int siteId)
        {
            var dictionary = GetCacheDictionary();

            lock (_syncRoot)
            {
                dictionary.Remove(siteId);
            }
        }

        private Dictionary<int, Dictionary<int, TemplateInfo>> GetCacheDictionary()
        {
            var dictionary = _cacheManager.Get<Dictionary<int, Dictionary<int, TemplateInfo>>>(CacheKey);
            if (dictionary == null)
            {
                dictionary = new Dictionary<int, Dictionary<int, TemplateInfo>>();
                _cacheManager.InsertHours(CacheKey, dictionary, 24);
            }
            return dictionary;
        }

        public string GetTemplateFilePath(SiteInfo siteInfo, TemplateInfo templateInfo)
        {
            string filePath;
            if (templateInfo.Type == TemplateType.IndexPageTemplate)
            {
                filePath = PathUtils.Combine(_settingsManager.WebRootPath, siteInfo.SiteDir, templateInfo.RelatedFileName);
            }
            else if (templateInfo.Type == TemplateType.ContentTemplate)
            {
                filePath = PathUtils.Combine(_settingsManager.WebRootPath, siteInfo.SiteDir, DirectoryUtils.Site.Template, DirectoryUtils.Site.Content, templateInfo.RelatedFileName);
            }
            else
            {
                filePath = PathUtils.Combine(_settingsManager.WebRootPath, siteInfo.SiteDir, DirectoryUtils.Site.Template, templateInfo.RelatedFileName);
            }
            return filePath;
        }

        public TemplateInfo GetIndexPageTemplateInfo(int siteId)
        {
            var templateId = GetDefaultTemplateId(siteId, TemplateType.IndexPageTemplate);
            TemplateInfo templateInfo = null;
            if (templateId != 0)
            {
                templateInfo = GetTemplateInfo(siteId, templateId);
            }

            return templateInfo ?? GetDefaultTemplateInfo(siteId, TemplateType.IndexPageTemplate);
        }

        public TemplateInfo GetChannelTemplateInfo(int siteId, int channelId)
        {
            var templateId = 0;
            if (siteId == channelId)
            {
                templateId = GetDefaultTemplateId(siteId, TemplateType.IndexPageTemplate);
            }
            else
            {
                var channelInfo = _channelRepository.GetChannelInfo(siteId, channelId);
                if (channelInfo != null)
                {
                    templateId = channelInfo.ChannelTemplateId;
                }
            }

            TemplateInfo templateInfo = null;
            if (templateId != 0)
            {
                templateInfo = GetTemplateInfo(siteId, templateId);
            }

            return templateInfo ?? GetDefaultTemplateInfo(siteId, TemplateType.ChannelTemplate);
        }

        public TemplateInfo GetContentTemplateInfo(int siteId, int channelId)
        {
            var templateId = 0;
            var channelInfo = _channelRepository.GetChannelInfo(siteId, channelId);
            if (channelInfo != null)
            {
                templateId = channelInfo.ContentTemplateId;
            }

            TemplateInfo templateInfo = null;
            if (templateId != 0)
            {
                templateInfo = GetTemplateInfo(siteId, templateId);
            }

            return templateInfo ?? GetDefaultTemplateInfo(siteId, TemplateType.ContentTemplate);
        }

        public TemplateInfo GetFileTemplateInfo(int siteId, int fileTemplateId)
        {
            var templateId = fileTemplateId;

            TemplateInfo templateInfo = null;
            if (templateId != 0)
            {
                templateInfo = GetTemplateInfo(siteId, templateId);
            }

            return templateInfo ?? GetDefaultTemplateInfo(siteId, TemplateType.FileTemplate);
        }

        public void WriteContentToTemplateFile(SiteInfo siteInfo, TemplateInfo templateInfo, string content, string administratorName)
        {
            if (content == null) content = string.Empty;
            var filePath = GetTemplateFilePath(siteInfo, templateInfo);
            FileUtils.WriteText(filePath, content);

            if (templateInfo.Id > 0)
            {
                var logInfo = new TemplateLogInfo
                {
                    TemplateId = templateInfo.Id,
                    SiteId = templateInfo.SiteId,
                    AddUserName = administratorName,
                    ContentLength = content.Length,
                    TemplateContent = content
                };
                _templateLogRepository.Insert(logInfo);
            }
        }

        public int GetIndexTemplateId(int siteId)
        {
            return GetDefaultTemplateId(siteId, TemplateType.IndexPageTemplate);
        }

        public int GetChannelTemplateId(int siteId, int channelId)
        {
            var templateId = 0;

            var channelInfo = _channelRepository.GetChannelInfo(siteId, channelId);
            if (channelInfo != null)
            {
                templateId = channelInfo.ChannelTemplateId;
            }

            if (templateId == 0)
            {
                templateId = GetDefaultTemplateId(siteId, TemplateType.ChannelTemplate);
            }

            return templateId;
        }

        public int GetContentTemplateId(int siteId, int channelId)
        {
            var templateId = 0;

            var channelInfo = _channelRepository.GetChannelInfo(siteId, channelId);
            if (channelInfo != null)
            {
                templateId = channelInfo.ContentTemplateId;
            }

            if (templateId == 0)
            {
                templateId = GetDefaultTemplateId(siteId, TemplateType.ContentTemplate);
            }

            return templateId;
        }

        public string GetTemplateContent(SiteInfo siteInfo, TemplateInfo templateInfo)
        {
            var filePath = GetTemplateFilePath(siteInfo, templateInfo);
            return GetContentByFilePath(filePath);
        }

        public string GetContentByFilePath(string filePath)
        {
            try
            {
                var content = _cacheManager.Get<string>(filePath);
                if (content != null) return content;

                if (FileUtils.IsFileExists(filePath))
                {
                    content = FileUtils.ReadText(filePath, ECharset.utf_8);
                }

                _cacheManager.Insert(filePath, content, TimeSpan.FromHours(12), filePath);
                return content;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
