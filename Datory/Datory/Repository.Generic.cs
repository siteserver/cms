using System.Collections.Generic;
using Datory.Utils;
using SqlKata;

namespace Datory
{
    public partial class Repository<T> : IRepository where T : Entity, new()
    {
        public DatabaseType DatabaseType { get; }
        public string ConnectionString { get; }
        public string TableName { get; }
        public List<TableColumn> TableColumns { get; }
        public Query Q => new Query();

        public Repository()
        {
            DatabaseType = DatoryUtils.GetDatabaseType();
            ConnectionString = DatoryUtils.GetConnectionString();
            TableName = ReflectionUtils.GetTableName(typeof(T));
            TableColumns = ReflectionUtils.GetTableColumns(typeof(T));
        }

        public Repository(DatabaseType databaseType, string connectionString)
        {
            DatabaseType = databaseType;
            ConnectionString = connectionString;
            TableName = ReflectionUtils.GetTableName(typeof(T));
            TableColumns = ReflectionUtils.GetTableColumns(typeof(T));
        }

        public Repository(DatabaseType databaseType, string connectionString, string tableName)
        {
            DatabaseType = databaseType;
            ConnectionString = connectionString;
            TableName = tableName;
            TableColumns = ReflectionUtils.GetTableColumns(typeof(T));
        }

        public Repository(DatabaseType databaseType, string connectionString, string tableName, List<TableColumn> tableColumns)
        {
            DatabaseType = databaseType;
            ConnectionString = connectionString;
            TableName = tableName;
            TableColumns = tableColumns;
        }

        //        private readonly Lazy<Compiler> _compiler = new Lazy<Compiler>(() => SqlDifferences.GetCompiler(WebConfigUtils.DatabaseType));

        //        private KeyValuePair<string, Dictionary<string, object>> Compile(Query xQuery, Tuple<string, Dictionary<string, object>> replace = null)
        //        {
        //            string sql;
        //            Dictionary<string, object> bindings;
        //            var compiled = _compiler.Value.Compile(xQuery);

        //            if (replace != null)
        //            {
        //                sql = replace.Item1;
        //                bindings = replace.Item2;

        //                var index = compiled.Sql.IndexOf(" WHERE ", StringComparison.Ordinal);
        //                if (index != -1)
        //                {
        //                    sql += compiled.Sql.Substring(index);
        //                }
        //                foreach (var binding in compiled.NamedBindings)
        //                {
        //                    bindings[binding.Key] = binding.Value;    
        //                }

        //#if DEBUG
        //                FileUtils.AppendText(PathUtils.GetTemporaryFilesPath($"log.{DateUtils.GetDateString(DateTime.Now)}.sql"), $@"
        //sql:{sql}
        //bindings:{TranslateUtils.JsonSerialize(bindings)}
        //");
        //#endif
        //            }
        //            else
        //            {
        //                sql = compiled.Sql;
        //                bindings = compiled.NamedBindings;
        //#if DEBUG
        //                FileUtils.AppendText(PathUtils.GetTemporaryFilesPath($"log.{DateUtils.GetDateString(DateTime.Now)}.sql"), $@"
        //{compiled}
        //");
        //#endif
        //            }

        //            return new KeyValuePair<string, Dictionary<string, object>>(sql, bindings);
        //        }

        //        private static IDbConnection GetConnection()
        //        {
        //            return SqlDifferences.GetIDbConnection(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);
        //        }

        //        private Query GetXQuery(Query query)
        //        {
        //            return query != null ? query.Clone().From(TableName) : new Query(TableName);
        //        }

        //        protected Query Q => new Query();



        //        protected int InsertObject(T dataInfo)
        //        {
        //            if (dataInfo == null) return 0;
        //            dataInfo.Guid = StringUtils.GetGuid();
        //            dataInfo.LastModifiedDate = DateTime.Now;
        //            using (var connection = GetConnection())
        //            {
        //                dataInfo.Id = Convert.ToInt32(connection.InsertObject(dataInfo));
        //            }

