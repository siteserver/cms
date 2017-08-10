using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class TableStructure
    {
        private static readonly object LockObject = new object();

        public static string GetSelectSqlStringByQueryString(string connectionString, string queryString, int startNum,
            int totalNum, string orderByString, string guid)
        {
            lock (LockObject)
            {
                var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(TableStructure),
                    nameof(GetSelectSqlStringByQueryString), connectionString, queryString, startNum.ToString(),
                    totalNum.ToString(), orderByString);
                var retval = StlCacheUtils.GetCache<string>(cacheKey);
                if (retval != null) return retval;

                retval = BaiRongDataProvider.TableStructureDao.GetSelectSqlStringByQueryString(connectionString,
                    queryString, startNum, totalNum, orderByString);
                StlCacheUtils.SetCache(cacheKey, retval);
                return retval;
            }
        }
    }
}
