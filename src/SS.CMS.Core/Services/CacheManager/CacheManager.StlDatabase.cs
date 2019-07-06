using System.Collections.Generic;
using System.Data;
using Dapper;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class CacheManager
    {
        public static int GetPageTotalCount(string sqlString)
        {
            // retval = DatabaseUtils.GetPageTotalCount(sqlString);
            return 0;
        }

        public static string GetStlPageSqlString(string sqlString, string orderByString, int totalNum, int pageNum, int currentPageIndex)
        {
            // retval = DatabaseUtils.GetStlPageSqlString(sqlString, orderByString, totalNum, pageNum,
            // currentPageIndex);
            return string.Empty;
        }

        public static string GetString(string connectionString, string queryString)
        {
            // retval = DatabaseUtils.GetString(connectionString, queryString);
            return string.Empty;
        }

        public static DataSet GetDataSet(string connectionString, string queryString)
        {
            // retval = DatabaseUtils.GetDataSet(connectionString, queryString);
            return null;
        }

        public static List<KeyValuePair<int, Dictionary<string, object>>> GetContainerSqlList(IDatabase database, string queryString)
        {
            var rows = new List<KeyValuePair<int, Dictionary<string, object>>>();
            var itemIndex = 0;
            using (var connection = database.GetConnection())
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
            return rows;
        }

        public static DataTable GetDataTable(string connectionString, string queryString)
        {
            // retval = DatabaseUtils.GetDataTable(connectionString, queryString);
            return null;
        }
    }
}