        //            return dataInfo.Id;
        //        }

        //        protected bool Exists(int id)
        //        {
        //            return id > 0 && Exists(Q.Where(nameof(IDataInfo.Id), id));
        //        }

        //        protected bool Exists(string guid)
        //        {
        //            return StringUtils.IsGuid(guid) && Exists(Q.Where(nameof(IDataInfo.Guid), guid));
        //        }

        //        protected bool Exists(Query query = null)
        //        {
        //            bool exists;
        //            var xQuery = GetXQuery(query);
        //            xQuery.ClearComponent("select").SelectRaw("COUNT(1)").ClearComponent("order");
        //            var result = Compile(xQuery);
        //            using (var connection = GetConnection())
        //            {
        //                exists = connection.ExecuteScalar<bool>(result.Key, result.Value);
        //            }

        //            return exists;
        //        }

        //        protected int Count(Query query = null)
        //        {
        //            int count;
        //            var xQuery = GetXQuery(query);
        //            xQuery.ClearComponent("order").AsCount();
        //            var result = Compile(xQuery);
        //            using (var connection = GetConnection())
        //            {
        //                count = connection.ExecuteScalar<int>(result.Key, result.Value);
        //            }

        //            return count;
        //        }

        //        protected TValue GetValueById<TValue>(string columnName, int id)
        //        {
        //            return id <= 0
        //                ? default(TValue)
        //                : GetValueById<TValue>(columnName, Q.Where(nameof(IDataInfo.Id), id));
        //        }

        //        protected TValue GetValueById<TValue>(string columnName, string guid)
        //        {
        //            return !StringUtils.IsGuid(guid)
        //                ? default(TValue)
        //                : GetValueById<TValue>(columnName, Q.Where(nameof(IDataInfo.Guid), guid));
        //        }

        //        protected TValue GetValueById<TValue>(string columnName, Query query)
        //        {
        //            if (query == null) return default(TValue);

        //            TValue value;

        //            var xQuery = GetXQuery(query);
        //            xQuery.ClearComponent("select").Select(columnName).Limit(1);
        //            var result = Compile(xQuery);
        //            using (var connection = GetConnection())
        //            {
        //                value = connection.QueryFirstOrDefault<TValue>(result.Key, result.Value);
        //            }

        //            return value;
        //        }

        //        protected dynamic GetValues(Query query)
        //        {
        //            if (query == null) return null;

        //            dynamic value;

        //            var xQuery = GetXQuery(query);
        //            var result = Compile(xQuery);
        //            using (var connection = GetConnection())
        //            {
        //                value = connection.QueryFirstOrDefault(result.Key, result.Value);
        //            }

        //            return value;
        //        }

        //        protected int GetMaxValue(string columnName, Query query = null)
        //        {
        //            int? value;

        //            var xQuery = GetXQuery(query);
        //            xQuery.AsMax(columnName);
        //            var result = Compile(xQuery);
        //            using (var connection = GetConnection())
        //            {
        //                value = connection.QueryFirstOrDefault<int?>(result.Key, result.Value);
        //            }

        //            return value ?? 0;
        //        }

        //        protected IList<TValue> GetAllValue<TValue>(string columnName, Query query = null)
        //        {
        //            IList<TValue> values;

        //            var xQuery = GetXQuery(query);
        //            xQuery.ClearComponent("select").Select(columnName);
        //            var result = Compile(xQuery);
        //            using (var connection = GetConnection())
        //            {
        //                values = connection.Query<TValue>(result.Key, result.Value).ToList();
        //            }

        //            return values;
        //        }

        //        protected IList<dynamic> GetAllValues(Query query)
        //        {
        //            IList<dynamic> values;
        //            var xQuery = GetXQuery(query);
        //            var result = Compile(xQuery);
        //            using (var connection = GetConnection())
        //            {
        //                values = connection.Query<dynamic>(result.Key, result.Value).ToList();
        //            }

