using System.Collections.Generic;

namespace SS.CMS.Abstractions
{
    public partial interface IDatabaseManager
    {
        List<KeyValuePair<int, Dictionary<string, object>>> ParserGetSqlDataSource(string connectionString, string queryString);
    }
}
