using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.DataCache
{
	public static class SpecialManager
	{
        private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(SpecialManager));
        private static readonly object SyncRoot = new object();

	    public static SpecialInfo DeleteSpecialInfo(int siteId, int specialId)
	    {
	        var specialInfo = GetSpecialInfo(siteId, specialId);
	        DataProvider.SpecialDao.Delete(siteId, specialId);

	        return specialInfo;
	    }

        public static SpecialInfo GetSpecialInfo(int siteId, int specialId)
        {
            SpecialInfo specialInfo = null;
            var specialInfoDictionary = GetSpecialInfoDictionaryBySiteId(siteId);

            if (specialInfoDictionary != null && specialInfoDictionary.ContainsKey(specialId))
            {
                specialInfo = specialInfoDictionary[specialId];
            }
            return specialInfo;
        }

	    public static string GetTitle(int siteId, int specialId)
	    {
	        var title = string.Empty;

	        var specialInfo = GetSpecialInfo(siteId, specialId);
	        if (specialInfo != null)
	        {
	            title = specialInfo.Title;
	        }

	        return title;
	    }

        public static List<TemplateInfo> GetTemplateInfoList(SiteInfo siteInfo, int specialId)
	    {
            var list = new List<TemplateInfo>();

	        var specialInfo = GetSpecialInfo(siteInfo.Id, specialId);
	        if (specialInfo != null)
	        {
	            var directoryPath = GetSpecialDirectoryPath(siteInfo, specialInfo.Url);
	            var srcDirectoryPath = GetSpecialSrcDirectoryPath(directoryPath);

                var htmlFilePaths = Directory.GetFiles(srcDirectoryPath, "*.html", SearchOption.AllDirectories);
                foreach (var htmlFilePath in htmlFilePaths)
                {
                    var relatedPath = PathUtils.GetPathDifference(srcDirectoryPath, htmlFilePath);

                    var templateInfo = new TemplateInfo
                    {
                        Charset = ECharset.utf_8,
                        Content = GetContentByFilePath(htmlFilePath),
                        CreatedFileExtName = ".html",
                        CreatedFileFullName = PathUtils.Combine(specialInfo.Url, relatedPath),
                        Id = 0,
                        IsDefault = false,
                        RelatedFileName = string.Empty,
                        SiteId = siteInfo.Id,
                        TemplateType = TemplateType.FileTemplate,
                        TemplateName = relatedPath
                    };

                    list.Add(templateInfo);
                }
            }

            return list;
	    }

	    public static List<int> GetAllSpecialIdList(int siteId)
	    {
	        var list = new List<int>();

	        var specialInfoDictionary = GetSpecialInfoDictionaryBySiteId(siteId);
	        if (specialInfoDictionary == null) return list;

	        foreach (var specialInfo in specialInfoDictionary.Values)
	        {
	            list.Add(specialInfo.Id);
            }

	        return list;
	    }

        private static Dictionary<int, SpecialInfo> GetSpecialInfoDictionaryBySiteId(int siteId, bool flush = false)
        {
            var dictionary = GetCacheDictionary();

            Dictionary<int, SpecialInfo> specialInfoDictionary = null;

            if (!flush && dictionary.ContainsKey(siteId))
            {
                specialInfoDictionary = dictionary[siteId];
            }

            if (specialInfoDictionary == null)
            {
                specialInfoDictionary = DataProvider.SpecialDao.GetSpecialInfoDictionaryBySiteId(siteId);

                if (specialInfoDictionary != null)
                {
                    UpdateCache(dictionary, specialInfoDictionary, siteId);
                }
            }
            return specialInfoDictionary;
        }

        private static void UpdateCache(Dictionary<int, Dictionary<int, SpecialInfo>> dictionary, Dictionary<int, SpecialInfo> specialInfoDictionary, int siteId)
        {
            lock (SyncRoot)
            {
                dictionary[siteId] = specialInfoDictionary;
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

        private static Dictionary<int, Dictionary<int, SpecialInfo>> GetCacheDictionary()
        {
            var dictionary = DataCacheManager.Get<Dictionary<int, Dictionary<int, SpecialInfo>>>(CacheKey);
            if (dictionary != null) return dictionary;

            dictionary = new Dictionary<int, Dictionary<int, SpecialInfo>>();
            DataCacheManager.InsertHours(CacheKey, dictionary, 24);
            return dictionary;
        }

        private static string GetContentByFilePath(string filePath)
        {
            try
            {
                var content = DataCacheManager.Get<string>(filePath);
                if (content != null) return content;
                
                if (FileUtils.IsFileExists(filePath))
                {
                    content = FileUtils.ReadText(filePath, Encoding.UTF8);
                }

                DataCacheManager.Insert(filePath, content, TimeSpan.FromHours(12), filePath);
                return content;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetSpecialDirectoryPath(SiteInfo siteInfo, string url)
	    {
	        var virtualPath = PageUtils.RemoveFileNameFromUrl(url);
	        return PathUtility.MapPath(siteInfo, virtualPath);
	    }

	    public static string GetSpecialUrl(SiteInfo siteInfo, string url)
	    {
	        var virtualPath = PageUtils.RemoveFileNameFromUrl(url);
	        return PageUtility.ParseNavigationUrl(siteInfo, virtualPath, false);
	    }

	    public static string GetSpecialUrl(SiteInfo siteInfo, int specialId)
	    {
	        var specialInfo = GetSpecialInfo(siteInfo.Id, specialId);
	        return GetSpecialUrl(siteInfo, specialInfo.Url);
	    }

        public static string GetSpecialZipFilePath(string directoryPath)
	    {
	        return PathUtils.Combine(directoryPath, "_src.zip");
	    }

	    public static string GetSpecialSrcDirectoryPath(string directoryPath)
	    {
	        return PathUtils.Combine(directoryPath, "_src");
	    }
    }
}
