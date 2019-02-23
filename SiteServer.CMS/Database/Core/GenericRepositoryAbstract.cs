using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using SiteServer.Plugin;
using SiteServer.Utils;
using SqlKata;
using SqlKata.Compilers;

namespace SiteServer.CMS.Database.Core
{
    public abstract class GenericRepositoryAbstract: IRepository
    {
        public abstract string TableName { get; }
        public abstract List<TableColumn> TableColumns { get; }

        private readonly Lazy<Compiler> _compiler = new Lazy<Compiler>(() => SqlDifferences.GetCompiler(WebConfigUtils.DatabaseType));

        private KeyValuePair<string, Dictionary<string, object>> Compile(Query xQuery, Tuple<string, Dictionary<string, object>> replace = null)
        {
            string sql;
            Dictionary<string, object> bindings;
            var compiled = _compiler.Value.Compile(xQuery);

            if (replace != null)
            {
                sql = replace.Item1;
                bindings = replace.Item2;

                var index = compiled.Sql.IndexOf(" WHERE ", StringComparison.Ordinal);
                if (index != -1)
                {
                    sql += compiled.Sql.Substring(index);
                }
                foreach (var binding in compiled.NamedBindings)
                {
                    bindings[binding.Key] = binding.Value;
                }

//#if DEBUG
//                FileUtils.AppendText(PathUtils.GetTemporaryFilesPath($"log.{DateUtils.GetDateString(DateTime.Now)}.sql"), $@"
//sql:{sql}
//bindings:{TranslateUtils.JsonSerialize(bindings)}
//");
//#endif
            }
            else
            {
                sql = compiled.Sql;
                bindings = compiled.NamedBindings;
//#if DEBUG
//                FileUtils.AppendText(PathUtils.GetTemporaryFilesPath($"log.{DateUtils.GetDateString(DateTime.Now)}.sql"), $@"
//{compiled}
//");
//#endif
            }

            return new KeyValuePair<string, Dictionary<string, object>>(sql, bindings);
        }

        private Query GetXQuery(Query query)
        {
            return query != null ? query.Clone().From(TableName) : new Query(TableName);
        }

        private IDbConnection GetConnection()
        {
            return SqlDifferences.GetIDbConnection(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);
        }

        protected Query Q => new Query();

        protected int InsertObject<T>(T dataInfo) where T : class, IDataInfo
        {
            if (dataInfo == null) return 0;
            dataInfo.Guid = StringUtils.GetGuid();
            dataInfo.LastModifiedDate = DateTime.Now;
            using (var connection = GetConnection())
            {
                dataInfo.Id = Convert.ToInt32(connection.Insert(dataInfo));
            }

            return dataInfo.Id;
        }

        protected bool Exists(int id)
        {
            return id > 0 && Exists(Q.Where(nameof(IDataInfo.Id), id));
        }

        protected bool Exists(string guid)
        {
            return StringUtils.IsGuid(guid) && Exists(Q.Where(nameof(IDataInfo.Guid), guid));
        }

        protected bool Exists(Query query = null)
        {
            bool exists;
            var xQuery = GetXQuery(query);
            xQuery.ClearComponent("select").SelectRaw("COUNT(1)").ClearComponent("order");
            var result = Compile(xQuery);
            using (var connection = GetConnection())
            {
                exists = connection.ExecuteScalar<bool>(result.Key, result.Value);
            }

            return exists;
        }

        protected int Count(Query query = null)
        {
            int count;
            var xQuery = GetXQuery(query);
            xQuery.ClearComponent("order").AsCount();
            var result = Compile(xQuery);
            using (var connection = GetConnection())
            {
                count = connection.ExecuteScalar<int>(result.Key, result.Value);
            }

            return count;
        }

        protected TValue GetValue<TValue>(Query query)
        {
            if (query == null) return default(TValue);

            TValue value;

            var xQuery = GetXQuery(query);
            xQuery.Limit(1);
            var result = Compile(xQuery);
            using (var connection = GetConnection())
            {
                value = connection.QueryFirstOrDefault<TValue>(result.Key, result.Value);
            }

            return value;
        }

        protected IList<TValue> GetValueList<TValue>(Query query = null)
        {
            IList<TValue> values;

            var xQuery = GetXQuery(query);
            var result = Compile(xQuery);
            using (var connection = GetConnection())
            {
                values = connection.Query<TValue>(result.Key, result.Value).ToList();
            }

            return values;
        }

        protected int Max(string columnName, Query query = null)
        {
            int? value;

            var xQuery = GetXQuery(query);
            xQuery.AsMax(columnName);
            var result = Compile(xQuery);
            using (var connection = GetConnection())
            {
                value = connection.QueryFirstOrDefault<int?>(result.Key, result.Value);
            }

            return value ?? 0;
        }

        protected T GetObjectById<T>(int id) where T : class, IDataInfo
        {
            return id <= 0 ? null : GetObject<T>(Q.Where(nameof(IDataInfo.Id), id));
        }

        protected T GetObjectByGuid<T>(string guid) where T : class, IDataInfo
        {
            return !StringUtils.IsGuid(guid) ? null : GetObject<T>(Q.Where(nameof(IDataInfo.Guid), guid));
        }

