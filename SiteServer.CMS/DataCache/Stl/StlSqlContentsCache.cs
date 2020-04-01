using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;

namespace SiteServer.CMS.DataCache.Stl
{
    public static class StlSqlContentsCache
    {
        private static readonly object LockObject = new object();

        public static string GetSelectSqlStringByQueryString(string connectionString, string queryString, int startNum,
            int totalNum, string orderByString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlSqlContentsCache),
                       nameof(GetSelectSqlStringByQueryString), connectionString, queryString, startNum.ToString(),
                       totalNum.ToString(), orderByString);
            var retVal = StlCacheManager.Get<string>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<string>(cacheKey);
                if (retVal == null)
                {
                    retVal = DataProvider.DatabaseDao.GetSelectSqlStringByQueryString(connectionString,
                    queryString, startNum, totalNum, orderByString);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }
    }
}
