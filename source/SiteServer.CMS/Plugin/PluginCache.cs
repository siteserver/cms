using BaiRong.Core;

namespace SiteServer.CMS.Plugin
{
    internal static class PluginCache
    {
        private const string CacheKeyPrefix = "SiteServer.CMS.Core.Plugin.PluginCache.";

        public static void ClearCache()
        {
            CacheUtils.RemoveByStartString(CacheKeyPrefix);
        }

        public static T GetCache<T>(string cacheName) where T : class
        {
            if (CacheUtils.Get(CacheKeyPrefix + cacheName) != null) return CacheUtils.Get(CacheKeyPrefix + cacheName) as T;

            return null;
        }

        public static void SetCache<T>(string cacheName, T obj) where T : class
        {
            CacheUtils.Max(CacheKeyPrefix + cacheName, obj);
        }
    }
}
