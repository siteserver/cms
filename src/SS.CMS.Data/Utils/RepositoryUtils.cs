using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dapper;
using SqlKata;

[assembly: InternalsVisibleTo("SS.CMS.Data.Tests")]

namespace SS.CMS.Data.Utils
{
    internal static class RepositoryUtils
    {
        private static Query NewQuery(string tableName, Query query = null)
        {
            return query != null ? query.Clone().From(tableName) : new Query(tableName);
        }

        private static (string sql, Dictionary<string, object> namedBindings) Compile(IDb db, string tableName, Query query)
        {
            var method = query.Method;
            if (method == "update")
            {
                query.Method = "select";
            }

            string sql;
            Dictionary<string, object> namedBindings;

            var compiler = DbUtils.GetCompiler(db.DatabaseType, db.ConnectionString);
            var compiled = compiler.Compile(query);

            if (method == "update")
            {
                var bindings = new List<object>();

                var setList = new List<string>();
                var components = query.GetComponents("update");
                components.Add(new BasicCondition
                {
                    Column = nameof(Entity.LastModifiedDate),
                    Value = DateTime.Now
                });
                foreach (var clause in components)
                {
                    if (clause is RawCondition raw)
                    {
                        var set = compiler.WrapIdentifiers(raw.Expression);
                        if (setList.Contains(set, StringComparer.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        setList.Add(set);
                        if (raw.Bindings != null)
                        {
                            bindings.AddRange(raw.Bindings);
                        }
                    }
                    else if (clause is BasicCondition basic)
                    {
                        var set = compiler.Wrap(basic.Column) + " = ?";
                        if (setList.Contains(set, StringComparer.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        setList.Add(set);
                        bindings.Add(basic.Value);
                    }
                }
                bindings.AddRange(compiled.Bindings);
                //var index = compiled.Sql.IndexOf(" WHERE ", StringComparison.Ordinal);
                //var where = string.Empty;
                //if (index != -1)
                //{
                //    where = compiled.Sql.Substring(index);
                //}

                var result = new SqlResult
                {
                    Query = query
                };
                var where = compiler.CompileWheres(result);
                sql = $"UPDATE {tableName} SET { string.Join(", ", setList)} {where}";

                //sql = Helper.ExpandParameters(sql, "?", bindings.ToArray());
                sql = Helper.ReplaceAll(sql, "?", i => "@p" + i);

                namedBindings = Helper.Flatten(bindings).Select((v, i) => new { i, v })
                    .ToDictionary(x => "@p" + x.i, x => x.v);
            }
            else
            {
                sql = compiled.Sql;
                namedBindings = compiled.NamedBindings;
            }

            return (sql, namedBindings);
        }

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

        public static bool Exists(IDb db, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("COUNT(1)").ClearComponent("order");
            var (sql, bindings) = Compile(db, tableName, xQuery);

            using (var connection = db.GetConnection())
            {
                return connection.ExecuteScalar<bool>(sql, bindings);
            }
        }

        public static async Task<bool> ExistsAsync(IDb db, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("COUNT(1)").ClearComponent("order");
            var (sql, bindings) = Compile(db, tableName, xQuery);

            using (var connection = db.GetConnection())
            {
                return await connection.ExecuteScalarAsync<bool>(sql, bindings);
            }
        }

        public static int Count(IDb db, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("order").AsCount();
            var (sql, bindings) = Compile(db, tableName, xQuery);

            using (var connection = db.GetConnection())
            {
                return connection.ExecuteScalar<int>(sql, bindings);
            }
        }

        public static async Task<int> CountAsync(IDb db, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("order").AsCount();
            var (sql, bindings) = Compile(db, tableName, xQuery);

            using (var connection = db.GetConnection())
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

        public static int Sum(IDb db, string tableName, string columnName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.AsSum(columnName);
            var (sql, bindings) = Compile(db, tableName, xQuery);

            using (var connection = db.GetConnection())
            {
                return connection.ExecuteScalar<int>(sql, bindings);
            }
        }

        public static async Task<int> SumAsync(IDb db, string tableName, string columnName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.AsSum(columnName);
            var (sql, bindings) = Compile(db, tableName, xQuery);

            using (var connection = db.GetConnection())
            {
                return await connection.ExecuteScalarAsync<int>(sql, bindings);
            }
        }

        public static TValue GetValue<TValue>(IDb db, string tableName, Query query)
        {
            if (query == null) return default;

            var xQuery = NewQuery(tableName, query);
            xQuery.Limit(1);
            var (sql, bindings) = Compile(db, tableName, xQuery);

            using (var connection = db.GetConnection())
            {
                return connection.QueryFirstOrDefault<TValue>(sql, bindings);
            }
        }

        public static async Task<TValue> GetValueAsync<TValue>(IDb db, string tableName, Query query)
        {
            if (query == null) return default;

            var xQuery = NewQuery(tableName, query);
            xQuery.Limit(1);
            var (sql, bindings) = Compile(db, tableName, xQuery);

            using (var connection = db.GetConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<TValue>(sql, bindings);
            }
        }

        public static IEnumerable<TValue> GetValueList<TValue>(IDb db, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            var (sql, bindings) = Compile(db, tableName, xQuery);

            using (var connection = db.GetConnection())
            {
                return connection.Query<TValue>(sql, bindings);
            }
        }

        public static async Task<IEnumerable<TValue>> GetValueListAsync<TValue>(IDb db, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            var (sql, bindings) = Compile(db, tableName, xQuery);

            using (var connection = db.GetConnection())
            {
                return await connection.QueryAsync<TValue>(sql, bindings);
            }
        }

        public static int? Max(IDb db, string tableName, string columnName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.AsMax(columnName);
            var (sql, bindings) = Compile(db, tableName, xQuery);

            using (var connection = db.GetConnection())
            {
                return connection.QueryFirstOrDefault<int?>(sql, bindings);
            }
        }

        public static async Task<int?> MaxAsync(IDb db, string tableName, string columnName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.AsMax(columnName);
            var (sql, bindings) = Compile(db, tableName, xQuery);

            using (var connection = db.GetConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<int?>(sql, bindings);
            }
        }

        public static T GetObject<T>(IDb db, string tableName, Query query = null) where T : Entity
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("*").Limit(1);
            var (sql, bindings) = Compile(db, tableName, xQuery);

            T value;
            using (var connection = db.GetConnection())
            {
                value = connection.QueryFirstOrDefault<T>(sql, bindings);
            }

            SyncAndCheckGuid(db, tableName, value);
            return value;
        }

        public static async Task<T> GetObjectAsync<T>(IDb db, string tableName, Query query = null) where T : Entity
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("*").Limit(1);
            var (sql, bindings) = Compile(db, tableName, xQuery);

            T value;
            using (var connection = db.GetConnection())
            {
                value = await connection.QueryFirstOrDefaultAsync<T>(sql, bindings);
            }

            await SyncAndCheckGuidAsync(db, tableName, value);
            return value;
        }

        public static IEnumerable<T> GetObjectList<T>(IDb db, string tableName, Query query = null) where T : Entity
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("*");
            var (sql, bindings) = Compile(db, tableName, xQuery);

            IEnumerable<T> values;
            using (var connection = db.GetConnection())
            {
                values = connection.Query<T>(sql, bindings);
            }

            foreach (var dataInfo in values)
            {
                SyncAndCheckGuid(db, tableName, dataInfo);
            }
            return values;
        }

        public static async Task<IEnumerable<T>> GetObjectListAsync<T>(IDb db, string tableName, Query query = null) where T : Entity
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("*");
            var (sql, bindings) = Compile(db, tableName, xQuery);

            IEnumerable<T> values;
            using (var connection = db.GetConnection())
            {
                values = await connection.QueryAsync<T>(sql, bindings);
            }

            foreach (var dataInfo in values)
            {
                await SyncAndCheckGuidAsync(db, tableName, dataInfo);
            }
            return values;
        }

        public static int InsertObject<T>(IDb db, string tableName, IEnumerable<TableColumn> tableColumns, T dataInfo) where T : Entity
        {
            if (dataInfo == null) return 0;
            dataInfo.Guid = Utilities.GetGuid();
            dataInfo.CreationDate = DateTime.Now;
            dataInfo.LastModifiedDate = DateTime.Now;

            var dictionary = new Dictionary<string, object>();
            foreach (var tableColumn in tableColumns)
            {
                if (Utilities.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.Id))) continue;

                var value = tableColumn.IsExtend
                    ? Utilities.JsonSerialize(dataInfo.ToDictionary(dataInfo.GetColumnNames()))
                    : dataInfo.Get(tableColumn.AttributeName);

                dictionary[tableColumn.AttributeName] = value;
            }

