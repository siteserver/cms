using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dapper;
using SqlKata;

[assembly: InternalsVisibleTo("SS.CMS.Data.Tests")]

namespace SS.CMS.Data.Utils
{
    internal static partial class RepositoryUtils
    {
        public static bool Exists(IDatabase database, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("COUNT(1)").ClearComponent("order");
            var (sql, bindings) = Compile(database, tableName, xQuery);

            using (var connection = database.GetConnection())
            {
                return connection.ExecuteScalar<bool>(sql, bindings);
            }
        }

        public static async Task<bool> ExistsAsync(IDatabase database, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("COUNT(1)").ClearComponent("order");
            var (sql, bindings) = Compile(database, tableName, xQuery);

            using (var connection = database.GetConnection())
            {
                return await connection.ExecuteScalarAsync<bool>(sql, bindings);
            }
        }

        public static int Count(IDatabase database, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("order").AsCount();
            var (sql, bindings) = Compile(database, tableName, xQuery);

            using (var connection = database.GetConnection())
            {
                return connection.ExecuteScalar<int>(sql, bindings);
            }
        }

        public static async Task<int> CountAsync(IDatabase database, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("order").AsCount();
            var (sql, bindings) = Compile(database, tableName, xQuery);

            using (var connection = database.GetConnection())
            {
                return await connection.ExecuteScalarAsync<int>(sql, bindings);
            }
        }

        //private static string GetFirstSelectColumnName(Query query)
        //{
        //    string column = null;

        //    var components = query?.GetComponents("select");
        //    if (components == null) return null;

        //    foreach (var clause in components)
        //    {
        //        if (!(clause is Column select)) continue;
        //        column = select.Name;
        //        break;
        //    }

        //    return column;
        //}

        public static int Sum(IDatabase database, string tableName, string columnName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.AsSum(columnName);
            var (sql, bindings) = Compile(database, tableName, xQuery);

            using (var connection = database.GetConnection())
            {
                return connection.ExecuteScalar<int>(sql, bindings);
            }
        }

        public static async Task<int> SumAsync(IDatabase database, string tableName, string columnName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.AsSum(columnName);
            var (sql, bindings) = Compile(database, tableName, xQuery);

            using (var connection = database.GetConnection())
            {
                return await connection.ExecuteScalarAsync<int>(sql, bindings);
            }
        }

        public static TValue GetValue<TValue>(IDatabase database, string tableName, Query query)
        {
            if (query == null) return default;

            var xQuery = NewQuery(tableName, query);
            xQuery.Limit(1);
            var (sql, bindings) = Compile(database, tableName, xQuery);

            using (var connection = database.GetConnection())
            {
                return connection.QueryFirstOrDefault<TValue>(sql, bindings);
            }
        }

        public static async Task<TValue> GetValueAsync<TValue>(IDatabase database, string tableName, Query query)
        {
            if (query == null) return default;

            var xQuery = NewQuery(tableName, query);
            xQuery.Limit(1);
            var (sql, bindings) = Compile(database, tableName, xQuery);

            using (var connection = database.GetConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<TValue>(sql, bindings);
            }
        }

        public static IEnumerable<TValue> GetValueList<TValue>(IDatabase database, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            var (sql, bindings) = Compile(database, tableName, xQuery);

            using (var connection = database.GetConnection())
            {
                return connection.Query<TValue>(sql, bindings);
            }
        }

        public static async Task<IEnumerable<TValue>> GetValueListAsync<TValue>(IDatabase database, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            var (sql, bindings) = Compile(database, tableName, xQuery);

            using (var connection = database.GetConnection())
            {
                return await connection.QueryAsync<TValue>(sql, bindings);
            }
        }

        public static int? Max(IDatabase database, string tableName, string columnName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.AsMax(columnName);
            var (sql, bindings) = Compile(database, tableName, xQuery);

            using (var connection = database.GetConnection())
            {
                return connection.QueryFirstOrDefault<int?>(sql, bindings);
            }
        }

        public static async Task<int?> MaxAsync(IDatabase database, string tableName, string columnName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.AsMax(columnName);
            var (sql, bindings) = Compile(database, tableName, xQuery);

            using (var connection = database.GetConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<int?>(sql, bindings);
            }
        }

        public static T GetObject<T>(IDatabase database, string tableName, Query query = null) where T : Entity
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("*").Limit(1);
            var (sql, bindings) = Compile(database, tableName, xQuery);

            T value;
            using (var connection = database.GetConnection())
            {
                value = connection.QueryFirstOrDefault<T>(sql, bindings);
            }

            SyncAndCheckGuid(database, tableName, value);
            return value;
        }

        public static async Task<T> GetObjectAsync<T>(IDatabase database, string tableName, Query query = null) where T : Entity
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("*").Limit(1);
            var (sql, bindings) = Compile(database, tableName, xQuery);

            T value;
            using (var connection = database.GetConnection())
            {
                value = await connection.QueryFirstOrDefaultAsync<T>(sql, bindings);
            }

            await SyncAndCheckGuidAsync(database, tableName, value);
            return value;
        }

        public static IEnumerable<T> GetObjectList<T>(IDatabase database, string tableName, Query query = null) where T : Entity
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("*");
            var (sql, bindings) = Compile(database, tableName, xQuery);

            IEnumerable<T> values;
            using (var connection = database.GetConnection())
            {
                values = connection.Query<T>(sql, bindings);
            }

            foreach (var dataInfo in values)
            {
                SyncAndCheckGuid(database, tableName, dataInfo);
            }
            return values;
        }

        public static async Task<IEnumerable<T>> GetObjectListAsync<T>(IDatabase database, string tableName, Query query = null) where T : Entity
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("*");
            var (sql, bindings) = Compile(database, tableName, xQuery);

            IEnumerable<T> values;
            using (var connection = database.GetConnection())
            {
                values = await connection.QueryAsync<T>(sql, bindings);
            }

            foreach (var dataInfo in values)
            {
                await SyncAndCheckGuidAsync(database, tableName, dataInfo);
            }
            return values;
        }
    }
}
