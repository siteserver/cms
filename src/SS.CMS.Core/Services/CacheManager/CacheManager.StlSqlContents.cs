using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class CacheManager
    {
        public string GetSelectSqlStringByQueryString(string connectionString, string queryString, int startNum,
            int totalNum, string orderByString)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(CacheManager),
                       nameof(GetSelectSqlStringByQueryString), connectionString, queryString, startNum.ToString(),
                       totalNum.ToString(), orderByString);
            var retval = Get<string>(cacheKey);
            if (retval != null) return retval;

            retval = Get<string>(cacheKey);
            if (retval == null)
            {
                // retval = DatabaseUtils.GetSelectSqlStringByQueryString(connectionString,
                // queryString, startNum, totalNum, orderByString);
                Set(cacheKey, retval);
            }

            return retval;
        }
    }
}
