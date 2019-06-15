using SS.CMS.Core.Cache.Core;
using SS.CMS.Core.Common;

namespace SS.CMS.Core.Cache.Stl
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
            var retval = StlCacheManager.Get<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<string>(cacheKey);
                if (retval == null)
                {
                    // retval = DatabaseUtils.GetSelectSqlStringByQueryString(connectionString,
                    // queryString, startNum, totalNum, orderByString);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
