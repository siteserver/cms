using System.Collections.Generic;
using Datory;

namespace SSCMS.Services
{
    public partial interface IDatabaseManager
    {
        List<KeyValuePair<int, Dictionary<string, object>>> ParserGetSqlDataSource(DatabaseType databaseType, string connectionString, string queryString);
    }
}
