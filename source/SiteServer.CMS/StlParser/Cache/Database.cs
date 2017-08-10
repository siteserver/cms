using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Database
    {
        public static int GetPageTotalCount(string sqlString, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Database), nameof(GetPageTotalCount), sqlString);
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = BaiRongDataProvider.DatabaseDao.GetPageTotalCount(sqlString);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }

        public static string GetStlPageSqlString(string sqlString, string orderByString, int totalNum, int pageNum, int currentPageIndex, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Database), nameof(GetStlPageSqlString), sqlString, orderByString, totalNum.ToString(), pageNum.ToString(), currentPageIndex.ToString());
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            retval = BaiRongDataProvider.DatabaseDao.GetStlPageSqlString(sqlString, orderByString, totalNum, pageNum, currentPageIndex);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }

        public static string GetString(string connectionString, string queryString, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Database), nameof(GetString), connectionString, queryString);
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            retval = BaiRongDataProvider.DatabaseDao.GetString(connectionString, queryString);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
