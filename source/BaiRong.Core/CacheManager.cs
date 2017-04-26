using System;
using System.Web;
using System.Web.Caching;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core
{
    public class CacheManager
    {
        private CacheManager() { }

        private static HttpRuntime _httpRuntime;

        private static Cache Cache
        {
            get
            {
                EnsureHttpRuntime();
                return HttpRuntime.Cache;
            }
        }

        private static void EnsureHttpRuntime()
        {
            if (null == _httpRuntime)
            {
                _httpRuntime = new HttpRuntime();
            }
        }

        public static object GetCache(string cacheKey)
        {
            return Cache.Get(cacheKey);
        }

        public static void SetCache(string cacheKey, object obj)
        {
            Cache.Insert(cacheKey, obj, null, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.AboveNormal, null);
        }

        public static void SetCache(string cacheKey, object obj, DateTime dateTime)
        {
            Cache.Insert(cacheKey, obj, null, dateTime, TimeSpan.Zero, CacheItemPriority.AboveNormal, null);
        }

        public static void RemoveCache(string cacheKey)
        {
            Cache.Remove(cacheKey);
        }

        public static void UpdateTemporaryCacheFile(string cacheFileName)
        {
            var cacheFilePath = GetCacheFilePath(cacheFileName);
            FileUtils.WriteText(cacheFilePath, ECharset.utf_8, "cache chaged:" + DateUtils.GetDateAndTimeString(DateTime.Now));
        }

        public static string GetCacheFilePath(string cacheFileName)
        {
            return PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.TemporaryFiles, cacheFileName);
        }
    }
}
