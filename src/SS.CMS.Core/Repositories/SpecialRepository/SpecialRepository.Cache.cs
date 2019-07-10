using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Services;
using SS.CMS.Utils;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace SS.CMS.Core.Repositories
{
    public partial class SpecialRepository
    {
        public async Task<Special> GetSpecialInfoAsync(int siteId, int specialId)
        {
            Special specialInfo = null;
            var specialInfoDictionary = await GetSpecialInfoDictionaryBySiteIdAsync(siteId);

            if (specialInfoDictionary != null && specialInfoDictionary.ContainsKey(specialId))
            {
                specialInfo = specialInfoDictionary[specialId];
            }
            return specialInfo;
        }

        public async Task<string> GetTitleAsync(int siteId, int specialId)
        {
            var title = string.Empty;

            var specialInfo = await GetSpecialInfoAsync(siteId, specialId);
            if (specialInfo != null)
            {
                title = specialInfo.Title;
            }

            return title;
        }

        public async Task<List<Template>> GetTemplateInfoListAsync(Site siteInfo, int specialId, IPathManager pathManager)
        {
            var list = new List<Template>();

            var specialInfo = await GetSpecialInfoAsync(siteInfo.Id, specialId);
            if (specialInfo != null)
            {
                var directoryPath = pathManager.GetSpecialDirectoryPath(siteInfo, specialInfo.Url);
                var srcDirectoryPath = pathManager.GetSpecialSrcDirectoryPath(directoryPath);

                var htmlFilePaths = Directory.GetFiles(srcDirectoryPath, "*.html", SearchOption.AllDirectories);
                foreach (var htmlFilePath in htmlFilePaths)
                {
                    var relatedPath = PathUtils.GetPathDifference(srcDirectoryPath, htmlFilePath);

                    var templateInfo = new Template
                    {
                        Id = 0,
                        Content = await GetContentByFilePathAsync(htmlFilePath),
                        CreatedFileExtName = ".html",
                        CreatedFileFullName = PathUtils.Combine(specialInfo.Url, relatedPath),
                        IsDefault = false,
                        RelatedFileName = string.Empty,
                        SiteId = siteInfo.Id,
                        Type = TemplateType.FileTemplate,
                        TemplateName = relatedPath
                    };

                    list.Add(templateInfo);
                }
            }

            return list;
        }

        public async Task<List<int>> GetAllSpecialIdListAsync(int siteId)
        {
            var list = new List<int>();

            var specialInfoDictionary = await GetSpecialInfoDictionaryBySiteIdAsync(siteId);
            if (specialInfoDictionary == null) return list;

            foreach (var specialInfo in specialInfoDictionary.Values)
            {
                list.Add(specialInfo.Id);
            }

            return list;
        }

        // private Dictionary<int, SpecialInfo> GetSpecialInfoDictionaryBySiteId(int siteId)
        // {
        //     var dictionary = GetCacheDictionary();

        //     Dictionary<int, SpecialInfo> specialInfoDictionary = null;

        //     if (!flush && dictionary.ContainsKey(siteId))
        //     {
        //         specialInfoDictionary = dictionary[siteId];
        //     }

        //     if (specialInfoDictionary == null)
        //     {
        //         specialInfoDictionary = GetSpecialInfoDictionaryBySiteIdToCache(siteId);

        //         if (specialInfoDictionary != null)
        //         {
        //             UpdateCache(dictionary, specialInfoDictionary, siteId);
        //         }
        //     }
        //     return specialInfoDictionary;
        // }

        // private void UpdateCache(Dictionary<int, Dictionary<int, SpecialInfo>> dictionary, Dictionary<int, SpecialInfo> specialInfoDictionary, int siteId)
        // {
        //     lock (_syncRoot)
        //     {
        //         dictionary[siteId] = specialInfoDictionary;
        //     }
        // }

        // private Dictionary<int, Dictionary<int, SpecialInfo>> GetCacheDictionary()
        // {
        //     var dictionary = _cacheManager.Get<Dictionary<int, Dictionary<int, SpecialInfo>>>(CacheKey);
        //     if (dictionary != null) return dictionary;

        //     dictionary = new Dictionary<int, Dictionary<int, SpecialInfo>>();
        //     _cacheManager.InsertHours(CacheKey, dictionary, 24);
        //     return dictionary;
        // }

        private async Task<string> GetContentByFilePathAsync(string filePath)
        {
            return await _cache.GetOrCreateAsync(_cache.GetKey(nameof(SpecialRepository), filePath), async options =>
            {
                if (FileUtils.IsFileExists(filePath))
                {
                    return await FileUtils.ReadTextAsync(filePath, Encoding.UTF8);
                }
                return string.Empty;
            });
        }

        private async Task RemoveCacheAsync(int siteId)
        {
            await _cache.RemoveAsync(_cache.GetKey(nameof(SpecialRepository), siteId.ToString()));
        }
    }
}
