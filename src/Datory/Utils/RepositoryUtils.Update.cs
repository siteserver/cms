using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CacheManager.Core;
using Dapper;
using Newtonsoft.Json.Converters;
using SqlKata;

[assembly: InternalsVisibleTo("Datory.Data.Tests")]

namespace Datory.Utils
{
    internal static partial class RepositoryUtils
    {
        public static async Task SyncAndCheckGuidAsync(IDatabase database, string tableName, IRedis redis, Entity dataInfo)
        {
            if (dataInfo == null || dataInfo.Id <= 0) return;

            dataInfo.LoadExtend();

            if (Utilities.IsGuid(dataInfo.Guid)) return;

            dataInfo.Guid = Utilities.GetGuid();
            dataInfo.LastModifiedDate = DateTime.Now;

            await UpdateAllAsync(database, tableName, redis, new Query()
                .Set(nameof(Entity.Guid), dataInfo.Guid)
                .Where(nameof(Entity.Id), dataInfo.Id)
            );
        }

        public static async Task<int> UpdateAllAsync(IDatabase database, string tableName, IRedis redis, Query query)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.Method = "update";

            var compileInfo = await CompileAsync(database, tableName, redis, xQuery);

            using var connection = database.GetConnection();
            return await connection.ExecuteAsync(compileInfo.Sql, compileInfo.NamedBindings);
        }

        public static async Task<int> IncrementAllAsync(IDatabase database, string tableName, IRedis redis, string columnName, Query query, int num = 1)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery
                .ClearComponent("update")
                .SetRaw($"{database.GetQuotedIdentifier(columnName)} = {DbUtils.ColumnIncrement(database.DatabaseType, columnName, num)}");

            return await UpdateAllAsync(database, tableName, redis, xQuery);
        }

        public static async Task<int> DecrementAllAsync(IDatabase database, string tableName, IRedis redis, string columnName, Query query, int num = 1)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery
                .ClearComponent("update")
                .SetRaw($"{database.GetQuotedIdentifier(columnName)} = {DbUtils.ColumnDecrement(database.DatabaseType, columnName, num)}");

            return await UpdateAllAsync(database, tableName, redis, xQuery);
        }
    }
}
