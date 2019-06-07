using System;
using System.Collections.Generic;
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

        private static (string sql, Dictionary<string, object> namedBindings) Compile(DbContext dbContext, string tableName, Query query)
        {
            var method = query.Method;
            if (method == "update")
            {
                query.Method = "select";
            }

            string sql;
            Dictionary<string, object> namedBindings;

            var compiler = dbContext.GetCompiler();
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

        public static void SyncAndCheckGuid(DbContext dbContext, string tableName, Entity dataInfo)
        {
            if (dataInfo == null || dataInfo.Id <= 0) return;

            if (!string.IsNullOrEmpty(dataInfo.GetExtendColumnName()))
            {
                dataInfo.Sync(dataInfo.Get<string>(dataInfo.GetExtendColumnName()));
            }

            if (Utilities.IsGuid(dataInfo.Guid)) return;

            dataInfo.Guid = Utilities.GetGuid();
            dataInfo.LastModifiedDate = DateTime.Now;

            UpdateAll(dbContext, tableName, new Query()
                .Set(nameof(Entity.Guid), dataInfo.Guid)
                .Where(nameof(Entity.Id), dataInfo.Id)
            );
        }

        public static async Task SyncAndCheckGuidAsync(DbContext dbContext, string tableName, Entity dataInfo)
        {
            if (dataInfo == null || dataInfo.Id <= 0) return;

            if (!string.IsNullOrEmpty(dataInfo.GetExtendColumnName()))
            {
                dataInfo.Sync(dataInfo.Get<string>(dataInfo.GetExtendColumnName()));
            }

            if (Utilities.IsGuid(dataInfo.Guid)) return;

            dataInfo.Guid = Utilities.GetGuid();
            dataInfo.LastModifiedDate = DateTime.Now;

            await UpdateAllAsync(dbContext, tableName, new Query()
                .Set(nameof(Entity.Guid), dataInfo.Guid)
                .Where(nameof(Entity.Id), dataInfo.Id)
            );
        }

        public static bool Exists(DbContext dbContext, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("COUNT(1)").ClearComponent("order");
            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            using (var connection = dbContext.GetConnection())
            {
                return connection.ExecuteScalar<bool>(sql, bindings);
            }
        }

        public static async Task<bool> ExistsAsync(DbContext dbContext, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("COUNT(1)").ClearComponent("order");
            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            using (var connection = dbContext.GetConnection())
            {
                return await connection.ExecuteScalarAsync<bool>(sql, bindings);
            }
        }

        public static int Count(DbContext dbContext, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("order").AsCount();
            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            using (var connection = dbContext.GetConnection())
            {
                return connection.ExecuteScalar<int>(sql, bindings);
            }
        }

        public static async Task<int> CountAsync(DbContext dbContext, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("order").AsCount();
            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            using (var connection = dbContext.GetConnection())
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

        public static int Sum(DbContext dbContext, string tableName, string columnName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.AsSum(columnName);
            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            using (var connection = dbContext.GetConnection())
            {
                return connection.ExecuteScalar<int>(sql, bindings);
            }
        }

        public static async Task<int> SumAsync(DbContext dbContext, string tableName, string columnName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.AsSum(columnName);
            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            using (var connection = dbContext.GetConnection())
            {
                return await connection.ExecuteScalarAsync<int>(sql, bindings);
            }
        }

        public static TValue GetValue<TValue>(DbContext dbContext, string tableName, Query query)
        {
            if (query == null) return default;

            var xQuery = NewQuery(tableName, query);
            xQuery.Limit(1);
            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            using (var connection = dbContext.GetConnection())
            {
                return connection.QueryFirstOrDefault<TValue>(sql, bindings);
            }
        }

        public static async Task<TValue> GetValueAsync<TValue>(DbContext dbContext, string tableName, Query query)
        {
            if (query == null) return default;

            var xQuery = NewQuery(tableName, query);
            xQuery.Limit(1);
            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            using (var connection = dbContext.GetConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<TValue>(sql, bindings);
            }
        }

        public static IEnumerable<TValue> GetValueList<TValue>(DbContext dbContext, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            using (var connection = dbContext.GetConnection())
            {
                return connection.Query<TValue>(sql, bindings);
            }
        }

        public static async Task<IEnumerable<TValue>> GetValueListAsync<TValue>(DbContext dbContext, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            using (var connection = dbContext.GetConnection())
            {
                return await connection.QueryAsync<TValue>(sql, bindings);
            }
        }

        public static int? Max(DbContext dbContext, string tableName, string columnName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.AsMax(columnName);
            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            using (var connection = dbContext.GetConnection())
            {
                return connection.QueryFirstOrDefault<int?>(sql, bindings);
            }
        }

        public static async Task<int?> MaxAsync(DbContext dbContext, string tableName, string columnName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.AsMax(columnName);
            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            using (var connection = dbContext.GetConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<int?>(sql, bindings);
            }
        }

        public static T GetObject<T>(DbContext dbContext, string tableName, Query query = null) where T : Entity
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("*").Limit(1);
            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            T value;
            using (var connection = dbContext.GetConnection())
            {
                value = connection.QueryFirstOrDefault<T>(sql, bindings);
            }

            SyncAndCheckGuid(dbContext, tableName, value);
            return value;
        }

        public static async Task<T> GetObjectAsync<T>(DbContext dbContext, string tableName, Query query = null) where T : Entity
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("*").Limit(1);
            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            T value;
            using (var connection = dbContext.GetConnection())
            {
                value = await connection.QueryFirstOrDefaultAsync<T>(sql, bindings);
            }

            await SyncAndCheckGuidAsync(dbContext, tableName, value);
            return value;
        }

        public static IEnumerable<T> GetObjectList<T>(DbContext dbContext, string tableName, Query query = null) where T : Entity
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("*");
            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            IEnumerable<T> values;
            using (var connection = dbContext.GetConnection())
            {
                values = connection.Query<T>(sql, bindings);
            }

            foreach (var dataInfo in values)
            {
                SyncAndCheckGuid(dbContext, tableName, dataInfo);
            }
            return values;
        }

        public static async Task<IEnumerable<T>> GetObjectListAsync<T>(DbContext dbContext, string tableName, Query query = null) where T : Entity
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("*");
            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            IEnumerable<T> values;
            using (var connection = dbContext.GetConnection())
            {
                values = await connection.QueryAsync<T>(sql, bindings);
            }

            foreach (var dataInfo in values)
            {
                await SyncAndCheckGuidAsync(dbContext, tableName, dataInfo);
            }
            return values;
        }

        public static int InsertObject<T>(DbContext dbContext, string tableName, IEnumerable<TableColumn> tableColumns, T dataInfo) where T : Entity
        {
            if (dataInfo == null) return 0;
            dataInfo.Guid = Utilities.GetGuid();
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
            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            using (var connection = dbContext.GetConnection())
            {
                dataInfo.Id = connection.QueryFirst<int>(sql, bindings);
            }

            return dataInfo.Id;
        }

        public static async Task<int> InsertObjectAsync<T>(DbContext dbContext, string tableName, IEnumerable<TableColumn> tableColumns, T dataInfo) where T : Entity
        {
            if (dataInfo == null) return 0;
            dataInfo.Guid = Utilities.GetGuid();
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
            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            using (var connection = dbContext.GetConnection())
            {
                dataInfo.Id = await connection.QueryFirstAsync<int>(sql, bindings);
            }

            return dataInfo.Id;
        }

        public static int DeleteAll(DbContext dbContext, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.AsDelete();

            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            using (var connection = dbContext.GetConnection())
            {
                return connection.Execute(sql, bindings);
            }
        }

        public static async Task<int> DeleteAllAsync(DbContext dbContext, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.AsDelete();

            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            using (var connection = dbContext.GetConnection())
            {
                return await connection.ExecuteAsync(sql, bindings);
            }
        }

        public static int UpdateAll(DbContext dbContext, string tableName, Query query)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.Method = "update";

            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            using (var connection = dbContext.GetConnection())
            {
                return connection.Execute(sql, bindings);
            }
        }

        public static async Task<int> UpdateAllAsync(DbContext dbContext, string tableName, Query query)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.Method = "update";

            var (sql, bindings) = Compile(dbContext, tableName, xQuery);

            using (var connection = dbContext.GetConnection())
            {
                return await connection.ExecuteAsync(sql, bindings);
            }
        }

        public static int IncrementAll(DbContext dbContext, string tableName, string columnName, Query query, int num = 1)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery
                .ClearComponent("update")
                .SetRaw($"{columnName} = {dbContext.ColumnIncrement(columnName, num)}");

            return UpdateAll(dbContext, tableName, xQuery);
        }

        public static async Task<int> IncrementAllAsync(DbContext dbContext, string tableName, string columnName, Query query, int num = 1)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery
                .ClearComponent("update")
                .SetRaw($"{columnName} = {dbContext.ColumnIncrement(columnName, num)}");

            return await UpdateAllAsync(dbContext, tableName, xQuery);
        }

        public static int DecrementAll(DbContext dbContext, string tableName, string columnName, Query query, int num = 1)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery
                .ClearComponent("update")
                .SetRaw($"{columnName} = {dbContext.ColumnDecrement(columnName, num)}");

            return UpdateAll(dbContext, tableName, xQuery);
        }

        public static async Task<int> DecrementAllAsync(DbContext dbContext, string tableName, string columnName, Query query, int num = 1)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery
                .ClearComponent("update")
                .SetRaw($"{columnName} = {dbContext.ColumnDecrement(columnName, num)}");

            return await UpdateAllAsync(dbContext, tableName, xQuery);
        }
    }
}