            var xQuery = NewQuery(tableName);
            xQuery.AsInsert(dictionary, true);
            var (sql, bindings) = Compile(db, tableName, xQuery);

            using (var connection = db.GetConnection())
            {
                dataInfo.Id = connection.QueryFirst<int>(sql, bindings);
            }

            return dataInfo.Id;
        }

        public static async Task<int> InsertObjectAsync<T>(IDb db, string tableName, IEnumerable<TableColumn> tableColumns, T dataInfo) where T : Entity
        {
            if (dataInfo == null) return 0;
            dataInfo.Guid = Utilities.GetGuid();
            dataInfo.CreationDate = DateTime.Now;
            dataInfo.LastModifiedDate = DateTime.Now;

            var dictionary = new Dictionary<string, object>();
            foreach (var tableColumn in tableColumns)
            {
                if (Utilities.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.Id))) continue;

                var value = tableColumn.IsExtend
                    ? Utilities.JsonSerialize(dataInfo.ToDictionary(dataInfo.GetColumnNames()))
                    : dataInfo.Get(tableColumn.AttributeName);

                dictionary[tableColumn.AttributeName] = value;
            }

            var xQuery = NewQuery(tableName);
            xQuery.AsInsert(dictionary, true);
            var (sql, bindings) = Compile(db, tableName, xQuery);

            using (var connection = db.GetConnection())
            {
                dataInfo.Id = await connection.QueryFirstAsync<int>(sql, bindings);
            }

            return dataInfo.Id;
        }

        public static int DeleteAll(IDb db, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.AsDelete();

            var (sql, bindings) = Compile(db, tableName, xQuery);

            using (var connection = db.GetConnection())
            {
                return connection.Execute(sql, bindings);
            }
        }

        public static async Task<int> DeleteAllAsync(IDb db, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.AsDelete();

            var (sql, bindings) = Compile(db, tableName, xQuery);

            using (var connection = db.GetConnection())
            {
                return await connection.ExecuteAsync(sql, bindings);
            }
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
