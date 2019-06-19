using System.Collections.Generic;
using System.Data;
using Dapper;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class CacheManager
    {
        public int GetPageTotalCount(string sqlString)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(CacheManager), nameof(GetPageTotalCount),
                    sqlString);
            var retval = GetInt(cacheKey);
            if (retval != -1) return retval;

            retval = GetInt(cacheKey);
            if (retval == -1)
            {
                // retval = DatabaseUtils.GetPageTotalCount(sqlString);
                Set(cacheKey, retval);
            }

            return retval;
        }

        public string GetStlPageSqlString(string sqlString, string orderByString, int totalNum, int pageNum, int currentPageIndex)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(CacheManager), nameof(GetStlPageSqlString),
                       sqlString, orderByString, totalNum.ToString(), pageNum.ToString(), currentPageIndex.ToString());
            var retval = Get<string>(cacheKey);
            if (retval != null) return retval;

            retval = Get<string>(cacheKey);
            if (retval == null)
            {
                // retval = DatabaseUtils.GetStlPageSqlString(sqlString, orderByString, totalNum, pageNum,
                // currentPageIndex);
                Set(cacheKey, retval);
            }

            return retval;
        }

        public string GetString(string connectionString, string queryString)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(CacheManager), nameof(GetString),
                       connectionString, queryString);
            var retval = Get<string>(cacheKey);
            if (retval != null) return retval;

            retval = Get<string>(cacheKey);
            if (retval == null)
            {
                // retval = DatabaseUtils.GetString(connectionString, queryString);
                Set(cacheKey, retval);
            }

            return retval;
        }

        public DataSet GetDataSet(string connectionString, string queryString)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(CacheManager), nameof(GetDataSet),
                connectionString, queryString);
            var retval = Get<DataSet>(cacheKey);
            if (retval != null) return retval;

            retval = Get<DataSet>(cacheKey);
            if (retval == null)
            {
                // retval = DatabaseUtils.GetDataSet(connectionString, queryString);
                Set(cacheKey, retval);
            }

            return retval;
        }

        public List<KeyValuePair<int, Dictionary<string, object>>> GetContainerSqlList(IDb db, string queryString)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(CacheManager), nameof(GetContainerSqlList),
                db.DatabaseType.Value, db.ConnectionString, queryString);
            var retval = Get<List<KeyValuePair<int, Dictionary<string, object>>>>(cacheKey);
            if (retval != null) return retval;

            retval = Get<List<KeyValuePair<int, Dictionary<string, object>>>>(cacheKey);
            if (retval == null)
            {
                var rows = new List<KeyValuePair<int, Dictionary<string, object>>>();
                var itemIndex = 0;
                using (var connection = db.GetConnection())
                {
                    using (var reader = connection.ExecuteReader(queryString))
                    {
                        while (reader.Read())
                        {
                            var dict = new Dictionary<string, object>();
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                dict[reader.GetName(i)] = reader.GetValue(i);
                            }
                            rows.Add(new KeyValuePair<int, Dictionary<string, object>>(itemIndex, dict));
                        }
                    }
                }
                retval = rows;
                Set(cacheKey, retval);
            }

            return retval;
        }

        public DataTable GetDataTable(string connectionString, string queryString)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(CacheManager), nameof(GetDataTable),
                connectionString, queryString);
            var retval = Get<DataTable>(cacheKey);
            if (retval != null) return retval;

            retval = Get<DataTable>(cacheKey);
            if (retval == null)
            {
                // retval = DatabaseUtils.GetDataTable(connectionString, queryString);
                Set(cacheKey, retval);
            }

            return retval;
        }
    }
}
