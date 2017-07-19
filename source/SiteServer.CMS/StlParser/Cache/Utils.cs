using System;
using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Utils
    {
        private Utils()
        {
        }

        public static string GetCacheKey(string nameofClass, string nameofMethod, string guid, params string[] values)
        {
            var key = $"SiteServer.CMS.StlParser.Utility.DatabaseCache.{nameofClass}.{nameofMethod}.{guid}";
            if (values == null || values.Length <= 0) return key;
            for (var i = 1; i < values.Length; i++)
            {
                key += "." + values[i];
            }
            return key;
        }

        public static T GetCache<T>(string cacheKey) where T : class
        {
            return CacheUtils.Get<T>(cacheKey);
        }

        public static int GetIntCache(string cacheKey)
        {
            return CacheUtils.GetInt(cacheKey, -1);
        }

        public static DateTime GetDateTimeCache(string cacheKey)
        {
            return CacheUtils.GetDateTime(cacheKey, DateTime.MinValue);
        }

        public static void SetCache(string cacheKey, object value)
        {
            CacheUtils.Insert(cacheKey, value, CacheUtils.MinuteFactor * 30);
        }
    }
}