        protected T GetObject<T>(Query query = null) where T : class, IDataInfo
        {
            T value;
            var xQuery = GetXQuery(query);
            xQuery.ClearComponent("select").SelectRaw("*").Limit(1);
            var result = Compile(xQuery);
            using (var connection = GetConnection())
            {
                value = connection.QueryFirstOrDefault<T>(result.Key, result.Value);
            }

            return _CheckGuid(value);
        }

        protected IList<T> GetObjectList<T>(Query query = null) where T : class, IDataInfo
        {
            IList<T> values;
            var xQuery = GetXQuery(query);
            xQuery.ClearComponent("select").SelectRaw("*");
            var result = Compile(xQuery);
            using (var connection = GetConnection())
            {
                values = connection.Query<T>(result.Key, result.Value).ToList();
            }
            foreach (var dataInfo in values)
            {
                _CheckGuid(dataInfo);
            }
            return values;
        }

        protected bool UpdateObject<T>(T dataInfo) where T : class, IDataInfo
        {
            bool updated;
            if (dataInfo == null) return false;

            if (!StringUtils.IsGuid(dataInfo.Guid))
            {
                dataInfo.Guid = StringUtils.GetGuid();
            }
            dataInfo.LastModifiedDate = DateTime.Now;

            using (var connection = GetConnection())
            {
                updated = connection.Update(dataInfo);
            }

            return updated;
        }

        //protected bool UpdateById(IDictionary<string, object> values, int id)
        //{
        //    if (id <= 0) return false;

        //    return UpdateValue(values, Q.Where(nameof(IDataInfo.Id), id)) > 0;
        //}

        //protected bool UpdateByGuid(IDictionary<string, object> values, string guid)
        //{
        //    if (!StringUtils.IsGuid(guid)) return false;

        //    return UpdateValue(values, Q.Where(nameof(IDataInfo.Guid), guid)) > 0;
        //}

        protected bool UpdateObject<T>(T dataInfo, params string[] columnNames) where T : class, IDataInfo
        {
            var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var columnName in columnNames)
            {
                values[columnName] = ReflectionUtils.GetValue(dataInfo, columnName);
            }

            if (dataInfo.Id > 0)
            {
                return UpdateValue(values, Q.Where(nameof(IDataInfo.Id), dataInfo.Id)) > 0;
            }
            if (StringUtils.IsGuid(dataInfo.Guid))
            {
                return UpdateValue(values, Q.Where(nameof(IDataInfo.Guid), dataInfo.Guid)) > 0;
            }

            return false;
        }

        protected int UpdateValue(IDictionary<string, object> values, Query query = null)
        {
            if (values == null || values.Count == 0) return 0;

            int affected;
            var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in values.Keys)
            {
                dictionary[key] = values[key];
            }
            dictionary[nameof(IDataInfo.LastModifiedDate)] = DateTime.Now;
            var xQuery = GetXQuery(query);
            xQuery.AsUpdate(dictionary);
            var result = Compile(xQuery);
            using (var connection = GetConnection())
            {
                affected = connection.Execute(result.Key, result.Value);
            }

            return affected;
        }

        protected bool DeleteById(int id)
        {
            if (id <= 0) return false;

            return DeleteAll(Q.Where(nameof(IDataInfo.Id), id)) > 0;
        }

        protected bool DeleteByGuid(string guid)
        {
            if (!StringUtils.IsGuid(guid)) return false;

            return DeleteAll(Q.Where(nameof(IDataInfo.Guid), guid)) > 0;
        }

        protected int DeleteAll(Query query = null)
        {
            int affected;
            var xQuery = GetXQuery(query);
            xQuery.AsDelete();
            var result = Compile(xQuery);
            using (var connection = GetConnection())
            {
                affected = connection.Execute(result.Key, result.Value);
            }

            return affected;
        }

        protected int IncrementAll(string columnName, Query query, int num = 1)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return 0;

            int affected;
            var sql =
                $"UPDATE {TableName} SET {columnName} = {SqlDifferences.ColumnIncrement(columnName, num)}, {nameof(IDataInfo.LastModifiedDate)} = @{nameof(IDataInfo.LastModifiedDate)}";
            var bindings = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(IDataInfo.LastModifiedDate)] = DateTime.Now
            };
            var xQuery = GetXQuery(query);
            var result = Compile(xQuery, new Tuple<string, Dictionary<string, object>>(sql, bindings));
            using (var connection = GetConnection())
            {
                affected = connection.Execute(result.Key, result.Value);
            }

            return affected;
        }

        protected int DecrementAll(string columnName, Query query, int num = 1)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return 0;

            int affected;
            var sql =
                $"UPDATE {TableName} SET {columnName} = {SqlDifferences.ColumnDecrement(columnName, num)}, {nameof(IDataInfo.LastModifiedDate)} = @{nameof(IDataInfo.LastModifiedDate)}";
            var bindings = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(IDataInfo.LastModifiedDate)] = DateTime.Now
            };
            var xQuery = GetXQuery(query);
            var result = Compile(xQuery, new Tuple<string, Dictionary<string, object>>(sql, bindings));
            using (var connection = GetConnection())
            {
                affected = connection.Execute(result.Key, result.Value);
            }

            return affected;
        }

        private T _CheckGuid<T>(T dataInfo) where T : class, IDataInfo
        {
            if (dataInfo != null && !StringUtils.IsGuid(dataInfo.Guid))
            {
                UpdateObject(dataInfo);
            }

            return dataInfo;
        }
    }
}
