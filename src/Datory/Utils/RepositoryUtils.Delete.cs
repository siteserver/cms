using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dapper;
using SqlKata;

[assembly: InternalsVisibleTo("Datory.Data.Tests")]

namespace Datory.Utils
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

        public static async Task<int> DeleteAllAsync(IDatabase database, string tableName, IRedis redis, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.AsDelete();

            var compileInfo = await CompileAsync(database, tableName, redis, xQuery);

            using var connection = database.GetConnection();
            return await connection.ExecuteAsync(compileInfo.Sql, compileInfo.NamedBindings);
        }
    }
}
