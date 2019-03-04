using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using SiteServer.CMS.Database.Wrapper;
using SiteServer.Plugin;
using SiteServer.Utils;
using SqlKata;
using SqlKata.Compilers;

namespace SiteServer.CMS.Database.Core
{
    public static class RepositoryHelper
    {
        private static Query NewQuery(string tableName, Query query = null)
        {
            return query != null ? query.Clone().From(tableName) : new Query(tableName);
        }

        private static IDbConnection GetConnection()
        {
            return SqlDifferences.GetIDbConnection(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);
        }

        private static readonly Lazy<Compiler> Compiler = new Lazy<Compiler>(() => SqlDifferences.GetCompiler(WebConfigUtils.DatabaseType));

        private static (string sql, Dictionary<string, object> namedBindings) Compile(string tableName, Query query)
        {
            var method = query.Method;
            if (method == "update")
            {
                query.Method = "select";
            }

            string sql;
            Dictionary<string, object> namedBindings;
            var compiled = Compiler.Value.Compile(query);

            if (method == "update")
            {
                var bindings = new List<object>();

                var setList = new List<string>();
                var components = query.GetComponents("update");
                components.Add(new BasicCondition
                {
                    Column = nameof(DynamicEntity.LastModifiedDate),
                    Value = DateTime.Now
                });
                foreach (var clause in components)
                {
                    if (clause is RawCondition raw)
                    {
                        var set = Compiler.Value.WrapIdentifiers(raw.Expression);
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
                        var set = Compiler.Value.Wrap(basic.Column) + " = ?";
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
                var where = Compiler.Value.CompileWheres(result);
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

            Logger(sql, namedBindings);

            return (sql, namedBindings);
        }

        //private static (string sql, Dictionary<string, object> bindings) Compile(string sql, Dictionary<string, object> bindings, Query queryWhereOnly)
        //{
        //    var compiled = Compiler.Value.Compile(queryWhereOnly);

        //    var index = compiled.Sql.IndexOf(" WHERE ", StringComparison.Ordinal);
        //    if (index != -1)
        //    {
        //        sql += compiled.Sql.Substring(index);
        //    }
        //    foreach (var binding in compiled.NamedBindings)
        //    {
        //        bindings[binding.Key] = binding.Value;
        //    }

        //    Logger(sql, bindings);

        //    return (sql, bindings);
        //}

        private static void Logger(string sql, object bindings)
        {
#if DEBUG
            FileUtils.AppendTextAsync(PathUtils.GetTemporaryFilesPath($"log.{DateUtils.GetDateString(DateTime.Now)}.sql"),
                bindings != null
                    ? $@"
            sql:{sql}
            bindings:{TranslateUtils.JsonSerialize(bindings)}
            "
                    : $@"
            sql:{sql}
            ").ConfigureAwait(false);
#endif
        }

        private static T SyncAndCheckGuid<T>(string tableName, T dataInfo) where T : DynamicEntity
        {
            if (dataInfo == null || dataInfo.Id <= 0) return dataInfo;

            if (!string.IsNullOrEmpty(dataInfo.GetExtendColumnName()))
            {
                dataInfo.Sync(dataInfo.Get<string>(dataInfo.GetExtendColumnName()));
            }

            if (!StringUtils.IsGuid(dataInfo.Guid))
            {
                dataInfo.Guid = StringUtils.GetGuid();
                dataInfo.LastModifiedDate = DateTime.Now;

                UpdateAll(tableName, new Query()
                    .Set(nameof(DynamicEntity.Guid), dataInfo.Guid)
                    .Where(nameof(DynamicEntity.Id), dataInfo.Id)
                );
            }

            return dataInfo;
        }

        public static bool Exists(string tableName, Query query = null)
        {
            bool exists;
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("COUNT(1)").ClearComponent("order");
            var (sql, bindings) = Compile(tableName, xQuery);
            using (var connection = GetConnection())
            {
                exists = connection.ExecuteScalar<bool>(sql, bindings);
            }

            return exists;
        }

        public static int Count(string tableName, Query query = null)
        {
            int count;
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("order").AsCount();
            var (sql, bindings) = Compile(tableName, xQuery);
            using (var connection = GetConnection())
            {
                count = connection.ExecuteScalar<int>(sql, bindings);
            }

            return count;
        }

        public static int Sum(string tableName, string column, Query query = null)
        {
            int count;
            var xQuery = NewQuery(tableName, query);
            xQuery.AsSum(column);
            var (sql, bindings) = Compile(tableName, xQuery);
            using (var connection = GetConnection())
            {
                count = connection.ExecuteScalar<int>(sql, bindings);
            }

            return count;
        }

        public static TValue GetValue<TValue>(string tableName, Query query)
        {
            if (query == null) return default(TValue);

            TValue value;

            var xQuery = NewQuery(tableName, query);
            xQuery.Limit(1);
            var (sql, bindings) = Compile(tableName, xQuery);
            using (var connection = GetConnection())
            {
                value = connection.QueryFirstOrDefault<TValue>(sql, bindings);
            }

            return value;
        }

        public static IList<TValue> GetValueList<TValue>(string tableName, Query query = null)
        {
            IList<TValue> values;

            var xQuery = NewQuery(tableName, query);
            var (sql, bindings) = Compile(tableName, xQuery);
            using (var connection = GetConnection())
            {
                values = connection.Query<TValue>(sql, bindings).ToList();
            }

            return values;
        }

        public static int Max(string tableName, string columnName, Query query = null)
        {
            int? value;

            var xQuery = NewQuery(tableName, query);
            xQuery.AsMax(columnName);
            var (sql, bindings) = Compile(tableName, xQuery);
            using (var connection = GetConnection())
            {
                value = connection.QueryFirstOrDefault<int?>(sql, bindings);
            }

            return value ?? 0;
        }

        public static T GetObject<T>(string tableName, Query query = null) where T : DynamicEntity
        {
            T value;
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("*").Limit(1);
            var (sql, bindings) = Compile(tableName, xQuery);
            using (var connection = GetConnection())
            {
                value = connection.QueryFirstOrDefault<T>(sql, bindings);
            }

            return SyncAndCheckGuid(tableName, value);
        }

        public static IList<T> GetObjectList<T>(string tableName, Query query = null) where T : DynamicEntity
        {
            IList<T> values;
            var xQuery = NewQuery(tableName, query);
            xQuery.ClearComponent("select").SelectRaw("*");
            var (sql, bindings) = Compile(tableName, xQuery);
            using (var connection = GetConnection())
            {
                values = connection.Query<T>(sql, bindings).ToList();
            }
            foreach (var dataInfo in values)
            {
                SyncAndCheckGuid(tableName, dataInfo);
            }
            return values;
        }

        public static int InsertObject<T>(string tableName, List<TableColumn> tableColumns, T dataInfo) where T : DynamicEntity
        {
            if (dataInfo == null) return 0;
            dataInfo.Guid = StringUtils.GetGuid();
            dataInfo.LastModifiedDate = DateTime.Now;
            //using (var connection = GetConnection())
            //{
            //    dataInfo.Id = Convert.ToInt32(connection.Insert(dataInfo));
            //}

            //return dataInfo.Id;

            var dictionary = new Dictionary<string, object>();
            foreach (var tableColumn in tableColumns)
            {
                if (StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(DynamicEntity.Id))) continue;

                var value = tableColumn.IsExtend
                    ? TranslateUtils.JsonSerialize(dataInfo.ToDictionary(dataInfo.GetColumnNames()))
                    : dataInfo.Get(tableColumn.AttributeName);

                dictionary[tableColumn.AttributeName] = value;
            }

            var xQuery = NewQuery(tableName);
            xQuery.AsInsert(dictionary, true);
            var (sql, bindings) = Compile(tableName, xQuery);
            using (var connection = GetConnection())
            {
                dataInfo.Id = connection.QueryFirst<int>(sql, bindings);
            }

            return dataInfo.Id;
        }

        public static int DeleteAll(string tableName, Query query = null)
        {
            int affected;
            var xQuery = NewQuery(tableName, query);
            xQuery.AsDelete();

            var (sql, bindings) = Compile(tableName, xQuery);
            using (var connection = GetConnection())
            {
                affected = connection.Execute(sql, bindings);
            }

            return affected;
        }

        //public static int UpdateValue(string tableName, IDictionary<string, object> values, Query query = null)
        //{
        //    if (values == null || values.Count == 0) return 0;

        //    int affected;
        //    //var dictionary = new Dictionary<string, object>();
        //    //foreach (var key in values.Keys)
        //    //{
        //    //    dictionary[key] = values[key];
        //    //}
        //    //dictionary[nameof(IDataInfo.LastModifiedDate)] = DateTime.Now;
        //    var xQuery = NewQuery(tableName, query);
        //    xQuery.AsUpdate(values);

        //    var (sql, bindings) = Compile(tableName, xQuery);
        //    using (var connection = GetConnection())
        //    {
        //        affected = connection.Execute(sql, bindings);
        //    }

        //    return affected;
        //}

        public static int UpdateAll(string tableName, Query query)
        {
            int affected;
            var xQuery = NewQuery(tableName, query);

            xQuery.Method = "update";

            var (sql, bindings) = Compile(tableName, xQuery);
            using (var connection = GetConnection())
            {
                affected = connection.Execute(sql, bindings);
            }

            return affected;
        }

        public static int IncrementAll(string tableName, string columnName, Query query, int num = 1)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return 0;

            //int affected;
            //var sql =
            //    $"UPDATE {tableName} SET {columnName} = {SqlDifferences.ColumnIncrement(columnName, num)}, {nameof(IDataInfo.LastModifiedDate)} = @{nameof(IDataInfo.LastModifiedDate)}";
            //var bindings = new Dictionary<string, object>
            //{
            //    [nameof(IDataInfo.LastModifiedDate)] = DateTime.Now
            //};
            //var xQuery = NewQuery(tableName, query);
            //var result = Compile(sql, bindings, xQuery);
            //using (var connection = GetConnection())
            //{
            //    affected = connection.Execute(result.sql, result.bindings);
            //}

            //xQuery.ClearComponent("update")
            //    .UpdateRaw($"{columnName} = {SqlDifferences.ColumnIncrement(columnName, num)}", null);

            //return affected;

            var xQuery = NewQuery(tableName, query);
            xQuery
                .ClearComponent("update")
                .SetRaw($"{columnName} = {SqlDifferences.ColumnIncrement(columnName, num)}");

            return UpdateAll(tableName, xQuery);
        }

        public static int DecrementAll(string tableName, string columnName, Query query, int num = 1)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return 0;

            //int affected;
            //var sql =
            //    $"UPDATE {tableName} SET {columnName} = {SqlDifferences.ColumnDecrement(columnName, num)}, {nameof(IDataInfo.LastModifiedDate)} = @{nameof(IDataInfo.LastModifiedDate)}";
            //var bindings = new Dictionary<string, object>
            //{
            //    [nameof(IDataInfo.LastModifiedDate)] = DateTime.Now
            //};
            //var xQuery = NewQuery(tableName, query);
            //var result = Compile(sql, bindings, xQuery);
            //using (var connection = GetConnection())
            //{
            //    affected = connection.Execute(result.sql, result.bindings);
            //}

            //return affected;

            var xQuery = NewQuery(tableName, query);
            xQuery
                .ClearComponent("update")
                .SetRaw($"{columnName} = {SqlDifferences.ColumnDecrement(columnName, num)}");

            return UpdateAll(tableName, xQuery);
        }
    }
}
