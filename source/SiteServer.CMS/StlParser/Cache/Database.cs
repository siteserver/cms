using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Database
    {
        public static int GetPageTotalCount(string sqlString, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Database), nameof(GetPageTotalCount), guid, sqlString);
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = BaiRongDataProvider.DatabaseDao.GetPageTotalCount(sqlString);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static string GetStlPageSqlString(string sqlString, string orderByString, int totalNum, int pageNum, int currentPageIndex, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Database), nameof(GetStlPageSqlString), guid, sqlString, orderByString, totalNum.ToString(), pageNum.ToString(), currentPageIndex.ToString());
            var retval = Utils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            retval = BaiRongDataProvider.DatabaseDao.GetStlPageSqlString(sqlString, orderByString, totalNum, pageNum, currentPageIndex);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static string GetString(string connectionString, string queryString, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Database), nameof(GetString), guid, connectionString, queryString);
            var retval = Utils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            retval = BaiRongDataProvider.DatabaseDao.GetString(connectionString, queryString);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
