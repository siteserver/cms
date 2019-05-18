namespace SiteServer.CMS.Core
{
    public static class CacheDbUtils
	{
		public static void RemoveAndInsert(string cacheKey, string cacheValue)
		{
			if (!string.IsNullOrEmpty(cacheKey))
			{
				DataProvider.DbCacheDao.RemoveAndInsert(cacheKey, cacheValue);
			}
		}

        public static void Clear()
        {
            DataProvider.DbCacheDao.Clear();
        }

		public static bool IsExists(string cacheKey)
		{
            return DataProvider.DbCacheDao.IsExists(cacheKey);
		}

        public static string GetValue(string cacheKey)
        {
            return DataProvider.DbCacheDao.GetValue(cacheKey);
        }

		public static string GetValueAndRemove(string cacheKey)
		{
            return DataProvider.DbCacheDao.GetValueAndRemove(cacheKey);
		}

        public static int GetCount()
        {
            return DataProvider.DbCacheDao.GetCount();
        }

	}
}