        //            return values;
        //        }

        //        protected T GetObjectById(int id)
        //        {
        //            return id <= 0 ? null : GetObjectById(Q.Where(nameof(IDataInfo.Id), id));
        //        }

        //        protected T GetObjectById(string guid)
        //        {
        //            return !StringUtils.IsGuid(guid) ? null : GetObjectById(Q.Where(nameof(IDataInfo.Guid), guid));
        //        }

        //        protected T GetObjectById(Query query = null)
        //        {
        //            T value;
        //            var xQuery = GetXQuery(query);
        //            xQuery.ClearComponent("select").SelectRaw("*").Limit(1);
        //            var result = Compile(xQuery);
        //            using (var connection = GetConnection())
        //            {
        //                value = connection.QueryFirstOrDefault<T>(result.Key, result.Value);
        //            }

        //            return _CheckGuid(value);
        //        }

        //        protected IList<T> GetObjectList(Query query = null)
        //        {
        //            IList<T> values;
        //            var xQuery = GetXQuery(query);
        //            xQuery.ClearComponent("select").SelectRaw("*");
        //            var result = Compile(xQuery);
        //            using (var connection = GetConnection())
        //            {
        //                values = connection.Query<T>(result.Key, result.Value).ToList();
        //            }
        //            foreach (var dataInfo in values)
        //            {
        //                _CheckGuid(dataInfo);
        //            }
        //            return values;
        //        }

        //        protected bool UpdateObject(T dataInfo)
        //        {
        //            bool updated;
        //            if (dataInfo == null) return false;

        //            if (!StringUtils.IsGuid(dataInfo.Guid))
        //            {
        //                dataInfo.Guid = StringUtils.GetGuid();
        //            }
        //            dataInfo.LastModifiedDate = DateTime.Now;

        //            using (var connection = GetConnection())
        //            {
        //                updated = connection.UpdateObject(dataInfo);
        //            }

        //            return updated;
        //        }

        //        protected bool UpdateObject(T dataInfo, params string[] columnNames)
        //        {
        //            var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        //            foreach (var columnName in columnNames)
        //            {
        //                values[columnName] = ReflectionUtils.GetValueById(dataInfo, columnName);
        //            }

        //            if (dataInfo.Id > 0)
        //            {
        //                return UpdateObject(values, dataInfo.Id);
        //            }
        //            if (StringUtils.IsGuid(dataInfo.Guid))
        //            {
        //                return UpdateObject(values, dataInfo.Guid);
        //            }

        //            return false;
        //        }

        //        protected bool UpdateObject(IDictionary<string, object> values, int id)
        //        {
        //            if (id <= 0) return false;

        //            return UpdateObject(values, Q.Where(nameof(IDataInfo.Id), id)) > 0;
        //        }

        //        protected bool UpdateObject(IDictionary<string, object> values, string guid)
        //        {
        //            if (!StringUtils.IsGuid(guid)) return false;

        //            return UpdateObject(values, Q.Where(nameof(IDataInfo.Guid), guid)) > 0;
        //        }

        //        protected int UpdateObject(IDictionary<string, object> values, Query query = null)
        //        {
        //            if (values == null || values.Count == 0) return 0;

        //            int affected;
        //            var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        //            foreach (var key in values.Keys)
        //            {
        //                dictionary[key] = values[key];
        //            }
        //            dictionary[nameof(IDataInfo.LastModifiedDate)] = DateTime.Now;
        //            var xQuery = GetXQuery(query);
        //            xQuery.AsUpdate(dictionary);
        //            var result = Compile(xQuery);
        //            using (var connection = GetConnection())
        //            {
        //                affected = connection.Execute(result.Key, result.Value);
        //            }

        //            return affected;
        //        }

