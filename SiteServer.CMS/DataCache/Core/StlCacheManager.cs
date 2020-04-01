using SiteServer.Utils;

namespace SiteServer.CMS.DataCache.Core
{
    public static class StlCacheManager
    {
        private const string CachePrefix = "StlCacheManager";

        public static string GetCacheKey(string nameofClass, string nameofMethod, params string[] values)
        {
            var key = $"{CachePrefix}.{nameofClass}.{nameofMethod}";
            if (values == null || values.Length <= 0) return key;
            foreach (var t in values)
            {
                key += "." + t;
            }
            return key;
        }

        public static T Get<T>(string cacheKey) where T : class
        {
            return CacheUtils.Get<T>(cacheKey);
        }

        public static int GetInt(string cacheKey)
        {
            return CacheUtils.GetInt(cacheKey, -1);
        }

        public static void Set(string cacheKey, object value)
        {
            CacheUtils.InsertMinutes(cacheKey, value, 2);
        }

        public static void Clear(string nameofClass)
        {
            CacheUtils.RemoveByStartString(GetCacheKey(nameofClass, string.Empty));
        }

        public static void ClearAll()
        {
            CacheUtils.RemoveByStartString(CachePrefix);
        }
    }
}
