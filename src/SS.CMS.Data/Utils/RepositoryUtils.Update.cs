using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dapper;
using SqlKata;

[assembly: InternalsVisibleTo("SS.CMS.Data.Tests")]

namespace SS.CMS.Data.Utils
{
    internal static partial class RepositoryUtils
    {
        public static void SyncAndCheckGuid(IDb db, string tableName, Entity dataInfo)
        {
            if (dataInfo == null || dataInfo.Id <= 0) return;

            if (!string.IsNullOrEmpty(dataInfo.GetExtendColumnName()))
            {
                dataInfo.Sync(dataInfo.Get<string>(dataInfo.GetExtendColumnName()));
            }

            if (Utilities.IsGuid(dataInfo.Guid)) return;

            dataInfo.Guid = Utilities.GetGuid();
            dataInfo.LastModifiedDate = DateTime.Now;

            UpdateAll(db, tableName, new Query()
                .Set(nameof(Entity.Guid), dataInfo.Guid)
                .Where(nameof(Entity.Id), dataInfo.Id)
            );
        }

        public static async Task SyncAndCheckGuidAsync(IDb db, string tableName, Entity dataInfo)
        {
            if (dataInfo == null || dataInfo.Id <= 0) return;

            if (!string.IsNullOrEmpty(dataInfo.GetExtendColumnName()))
            {
                dataInfo.Sync(dataInfo.Get<string>(dataInfo.GetExtendColumnName()));
            }

            if (Utilities.IsGuid(dataInfo.Guid)) return;

            dataInfo.Guid = Utilities.GetGuid();
            dataInfo.LastModifiedDate = DateTime.Now;

            await UpdateAllAsync(db, tableName, new Query()
                .Set(nameof(Entity.Guid), dataInfo.Guid)
                .Where(nameof(Entity.Id), dataInfo.Id)
            );
        }

        public static int UpdateAll(IDb db, string tableName, Query query)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.Method = "update";

            var (sql, bindings) = Compile(db, tableName, xQuery);

            using (var connection = db.GetConnection())
            {
                return connection.Execute(sql, bindings);
            }
        }

        public static async Task<int> UpdateAllAsync(IDb db, string tableName, Query query)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.Method = "update";

            var (sql, bindings) = Compile(db, tableName, xQuery);

            using (var connection = db.GetConnection())
            {
                return await connection.ExecuteAsync(sql, bindings);
            }
        }

        public static int IncrementAll(IDb db, string tableName, string columnName, Query query, int num = 1)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery
                .ClearComponent("update")
                .SetRaw($"{columnName} = {DbUtils.ColumnIncrement(db.DatabaseType, columnName, num)}");

            return UpdateAll(db, tableName, xQuery);
        }

        public static async Task<int> IncrementAllAsync(IDb db, string tableName, string columnName, Query query, int num = 1)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery
                .ClearComponent("update")
                .SetRaw($"{columnName} = {DbUtils.ColumnIncrement(db.DatabaseType, columnName, num)}");

            return await UpdateAllAsync(db, tableName, xQuery);
        }

        public static int DecrementAll(IDb db, string tableName, string columnName, Query query, int num = 1)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery
                .ClearComponent("update")
                .SetRaw($"{columnName} = {DbUtils.ColumnDecrement(db.DatabaseType, columnName, num)}");

            return UpdateAll(db, tableName, xQuery);
        }

        public static async Task<int> DecrementAllAsync(IDb db, string tableName, string columnName, Query query, int num = 1)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery
                .ClearComponent("update")
                .SetRaw($"{columnName} = {DbUtils.ColumnDecrement(db.DatabaseType, columnName, num)}");

            return await UpdateAllAsync(db, tableName, xQuery);
        }
    }
}
