namespace SS.CMS.Core.Common
{
    public static class CacheDbUtils
	{
		public static void RemoveAndInsert(string cacheKey, string cacheValue)
		{
			if (!string.IsNullOrEmpty(cacheKey))
			{
				DataProvider.DbCacheRepository.RemoveAndInsert(cacheKey, cacheValue);
			}
		}

        public static void Clear()
        {
            DataProvider.DbCacheRepository.Clear();
        }

		public static bool IsExists(string cacheKey)
		{
            return DataProvider.DbCacheRepository.IsExists(cacheKey);
		}

        public static string GetValue(string cacheKey)
        {
            return DataProvider.DbCacheRepository.GetValue(cacheKey);
        }

		public static string GetValueAndRemove(string cacheKey)
		{
            return DataProvider.DbCacheRepository.GetValueAndRemove(cacheKey);
		}

        public static int GetCount()
        {
            return DataProvider.DbCacheRepository.GetCount();
        }

	}
}
