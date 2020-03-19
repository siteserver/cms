using System.Collections.Generic;

namespace SSCMS
{
    public partial interface IDatabaseManager
    {
        List<KeyValuePair<int, Dictionary<string, object>>> ParserGetSqlDataSource(string connectionString, string queryString);
    }
}
