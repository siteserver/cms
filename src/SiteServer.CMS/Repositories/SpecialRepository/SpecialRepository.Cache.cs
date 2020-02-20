using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.Repositories
{
    public partial class SpecialRepository
    {
        private readonly string _cacheKey = CacheUtils.GetCacheKey(nameof(SpecialRepository));
        private readonly object _syncRoot = new object();

        public async Task<Special> DeleteSpecialAsync(Site site, int specialId)
        {
            var special = await GetSpecialAsync(site.Id, specialId);

            if (!string.IsNullOrEmpty(special.Url) && special.Url != "/")
            {
                var directoryPath = await GetSpecialDirectoryPathAsync(site, special.Url);
                DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            }

            await DeleteAsync(site.Id, specialId);

            return special;
        }

        public async Task<Special> GetSpecialAsync(int siteId, int specialId)
        {
            Special special = null;
            var specialDictionary = await GetSpecialDictionaryBySiteIdAsync(siteId);

            if (specialDictionary != null && specialDictionary.ContainsKey(specialId))
            {
                special = specialDictionary[specialId];
            }
            return special;
        }

        public async Task<string> GetTitleAsync(int siteId, int specialId)
        {
            var title = string.Empty;

            var special = await GetSpecialAsync(siteId, specialId);
            if (special != null)
            {
                title = special.Title;
            }

            return title;
        }

        public async Task<List<Template>> GetTemplateListAsync(Site site, int specialId)
        {
            var list = new List<Template>();

            var special = await GetSpecialAsync(site.Id, specialId);
            if (special != null)
            {
                var directoryPath = await GetSpecialDirectoryPathAsync(site, special.Url);
                var srcDirectoryPath = GetSpecialSrcDirectoryPath(directoryPath);
                if (!DirectoryUtils.IsDirectoryExists(srcDirectoryPath)) return list;

                var htmlFilePaths = Directory.GetFiles(srcDirectoryPath, "*.html", SearchOption.AllDirectories);
                foreach (var htmlFilePath in htmlFilePaths)
                {
                    var relatedPath = PathUtils.GetPathDifference(srcDirectoryPath, htmlFilePath);

                    var template = new Template
                    {
                        Content = GetContentByFilePath(htmlFilePath),
                        CreatedFileExtName = ".html",
                        CreatedFileFullName = PathUtils.Combine(special.Url, relatedPath),
                        Id = 0,
                        Default = false,
                        RelatedFileName = string.Empty,
                        SiteId = site.Id,
                        TemplateType = TemplateType.FileTemplate,
                        TemplateName = relatedPath
                    };

                    list.Add(template);
                }
            }

            return list;
        }

        public async Task<List<int>> GetAllSpecialIdListAsync(int siteId)
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

        private void RemoveCache(int siteId)
        {
            var dictionary = GetCacheDictionary();

            lock (_syncRoot)
            {
                dictionary.Remove(siteId);
            }
        }

        private Dictionary<int, Dictionary<int, Special>> GetCacheDictionary()
        {
            var dictionary = CacheUtils.Get<Dictionary<int, Dictionary<int, Special>>>(_cacheKey);
            if (dictionary != null) return dictionary;

            dictionary = new Dictionary<int, Dictionary<int, Special>>();
            CacheUtils.InsertHours(_cacheKey, dictionary, 24);
            return dictionary;
        }

        private string GetContentByFilePath(string filePath)
        {
            try
            {
                var content = CacheUtils.Get<string>(filePath);
                if (content != null) return content;

                if (FileUtils.IsFileExists(filePath))
                {
                    content = FileUtils.ReadText(filePath, Encoding.UTF8);
                }

                CacheUtils.Insert(filePath, content, TimeSpan.FromHours(12), filePath);
                return content;
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<string> GetSpecialDirectoryPathAsync(Site site, string url)
        {
            var virtualPath = PageUtils.RemoveFileNameFromUrl(url);
            return await PathUtility.MapPathAsync(site, virtualPath);
        }

        private async Task<string> GetSpecialUrlAsync(Site site, string url)
        {
            var virtualPath = PageUtils.RemoveFileNameFromUrl(url);
            if (!PageUtils.IsVirtualUrl(virtualPath))
            {
                virtualPath = $"@/{StringUtils.TrimSlash(virtualPath)}";
            }
            return await PageUtility.ParseNavigationUrlAsync(site, virtualPath, false);
        }

        public async Task<string> GetSpecialUrlAsync(Site site, int specialId)
        {
            var special = await GetSpecialAsync(site.Id, specialId);
            return await GetSpecialUrlAsync(site, special.Url);
        }

        public string GetSpecialZipFilePath(string title, string directoryPath)
        {
            return PathUtils.Combine(directoryPath, $"{title}.zip");
        }

        public async Task<string> GetSpecialZipFileUrlAsync(Site site, Special special)
        {
            return await PageUtility.ParseNavigationUrlAsync(site, $"@/{special.Url}/{special.Title}.zip", true);
        }

        public string GetSpecialSrcDirectoryPath(string directoryPath)
        {
            return PathUtils.Combine(directoryPath, "_src");
        }
    }
}
