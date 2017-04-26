namespace BaiRong.Core
{
	public class DbCacheManager
	{
		private DbCacheManager()
		{
		}

		public static void RemoveAndInsert(string cacheKey, string cacheValue)
		{
			if (!string.IsNullOrEmpty(cacheKey))
			{
				BaiRongDataProvider.DbCacheDao.RemoveAndInsert(cacheKey, cacheValue);
			}
		}

        public static void Clear()
        {
            BaiRongDataProvider.DbCacheDao.Clear();
        }

		public static bool IsExists(string cacheKey)
		{
            return BaiRongDataProvider.DbCacheDao.IsExists(cacheKey);
		}

        public static string GetValue(string cacheKey)
        {
            return BaiRongDataProvider.DbCacheDao.GetValue(cacheKey);
        }

		public static string GetValueAndRemove(string cacheKey)
		{
            return BaiRongDataProvider.DbCacheDao.GetValueAndRemove(cacheKey);
		}

        public static int GetCount()
        {
            return BaiRongDataProvider.DbCacheDao.GetCount();
        }

	}
}
