using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dapper;
using SqlKata;

[assembly: InternalsVisibleTo("SS.CMS.Data.Tests")]

namespace SS.CMS.Data.Utils
{
    internal static partial class RepositoryUtils
    {
        // public static int DeleteAll(IDatabase database, string tableName, Query query = null)
        // {
        //     var xQuery = NewQuery(tableName, query);
        //     xQuery.AsDelete();

        //     var (sql, bindings) = Compile(database, tableName, xQuery);

        //     using (var connection = database.GetConnection())
        //     {
        //         return connection.Execute(sql, bindings);
        //     }
        // }

        public static async Task<int> DeleteAllAsync(IDatabase database, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.AsDelete();

            var (sql, bindings) = Compile(database, tableName, xQuery);

            using (var connection = database.GetConnection())
            {
                return await connection.ExecuteAsync(sql, bindings);
            }
        }
    }
}
