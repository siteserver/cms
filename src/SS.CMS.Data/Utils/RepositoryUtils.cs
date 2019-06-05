using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        private static (string sql, Dictionary<string, object> namedBindings) Compile(DatabaseType databaseType, string connectionString, string tableName, Query query)
        {
            var method = query.Method;
            if (method == "update")
            {
                query.Method = "select";
            }

            string sql;
            Dictionary<string, object> namedBindings;

            var compiler = DatoryUtils.GetCompiler(databaseType, connectionString);
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

        public static void SyncAndCheckGuid(DatabaseType databaseType, string connectionString, string tableName, Entity dataInfo)
        {
            if (dataInfo == null || dataInfo.Id <= 0) return;

            if (!string.IsNullOrEmpty(dataInfo.GetExtendColumnName()))
            {
                dataInfo.Sync(dataInfo.Get<string>(dataInfo.GetExtendColumnName()));
            }

            if (Utilities.IsGuid(dataInfo.Guid)) return;

            dataInfo.Guid = Utilities.GetGuid();
            dataInfo.LastModifiedDate = DateTime.Now;

            UpdateAll(databaseType, connectionString, tableName, new Query()
                .Set(nameof(Entity.Guid), dataInfo.Guid)
                .Where(nameof(Entity.Id), dataInfo.Id)
            );
        }

        public static bool Exists(DatabaseType databaseType, string connectionString, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("COUNT(1)").ClearComponent("order");
            var (sql, bindings) = Compile(databaseType, connectionString, tableName, xQuery);

            using (var connection = new Connection(databaseType, connectionString))
            {
                return connection.ExecuteScalar<bool>(sql, bindings);
            }
        }

        public static int Count(DatabaseType databaseType, string connectionString, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("order").AsCount();
            var (sql, bindings) = Compile(databaseType, connectionString, tableName, xQuery);

            using (var connection = new Connection(databaseType, connectionString))
            {
                return connection.ExecuteScalar<int>(sql, bindings);
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

        public static int Sum(DatabaseType databaseType, string connectionString, string tableName, string columnName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.AsSum(columnName);
            var (sql, bindings) = Compile(databaseType, connectionString, tableName, xQuery);

            using (var connection = new Connection(databaseType, connectionString))
            {
                return connection.ExecuteScalar<int>(sql, bindings);
            }
        }

        public static TValue GetValue<TValue>(DatabaseType databaseType, string connectionString, string tableName, Query query)
        {
            if (query == null) return default;

            var xQuery = NewQuery(tableName, query);
            xQuery.Limit(1);
            var (sql, bindings) = Compile(databaseType, connectionString, tableName, xQuery);

            using (var connection = new Connection(databaseType, connectionString))
            {
                return connection.QueryFirstOrDefault<TValue>(sql, bindings);
            }
        }

        public static IList<TValue> GetValueList<TValue>(DatabaseType databaseType, string connectionString, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            var (sql, bindings) = Compile(databaseType, connectionString, tableName, xQuery);

            using (var connection = new Connection(databaseType, connectionString))
            {
                return connection.Query<TValue>(sql, bindings).ToList();
            }
        }

        public static int? Max(DatabaseType databaseType, string connectionString, string tableName, string columnName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.AsMax(columnName);
            var (sql, bindings) = Compile(databaseType, connectionString, tableName, xQuery);

            using (var connection = new Connection(databaseType, connectionString))
            {
                return connection.QueryFirstOrDefault<int?>(sql, bindings);
            }
        }

        public static T GetObject<T>(DatabaseType databaseType, string connectionString, string tableName, Query query = null) where T : Entity
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("*").Limit(1);
            var (sql, bindings) = Compile(databaseType, connectionString, tableName, xQuery);

            T value;
            using (var connection = new Connection(databaseType, connectionString))
            {
                value = connection.QueryFirstOrDefault<T>(sql, bindings);
            }

            SyncAndCheckGuid(databaseType, connectionString, tableName, value);
            return value;
        }

        public static IList<T> GetObjectList<T>(DatabaseType databaseType, string connectionString, string tableName, Query query = null) where T : Entity
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("*");
            var (sql, bindings) = Compile(databaseType, connectionString, tableName, xQuery);

            List<T> values;
            using (var connection = new Connection(databaseType, connectionString))
            {
                values = connection.Query<T>(sql, bindings).ToList();
            }

            foreach (var dataInfo in values)
            {
                SyncAndCheckGuid(databaseType, connectionString, tableName, dataInfo);
            }
            return values;
        }

        public static int InsertObject<T>(DatabaseType databaseType, string connectionString, string tableName, IEnumerable<TableColumn> tableColumns, T dataInfo) where T : Entity
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
            var (sql, bindings) = Compile(databaseType, connectionString, tableName, xQuery);

            using (var connection = new Connection(databaseType, connectionString))
            {
                dataInfo.Id = connection.QueryFirst<int>(sql, bindings);
            }
            
            return dataInfo.Id;
        }

        public static int DeleteAll(DatabaseType databaseType, string connectionString, string tableName, Query query = null)
        {
            var xQuery = NewQuery(tableName, query);
            xQuery.AsDelete();

            var (sql, bindings) = Compile(databaseType, connectionString, tableName, xQuery);

            using (var connection = new Connection(databaseType, connectionString))
            {
                return connection.Execute(sql, bindings);
            }
        }

        public static int UpdateAll(DatabaseType databaseType, string connectionString, string tableName, Query query)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery.Method = "update";

            var (sql, bindings) = Compile(databaseType, connectionString, tableName, xQuery);

            using (var connection = new Connection(databaseType, connectionString))
            {
                return connection.Execute(sql, bindings);
            }
        }

        public static int IncrementAll(DatabaseType databaseType, string connectionString, string tableName, string columnName, Query query, int num = 1)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery
                .ClearComponent("update")
                .SetRaw($"{columnName} = {DatoryUtils.ColumnIncrement(databaseType, columnName, num)}");

            return UpdateAll(databaseType, connectionString, tableName, xQuery);
        }

        public static int DecrementAll(DatabaseType databaseType, string connectionString, string tableName, string columnName, Query query, int num = 1)
        {
            var xQuery = NewQuery(tableName, query);

            xQuery
                .ClearComponent("update")
                .SetRaw($"{columnName} = {DatoryUtils.ColumnDecrement(databaseType, columnName, num)}");

            return UpdateAll(databaseType, connectionString, tableName, xQuery);
        }
    }
}