        //        protected bool DeleteById(int id)
        //        {
        //            if (id <= 0) return false;

        //            return DeleteAll(Q.Where(nameof(IDataInfo.Id), id)) > 0;
        //        }

        //        protected bool DeleteById(string guid)
        //        {
        //            if (!StringUtils.IsGuid(guid)) return false;

        //            return DeleteAll(Q.Where(nameof(IDataInfo.Guid), guid)) > 0;
        //        }

        //        protected int DeleteAll(Query query = null)
        //        {
        //            int affected;
        //            var xQuery = GetXQuery(query);
        //            xQuery.AsDelete();
        //            var result = Compile(xQuery);
        //            using (var connection = GetConnection())
        //            {
        //                affected = connection.Execute(result.Key, result.Value);
        //            }

        //            return affected;
        //        }

        //        protected int IncrementAll(string columnName, Query query, int num = 1)
        //        {
        //            if (string.IsNullOrWhiteSpace(columnName)) return 0;

        //            int affected;
        //            var sql =
        //                $"UPDATE {TableName} SET {columnName} = {SqlDifferences.ColumnIncrement(columnName, num)}, {nameof(IDataInfo.LastModifiedDate)} = @{nameof(IDataInfo.LastModifiedDate)}";
        //            var bindings = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        //            {
        //                [nameof(IDataInfo.LastModifiedDate)] = DateTime.Now
        //            };
        //            var xQuery = GetXQuery(query);
        //            var result = Compile(xQuery, new Tuple<string, Dictionary<string, object>>(sql, bindings));
        //            using (var connection = GetConnection())
        //            {
        //                affected = connection.Execute(result.Key, result.Value);
        //            }

        //            return affected;
        //        }

        //        //private int _IncrementAll(string columnName, Query query, int num)
        //        //{
        //        //    return Connection.Execute($"UPDATE {TableName} SET {columnName} = {SqlDifferences.ColumnIncrement(columnName, num)}, {nameof(IDataInfo.LastModifiedDate)} = @{nameof(IDataInfo.LastModifiedDate)} {query.SqlWhereString}", query.SqlParameters);
        //        //}

        //        protected int DecrementAll(string columnName, Query query, int num = 1)
        //        {
        //            if (string.IsNullOrWhiteSpace(columnName)) return 0;

        //            int affected;
        //            var sql =
        //                $"UPDATE {TableName} SET {columnName} = {SqlDifferences.ColumnDecrement(columnName, num)}, {nameof(IDataInfo.LastModifiedDate)} = @{nameof(IDataInfo.LastModifiedDate)}";
        //            var bindings = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        //            {
        //                [nameof(IDataInfo.LastModifiedDate)] = DateTime.Now
        //            };
        //            var xQuery = GetXQuery(query);
        //            var result = Compile(xQuery, new Tuple<string, Dictionary<string, object>>(sql, bindings));
        //            using (var connection = GetConnection())
        //            {
        //                affected = connection.Execute(result.Key, result.Value);
        //            }

        //            return affected;
        //        }

        //        //private int _DecrementAll(string columnName, Query query, int num)
        //        //{
        //        //    return Connection.Execute($"UPDATE {TableName} SET {columnName} = {SqlDifferences.ColumnDecrement(columnName, num)}, {nameof(IDataInfo.LastModifiedDate)} = @{nameof(IDataInfo.LastModifiedDate)} {query.SqlWhereString}", query.SqlParameters);
        //        //}

        //        //private static string GetParamName(string columnName)
        //        //{
        //        //    var now = DateTime.Now;
        //        //    return $"{columnName}{now.Second}{now.Millisecond}";
        //        //}

        //        private T _CheckGuid(T dataInfo)
        //        {
        //            if (dataInfo != null && !StringUtils.IsGuid(dataInfo.Guid))
        //            {
        //                UpdateObject(dataInfo);
        //            }

        //            return dataInfo;
        //        }
    }
}
