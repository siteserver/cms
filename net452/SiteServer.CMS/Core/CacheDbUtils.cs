using SiteServer.CMS.Database.Core;

namespace SiteServer.CMS.Core
{
    public static class CacheDbUtils
	{
		public static void RemoveAndInsert(string cacheKey, string cacheValue)
		{
			if (!string.IsNullOrEmpty(cacheKey))
			{
				DataProvider.DbCache.RemoveAndInsert(cacheKey, cacheValue);
			}
		}

        public static void Clear()
        {
            DataProvider.DbCache.Clear();
        }

		public static bool IsExists(string cacheKey)
		{
            return DataProvider.DbCache.IsExists(cacheKey);
		}

        public static string GetValue(string cacheKey)
        {
            return DataProvider.DbCache.GetValue(cacheKey);
        }

		public static string GetValueAndRemove(string cacheKey)
		{
            return DataProvider.DbCache.GetValueAndRemove(cacheKey);
		}

        public static int GetCount()
        {
            return DataProvider.DbCache.GetCount();
        }

	}
}
