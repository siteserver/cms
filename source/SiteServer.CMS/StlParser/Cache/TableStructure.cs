using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class TableStructure
    {
        public static string GetSelectSqlStringByQueryString(string connectionString, string queryString, int startNum, int totalNum, string orderByString, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(TableStructure), nameof(GetSelectSqlStringByQueryString), guid, connectionString, queryString, startNum.ToString(), totalNum.ToString(), orderByString);
            var retval = Utils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            retval = BaiRongDataProvider.TableStructureDao.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, orderByString);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
        
    }
}
