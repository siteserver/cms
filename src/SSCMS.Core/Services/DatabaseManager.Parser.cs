using System.Collections.Generic;
using Dapper;

namespace SSCMS.Core.Services
{
    public partial class DatabaseManager
    {
        public List<KeyValuePair<int, Dictionary<string, object>>> ParserGetSqlDataSource(string connectionString, string queryString)
        {
            var rows = new List<KeyValuePair<int, Dictionary<string, object>>>();
            var itemIndex = 0;
            using (var connection = GetConnection(connectionString))
            {
                using var reader = connection.ExecuteReader(queryString);
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
            return rows;
        }
    }
}
