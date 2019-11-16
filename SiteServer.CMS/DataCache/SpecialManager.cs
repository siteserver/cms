using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.DataCache
{
	public static class SpecialManager
	{
        private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(SpecialManager));
        private static readonly object SyncRoot = new object();

	    public static async Task<Special> DeleteSpecialAsync(Site site, int specialId)
	    {
	        var special = await GetSpecialAsync(site.Id, specialId);

            if (!string.IsNullOrEmpty(special.Url) && special.Url != "/")
            {
                var directoryPath = GetSpecialDirectoryPath(site, special.Url);
                DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            }

            await DataProvider.SpecialDao.DeleteAsync(site.Id, specialId);

	        return special;
	    }

        public static async Task<Special> GetSpecialAsync(int siteId, int specialId)
        {
            Special special = null;
            var specialDictionary = await GetSpecialDictionaryBySiteIdAsync(siteId);

            if (specialDictionary != null && specialDictionary.ContainsKey(specialId))
            {
                special = specialDictionary[specialId];
            }
            return special;
        }

	    public static async Task<string> GetTitleAsync(int siteId, int specialId)
	    {
	        var title = string.Empty;

	        var special = await GetSpecialAsync(siteId, specialId);
	        if (special != null)
	        {
	            title = special.Title;
	        }

	        return title;
	    }

        public static async Task<List<Template>> GetTemplateListAsync(Site site, int specialId)
	    {
            var list = new List<Template>();

	        var special = await GetSpecialAsync(site.Id, specialId);
	        if (special != null)
	        {
	            var directoryPath = GetSpecialDirectoryPath(site, special.Url);
	            var srcDirectoryPath = GetSpecialSrcDirectoryPath(directoryPath);

                var htmlFilePaths = Directory.GetFiles(srcDirectoryPath, "*.html", SearchOption.AllDirectories);
                foreach (var htmlFilePath in htmlFilePaths)
                {
                    var relatedPath = PathUtils.GetPathDifference(srcDirectoryPath, htmlFilePath);

                    var template = new Template
                    {
                        CharsetType = ECharset.utf_8,
                        Content = GetContentByFilePath(htmlFilePath),
                        CreatedFileExtName = ".html",
                        CreatedFileFullName = PathUtils.Combine(special.Url, relatedPath),
                        Id = 0,
                        Default = false,
                        RelatedFileName = string.Empty,
                        SiteId = site.Id,
                        Type = TemplateType.FileTemplate,
                        TemplateName = relatedPath
                    };

                    list.Add(template);
                }
            }

            return list;
	    }

	    public static async Task<List<int>> GetAllSpecialIdListAsync(int siteId)
	    {
	        var list = new List<int>();

	        var specialDictionary = await GetSpecialDictionaryBySiteIdAsync(siteId);
	        if (specialDictionary == null) return list;

	        foreach (var special in specialDictionary.Values)
	        {
	            list.Add(special.Id);
            }

	        return list;
	    }

        private static async Task<Dictionary<int, Special>> GetSpecialDictionaryBySiteIdAsync(int siteId, bool flush = false)
        {
            var dictionary = GetCacheDictionary();

            Dictionary<int, Special> specialDictionary = null;

            if (!flush && dictionary.ContainsKey(siteId))
            {
                specialDictionary = dictionary[siteId];
            }

            if (specialDictionary == null)
            {
                specialDictionary = await DataProvider.SpecialDao.GetSpecialDictionaryBySiteIdAsync(siteId);

                if (specialDictionary != null)
                {
                    UpdateCache(dictionary, specialDictionary, siteId);
                }
            }
            return specialDictionary;
        }

        private static void UpdateCache(Dictionary<int, Dictionary<int, Special>> dictionary, Dictionary<int, Special> specialDictionary, int siteId)
        {
            lock (SyncRoot)
            {
                dictionary[siteId] = specialDictionary;
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

        private static Dictionary<int, Dictionary<int, Special>> GetCacheDictionary()
        {
            var dictionary = DataCacheManager.Get<Dictionary<int, Dictionary<int, Special>>>(CacheKey);
            if (dictionary != null) return dictionary;

            dictionary = new Dictionary<int, Dictionary<int, Special>>();
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

        public static string GetSpecialDirectoryPath(Site site, string url)
	    {
	        var virtualPath = PageUtils.RemoveFileNameFromUrl(url);
	        return PathUtility.MapPath(site, virtualPath);
	    }

	    public static string GetSpecialUrl(Site site, string url)
	    {
	        var virtualPath = PageUtils.RemoveFileNameFromUrl(url);
            if (!PageUtils.IsVirtualUrl(virtualPath))
            {
                virtualPath = $"@/{StringUtils.TrimSlash(virtualPath)}";
            }
	        return PageUtility.ParseNavigationUrl(site, virtualPath, false);
	    }

	    public static async Task<string> GetSpecialUrlAsync(Site site, int specialId)
	    {
	        var special = await GetSpecialAsync(site.Id, specialId);
	        return GetSpecialUrl(site, special.Url);
	    }

        public static string GetSpecialZipFilePath(string title, string directoryPath)
	    {
	        return PathUtils.Combine(directoryPath, $"{title}.zip");
	    }

        public static string GetSpecialZipFileUrl(Site site, Special special)
        {
            return PageUtility.ParseNavigationUrl(site, $"@/{special.Url}/{special.Title}.zip", true);
        }

        public static string GetSpecialSrcDirectoryPath(string directoryPath)
	    {
	        return PathUtils.Combine(directoryPath, "_src");
	    }
    }
}
