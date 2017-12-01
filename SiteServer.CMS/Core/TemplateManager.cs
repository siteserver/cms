using System;
using BaiRong.Core;
using SiteServer.CMS.Model;
using System.Collections.Generic;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Core
{
	public class TemplateManager
	{
        private TemplateManager()
		{
        }

        private const string CacheKey = "SiteServer.CMS.Core.TemplateManager";
        private static readonly object SyncRoot = new object();

        public static TemplateInfo GetTemplateInfo(int publishmentSystemId, int templateId)
        {
            TemplateInfo templateInfo = null;
            var templateInfoDictionary = GetTemplateInfoDictionaryByPublishmentSystemId(publishmentSystemId);

            if (templateInfoDictionary != null && templateInfoDictionary.ContainsKey(templateId))
            {
                templateInfo = templateInfoDictionary[templateId];
            }
            return templateInfo;
        }

        public static string GetCreatedFileFullName(int publishmentSystemId, int templateId)
        {
            var createdFileFullName = string.Empty;

            var templateInfo = GetTemplateInfo(publishmentSystemId, templateId);
            if (templateInfo != null)
            {
                createdFileFullName = templateInfo.CreatedFileFullName;
            }

            return createdFileFullName;
        }

        public static string GetTemplateName(int publishmentSystemId, int templateId)
        {
            var templateName = string.Empty;

            var templateInfo = GetTemplateInfo(publishmentSystemId, templateId);
            if (templateInfo != null)
            {
                templateName = templateInfo.TemplateName;
            }

            return templateName;
        }

        public static TemplateInfo GetTemplateInfoByTemplateName(int publishmentSystemId, ETemplateType templateType, string templateName)
        {
            TemplateInfo info = null;

            var templateInfoDictionary = GetTemplateInfoDictionaryByPublishmentSystemId(publishmentSystemId);
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

        public static TemplateInfo GetDefaultTemplateInfo(int publishmentSystemId, ETemplateType templateType)
        {
            TemplateInfo info = null;

            var templateInfoDictionary = GetTemplateInfoDictionaryByPublishmentSystemId(publishmentSystemId);
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

            return info;
        }

        public static int GetDefaultTemplateId(int publishmentSystemId, ETemplateType templateType)
        {
            var id = 0;

            var templateInfoDictionary = GetTemplateInfoDictionaryByPublishmentSystemId(publishmentSystemId);
            if (templateInfoDictionary != null)
            {
                foreach (var templateInfo in templateInfoDictionary.Values)
                {
                    if (templateInfo.TemplateType == templateType && templateInfo.IsDefault)
                    {
                        id = templateInfo.TemplateId;
                        break;
                    }
                }
            }

            return id;
        }

        public static int GetTemplateIdByTemplateName(int publishmentSystemId, ETemplateType templateType, string templateName)
        {
            var id = 0;

            var templateInfoDictionary = GetTemplateInfoDictionaryByPublishmentSystemId(publishmentSystemId);
            if (templateInfoDictionary != null)
            {
                foreach (var templateInfo in templateInfoDictionary.Values)
                {
                    if (templateInfo.TemplateType == templateType && templateInfo.TemplateName == templateName)
                    {
                        id = templateInfo.TemplateId;
                        break;
                    }
                }
            }

            return id;
        }

        public static List<int> GetAllFileTemplateIdList(int publishmentSystemId)
        {
            var list = new List<int>();

            var templateInfoDictionary = GetTemplateInfoDictionaryByPublishmentSystemId(publishmentSystemId);
            if (templateInfoDictionary == null) return list;

            foreach (var templateInfo in templateInfoDictionary.Values)
            {
                if (templateInfo.TemplateType == ETemplateType.FileTemplate)
                {
                    list.Add(templateInfo.TemplateId);
                }
            }

            return list;
        }

	    private static Dictionary<int, TemplateInfo> GetTemplateInfoDictionaryByPublishmentSystemId(int publishmentSystemId, bool flush = false)
        {
            var dictionary = GetCacheDictionary();

            Dictionary<int, TemplateInfo> templateInfoDictionary = null;

            if (!flush && dictionary.ContainsKey(publishmentSystemId))
            {
                templateInfoDictionary = dictionary[publishmentSystemId];
            }

            if (templateInfoDictionary == null)
            {
                templateInfoDictionary = DataProvider.TemplateDao.GetTemplateInfoDictionaryByPublishmentSystemId(publishmentSystemId);

                if (templateInfoDictionary != null)
                {
                    UpdateCache(dictionary, templateInfoDictionary, publishmentSystemId);
                }
            }
            return templateInfoDictionary;
        }

        private static void UpdateCache(Dictionary<int, Dictionary<int, TemplateInfo>> dictionary, Dictionary<int, TemplateInfo> templateInfoDictionary, int publishmentSystemId)
        {
            lock (SyncRoot)
            {
                dictionary[publishmentSystemId] = templateInfoDictionary;
            }
        }

        public static void RemoveCache(int publishmentSystemId)
        {
            var dictionary = GetCacheDictionary();

            lock (SyncRoot)
            {
                dictionary.Remove(publishmentSystemId);
            }
        }

        private static Dictionary<int, Dictionary<int, TemplateInfo>> GetCacheDictionary()
        {
            var dictionary = CacheUtils.Get(CacheKey) as Dictionary<int, Dictionary<int, TemplateInfo>>;
            if (dictionary == null)
            {
                dictionary = new Dictionary<int, Dictionary<int, TemplateInfo>>();
                CacheUtils.InsertHours(CacheKey, dictionary, 24);
            }
            return dictionary;
        }

        public static string GetTemplateFilePath(PublishmentSystemInfo publishmentSystemInfo, TemplateInfo templateInfo)
        {
            string filePath;
            if (templateInfo.TemplateType == ETemplateType.IndexPageTemplate)
            {
                filePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, publishmentSystemInfo.PublishmentSystemDir, templateInfo.RelatedFileName);
            }
            else if (templateInfo.TemplateType == ETemplateType.ContentTemplate)
            {
                filePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, publishmentSystemInfo.PublishmentSystemDir, DirectoryUtils.PublishmentSytem.Template, DirectoryUtils.PublishmentSytem.Content, templateInfo.RelatedFileName);
            }
            else
            {
                filePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, publishmentSystemInfo.PublishmentSystemDir, DirectoryUtils.PublishmentSytem.Template, templateInfo.RelatedFileName);
            }
            return filePath;
        }

        public static TemplateInfo GetTemplateInfo(int publishmentSystemId, int nodeId, ETemplateType templateType)
        {
            var templateId = 0;
            if (templateType == ETemplateType.IndexPageTemplate)
            {
                templateId = GetDefaultTemplateId(publishmentSystemId, ETemplateType.IndexPageTemplate);
            }
            else if (templateType == ETemplateType.ChannelTemplate)
            {
                var nodeType = NodeManager.GetNodeType(publishmentSystemId, nodeId);
                if (nodeType == ENodeType.BackgroundPublishNode)
                {
                    templateId = GetDefaultTemplateId(publishmentSystemId, ETemplateType.IndexPageTemplate);
                }
                else
                {
                    var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
                    if (nodeInfo != null)
                    {
                        templateId = nodeInfo.ChannelTemplateId;
                    }
                }
            }
            else if (templateType == ETemplateType.ContentTemplate)
            {
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
                if (nodeInfo != null)
                {
                    templateId = nodeInfo.ContentTemplateId;
                }
            }

            TemplateInfo templateInfo = null;
            if (templateId != 0)
            {
                templateInfo = GetTemplateInfo(publishmentSystemId, templateId);
            }

            return templateInfo ?? GetDefaultTemplateInfo(publishmentSystemId, templateType);
        }

        public static void WriteContentToTemplateFile(PublishmentSystemInfo publishmentSystemInfo, TemplateInfo templateInfo, string content, string administratorName)
        {
            if (content == null) content = string.Empty;
            var filePath = GetTemplateFilePath(publishmentSystemInfo, templateInfo);
            FileUtils.WriteText(filePath, templateInfo.Charset, content);

            if (templateInfo.TemplateId > 0)
            {
                var logInfo = new TemplateLogInfo(0, templateInfo.TemplateId, templateInfo.PublishmentSystemId, DateTime.Now, administratorName, content.Length, content);
                DataProvider.TemplateLogDao.Insert(logInfo);
            }
        }

        public static void UpdateChannelTemplateId(int publishmentSystemId, int nodeId, int channelTemplateId)
        {
            DataProvider.NodeDao.UpdateChannelTemplateId(nodeId, channelTemplateId);
        }

        public static void UpdateContentTemplateId(int publishmentSystemId, int nodeId, int contentTemplateId)
        {
            DataProvider.NodeDao.UpdateContentTemplateId(nodeId, contentTemplateId);
        }

        public static int GetIndexTempalteId(int publishmentSystemId)
        {
            return GetDefaultTemplateId(publishmentSystemId, ETemplateType.IndexPageTemplate);
        }

        public static int GetChannelTempalteId(int publishmentSystemId, int nodeId)
        {
            var templateId = 0;

            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
            if (nodeInfo != null)
            {
                templateId = nodeInfo.ChannelTemplateId;
            }

            if (templateId == 0)
            {
                templateId = GetDefaultTemplateId(publishmentSystemId, ETemplateType.ChannelTemplate);
            }

            return templateId;
        }

        public static int GetContentTempalteId(int publishmentSystemId, int nodeId)
        {
            var templateId = 0;

            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
            if (nodeInfo != null)
            {
                templateId = nodeInfo.ContentTemplateId;
            }

            if (templateId == 0)
            {
                templateId = GetDefaultTemplateId(publishmentSystemId, ETemplateType.ContentTemplate);
            }

            return templateId;
        }

        public static string GetTemplateContent(PublishmentSystemInfo publishmentSystemInfo, TemplateInfo templateInfo)
        {
            var filePath = GetTemplateFilePath(publishmentSystemInfo, templateInfo);
            return GetContentByFilePath(filePath, templateInfo.Charset);
        }

        public static string GetIncludeContent(PublishmentSystemInfo publishmentSystemInfo, string file, ECharset charset)
        {
            var filePath = PathUtility.MapPath(publishmentSystemInfo, PathUtility.AddVirtualToPath(file));
            return GetContentByFilePath(filePath, charset);
        }

        public static string GetContentByFilePath(string filePath, ECharset charset = ECharset.utf_8)
        {
            try
            {
                if (CacheUtils.Get(filePath) != null) return CacheUtils.Get(filePath) as string;

                var content = string.Empty;
                if (FileUtils.IsFileExists(filePath))
                {
                    content = FileUtils.ReadText(filePath, charset);
                }

                CacheUtils.Insert(filePath, content, TimeSpan.FromHours(12), filePath);
                return content;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
