using System.Collections.Generic;
using BaiRong.Core.Model;

namespace BaiRong.Core
{
	public class ContentModelUtils
	{
        private ContentModelUtils()
		{
		}

        private static readonly object LockObject = new object();
        private const string CacheKeyPrefix = "BaiRong.Core.ContentModelUtils.";

        public static List<ContentModelInfo> GetContentModelInfoList(int siteId)
        {
            var cacheKey = GetCacheKey(siteId);
            lock (LockObject)
            {
                if (CacheUtils.Get(cacheKey) == null)
                {
                    var list = BaiRongDataProvider.ContentModelDao.GetContentModelInfoList(siteId);

                    CacheUtils.Insert(cacheKey, list, CacheUtils.HourFactor);
                    return list;
                }
                return CacheUtils.Get(cacheKey) as List<ContentModelInfo>;
            }
        }

        public static void RemoveCache(int siteId)
        {
            var cacheKey = GetCacheKey(siteId);
            CacheUtils.Remove(cacheKey);
        }

        private static string GetCacheKey(int siteId)
        {
            return CacheKeyPrefix + siteId;
        }
	}
}
