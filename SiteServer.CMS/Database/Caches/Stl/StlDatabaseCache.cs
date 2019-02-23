using System.Data;
using SiteServer.CMS.Apis;
using SiteServer.CMS.Database.Caches.Core;
using SiteServer.CMS.Database.Core;

namespace SiteServer.CMS.Database.Caches.Stl
{
    public static class StlDatabaseCache
    {
        private static readonly object LockObject = new object();

        public static int GetPageTotalCount(string sqlString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlDatabaseCache), nameof(GetPageTotalCount),
                    sqlString);
            var retval = StlCacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = DatabaseApi.Instance.GetPageTotalCount(sqlString);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetStlPageSqlString(string sqlString, string orderByString, int totalNum, int pageNum, int currentPageIndex)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlDatabaseCache), nameof(GetStlPageSqlString),
                       sqlString, orderByString, totalNum.ToString(), pageNum.ToString(), currentPageIndex.ToString());
            var retval = StlCacheManager.Get<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<string>(cacheKey);
                if (retval == null)
                {
                    retval = DatabaseApi.Instance.GetStlPageSqlString(sqlString, orderByString, totalNum, pageNum,
                    currentPageIndex);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetString(string connectionString, string queryString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlDatabaseCache), nameof(GetString),
                       connectionString, queryString);
            var retval = StlCacheManager.Get<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<string>(cacheKey);
                if (retval == null)
                {
                    retval = DatabaseApi.Instance.GetString(connectionString, queryString);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static DataSet GetDataSet(string connectionString, string queryString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlDatabaseCache), nameof(GetDataSet),
                connectionString, queryString);
            var retval = StlCacheManager.Get<DataSet>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<DataSet>(cacheKey);
                if (retval == null)
                {
                    retval = DatabaseApi.Instance.GetDataSet(connectionString, queryString);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static DataTable GetDataTable(string connectionString, string queryString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlDatabaseCache), nameof(GetDataTable),
                connectionString, queryString);
            var retval = StlCacheManager.Get<DataTable>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<DataTable>(cacheKey);
                if (retval == null)
                {
                    retval = DatabaseApi.Instance.GetDataTable(connectionString, queryString);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
