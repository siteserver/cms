using System.Collections.Generic;

namespace SiteServer.Abstractions
{
    public partial interface IDatabaseRepository
    {
        List<KeyValuePair<int, Dictionary<string, object>>> ParserGetSqlDataSource(string connectionString, string queryString);
    }
}
