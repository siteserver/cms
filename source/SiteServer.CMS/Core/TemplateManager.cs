using System;
using BaiRong.Core;
using SiteServer.CMS.Model;
using System.Collections.Generic;
using BaiRong.Core.Auth;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Core
{
	public class TemplateManager
	{
        private TemplateManager()
		{
        }

        private const string cacheKey = "SiteServer.CMS.Core.TemplateManager";
        private static object syncRoot = new object();

        public static TemplateInfo GetTemplateInfo(int publishmentSystemID, int templateID)
        {
            TemplateInfo templateInfo = null;
            var templateInfoDictionary = GetTemplateInfoDictionaryByPublishmentSystemID(publishmentSystemID);

            if (templateInfoDictionary != null && templateInfoDictionary.ContainsKey(templateID))
            {
                templateInfo = templateInfoDictionary[templateID];
            }
            return templateInfo;
        }

        public static string GetCreatedFileFullName(int publishmentSystemID, int templateID)
        {
            var createdFileFullName = string.Empty;

            var templateInfo = GetTemplateInfo(publishmentSystemID, templateID);
            if (templateInfo != null)
            {
                createdFileFullName = templateInfo.CreatedFileFullName;
            }

            return createdFileFullName;
        }

        public static string GetTemplateName(int publishmentSystemID, int templateID)
        {
            var templateName = string.Empty;

            var templateInfo = GetTemplateInfo(publishmentSystemID, templateID);
            if (templateInfo != null)
            {
                templateName = templateInfo.TemplateName;
            }

            return templateName;
        }

        public static TemplateInfo GetTemplateInfoByTemplateName(int publishmentSystemID, ETemplateType templateType, string templateName)
        {
            TemplateInfo info = null;

            var templateInfoDictionary = GetTemplateInfoDictionaryByPublishmentSystemID(publishmentSystemID);
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

        public static TemplateInfo GetDefaultTemplateInfo(int publishmentSystemID, ETemplateType templateType)
        {
            TemplateInfo info = null;

            var templateInfoDictionary = GetTemplateInfoDictionaryByPublishmentSystemID(publishmentSystemID);
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

        public static int GetDefaultTemplateID(int publishmentSystemID, ETemplateType templateType)
        {
            var id = 0;

            var templateInfoDictionary = GetTemplateInfoDictionaryByPublishmentSystemID(publishmentSystemID);
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

        public static int GetTemplateIDByTemplateName(int publishmentSystemID, ETemplateType templateType, string templateName)
        {
            var id = 0;

            var templateInfoDictionary = GetTemplateInfoDictionaryByPublishmentSystemID(publishmentSystemID);
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

        public static List<int> GetAllTemplateIDList(int publishmentSystemID)
        {
            var list = new List<int>();

            var templateInfoDictionary = GetTemplateInfoDictionaryByPublishmentSystemID(publishmentSystemID);
            if (templateInfoDictionary != null)
            {
                foreach (var templateInfo in templateInfoDictionary.Values)
                {
                    list.Add(templateInfo.TemplateId);
                }
            }

            return list;
        }

        private static Dictionary<int, TemplateInfo> GetTemplateInfoDictionaryByPublishmentSystemID(int publishmentSystemID)
        {
            return GetTemplateInfoDictionaryByPublishmentSystemID(publishmentSystemID, false);
        }

        private static Dictionary<int, TemplateInfo> GetTemplateInfoDictionaryByPublishmentSystemID(int publishmentSystemID, bool flush)
        {
            var dictionary = GetCacheDictionary();

            Dictionary<int, TemplateInfo> templateInfoDictionary = null;

            if (!flush && dictionary.ContainsKey(publishmentSystemID))
            {
                templateInfoDictionary = dictionary[publishmentSystemID];
            }

            if (templateInfoDictionary == null)
            {
                templateInfoDictionary = DataProvider.TemplateDao.GetTemplateInfoDictionaryByPublishmentSystemId(publishmentSystemID);

                if (templateInfoDictionary != null)
                {
                    UpdateCache(dictionary, templateInfoDictionary, publishmentSystemID);
                }
            }
            return templateInfoDictionary;
        }

        private static void UpdateCache(Dictionary<int, Dictionary<int, TemplateInfo>> dictionary, Dictionary<int, TemplateInfo> templateInfoDictionary, int publishmentSystemID)
        {
            lock (syncRoot)
            {
                dictionary[publishmentSystemID] = templateInfoDictionary;
            }
        }

        public static void RemoveCache(int publishmentSystemID)
        {
            var dictionary = GetCacheDictionary();

            lock (syncRoot)
            {
                dictionary.Remove(publishmentSystemID);
            }
        }

        private static Dictionary<int, Dictionary<int, TemplateInfo>> GetCacheDictionary()
        {
            var dictionary = CacheUtils.Get(cacheKey) as Dictionary<int, Dictionary<int, TemplateInfo>>;
            if (dictionary == null)
            {
                dictionary = new Dictionary<int, Dictionary<int, TemplateInfo>>();
                CacheUtils.Insert(cacheKey, dictionary, null, CacheUtils.DayFactor);
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

        public static TemplateInfo GetTemplateInfo(int publishmentSystemID, int nodeID, ETemplateType templateType)
        {
            var templateID = 0;
            if (templateType == ETemplateType.IndexPageTemplate)
            {
                templateID = GetDefaultTemplateID(publishmentSystemID, ETemplateType.IndexPageTemplate);
            }
            else if (templateType == ETemplateType.ChannelTemplate)
            {
                var nodeType = NodeManager.GetNodeType(publishmentSystemID, nodeID);
                if (nodeType == ENodeType.BackgroundPublishNode)
                {
                    templateID = GetDefaultTemplateID(publishmentSystemID, ETemplateType.IndexPageTemplate);
                }
                else
                {
                    var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemID, nodeID);
                    if (nodeInfo != null)
                    {
                        templateID = nodeInfo.ChannelTemplateId;
                    }
                }
            }
            else if (templateType == ETemplateType.ContentTemplate)
            {
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemID, nodeID);
                if (nodeInfo != null)
                {
                    templateID = nodeInfo.ContentTemplateId;
                }
            }

            TemplateInfo templateInfo = null;
            if (templateID != 0)
            {
                templateInfo = GetTemplateInfo(publishmentSystemID, templateID);
            }
            if (templateInfo == null)
            {
                templateInfo = GetDefaultTemplateInfo(publishmentSystemID, templateType);
            }
            return templateInfo;
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

        public static void UpdateChannelTemplateID(int publishmentSystemID, int nodeID, int channelTemplateID)
        {
            DataProvider.NodeDao.UpdateChannelTemplateId(nodeID, channelTemplateID);
        }

        public static void UpdateContentTemplateID(int publishmentSystemID, int nodeID, int contentTemplateID)
        {
            DataProvider.NodeDao.UpdateContentTemplateId(nodeID, contentTemplateID);
        }

        public static int GetIndexTempalteID(int publishmentSystemID)
        {
            return GetDefaultTemplateID(publishmentSystemID, ETemplateType.IndexPageTemplate);
        }

        public static int GetChannelTempalteID(int publishmentSystemID, int nodeID)
        {
            var templateID = 0;

            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemID, nodeID);
            if (nodeInfo != null)
            {
                templateID = nodeInfo.ChannelTemplateId;
            }

            if (templateID == 0)
            {
                templateID = GetDefaultTemplateID(publishmentSystemID, ETemplateType.ChannelTemplate);
            }

            return templateID;
        }

        public static int GetContentTempalteID(int publishmentSystemID, int nodeID)
        {
            var templateID = 0;

            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemID, nodeID);
            if (nodeInfo != null)
            {
                templateID = nodeInfo.ContentTemplateId;
            }

            if (templateID == 0)
            {
                templateID = GetDefaultTemplateID(publishmentSystemID, ETemplateType.ContentTemplate);
            }

            return templateID;
        }
    }
}
