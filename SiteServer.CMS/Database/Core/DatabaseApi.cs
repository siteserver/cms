using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using Datory;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using SiteServer.CMS.Fx;
using SiteServer.Utils;
using TableColumn = Datory.TableColumn;

namespace SiteServer.CMS.Database.Core
{
    public class DatabaseApi
    {
        public int Execute(string sql, object param = null)
        {
            int affected;
            using (var connection = DatoryUtils.GetConnection(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
            {
                affected = connection.Execute(sql, param);
            }

            return affected;
        }

        private int GetIntResult(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = WebConfigUtils.ConnectionString;
            }

            var count = 0;

            using (var conn = DatoryUtils.GetConnection(WebConfigUtils.DatabaseType, connectionString))
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
                {
                    if (rdr.Read())
                    {
                        count = GetInt(rdr, 0);
                    }
                    rdr.Close();
                }
            }
            return count;
        }

        public int GetIntResult(string sqlString)
        {
            var count = 0;

            using (var conn = DatoryUtils.GetConnection(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
                {
                    if (rdr.Read())
                    {
                        count = GetInt(rdr, 0);
                    }
                    rdr.Close();
                }
            }
            return count;
        }

        public string GetString(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = WebConfigUtils.ConnectionString;
            }

            var retVal = string.Empty;

            using (var conn = DatoryUtils.GetConnection(WebConfigUtils.DatabaseType, connectionString))
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
                {
                    if (rdr.Read())
                    {
                        retVal = GetString(rdr, 0);
                    }
                    rdr.Close();
                }
            }
            return retVal;
        }

        public List<string> GetStringList(string sqlString)
        {
            var list = new List<string>();

            using (var conn = DatoryUtils.GetConnection(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
                {
                    while (rdr.Read())
                    {
                        list.Add(GetString(rdr, 0));
                    }
                    rdr.Close();
                }
            }
            return list;
        }

        public IDataReader GetDataReader(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = WebConfigUtils.ConnectionString;
            }

            return string.IsNullOrEmpty(sqlString) ? null : ExecuteReader(connectionString, sqlString);
        }

        public IDataReader GetDataSource(string sqlString)
        {
            if (string.IsNullOrEmpty(sqlString)) return null;
            var enumerable = ExecuteReader(WebConfigUtils.ConnectionString, sqlString);
            return enumerable;
        }

        public DataTable GetDataTable(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = WebConfigUtils.ConnectionString;
            }

            if (string.IsNullOrEmpty(sqlString)) return null;
            var dataSet = ExecuteDataset(connectionString, sqlString);

            if (dataSet == null || dataSet.Tables.Count == 0) return null;

            return dataSet.Tables[0];
        }

        public DataSet GetDataSet(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = WebConfigUtils.ConnectionString;
            }

            if (string.IsNullOrEmpty(sqlString)) return null;
            return ExecuteDataset(connectionString, sqlString);
        }

        public int GetPageTotalCount(string sqlString)
        {
            var temp = sqlString.ToLower();
            var pos = temp.LastIndexOf("order by", StringComparison.Ordinal);
            if (pos > -1)
                sqlString = sqlString.Substring(0, pos);

            // Add new ORDER BY info if SortKeyField is specified
            //if (!string.IsNullOrEmpty(sortField) && addCustomSortInfo)
            //    SelectCommand += " ORDER BY " + SortField;

            var cmdText = WebConfigUtils.DatabaseType == DatabaseType.Oracle
                ? $"SELECT COUNT(*) FROM ({sqlString})"
                : $"SELECT COUNT(*) FROM ({sqlString}) AS T0";
            return GetIntResult(cmdText);
        }

        public string GetStlPageSqlString(string sqlString, string orderString, int totalCount, int itemsPerPage, int currentPageIndex)
        {
            var retVal = string.Empty;

            var temp = sqlString.ToLower();
            var pos = temp.LastIndexOf("order by", StringComparison.Ordinal);
            if (pos > -1)
                sqlString = sqlString.Substring(0, pos);

            var recordsInLastPage = itemsPerPage;

            // Calculate the correspondent number of pages
            var lastPage = totalCount / itemsPerPage;
            var remainder = totalCount % itemsPerPage;
            if (remainder > 0)
                lastPage++;
            var pageCount = lastPage;

            if (remainder > 0)
                recordsInLastPage = remainder;

            var toRetrieve = itemsPerPage;
            if (currentPageIndex == pageCount - 1)
                toRetrieve = recordsInLastPage;

            orderString = orderString.ToUpper();
            var orderStringReverse = orderString.Replace(" DESC", " DESC2");
            orderStringReverse = orderStringReverse.Replace(" ASC", " DESC");
            orderStringReverse = orderStringReverse.Replace(" DESC2", " ASC");

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderStringReverse} LIMIT {toRetrieve}
) AS t2 {orderString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $@"
SELECT * FROM (
    SELECT TOP {toRetrieve} * FROM (
        SELECT TOP {itemsPerPage * (currentPageIndex + 1)} * FROM ({sqlString}) AS t0 {orderString}
    ) AS t1 {orderStringReverse}
) AS t2 {orderString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderStringReverse} LIMIT {toRetrieve}
) AS t2 {orderString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) WHERE ROWNUM <= {itemsPerPage * (currentPageIndex + 1)} {orderString}
    ) WHERE ROWNUM <= {toRetrieve} {orderStringReverse}
) {orderString}";
            }

            return retVal;
        }

        public bool ConnectToServer(DatabaseType databaseType, string connectionStringWithoutDatabaseName, out List<string> databaseNameList, out string errorMessage)
        {
            errorMessage = string.Empty;
            databaseNameList = new List<string>();
            try
            {
                databaseNameList = GetDatabaseNameList(databaseType, connectionStringWithoutDatabaseName);
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }

            return false;
        }

        private List<string> GetDatabaseNameList(DatabaseType databaseType, string connectionStringWithoutDatabaseName)
        {
            if (string.IsNullOrEmpty(connectionStringWithoutDatabaseName))
            {
                connectionStringWithoutDatabaseName = WebConfigUtils.ConnectionString;
            }

            var list = new List<string>();

            if (databaseType == DatabaseType.MySql)
            {
                var connection = new MySqlConnection(connectionStringWithoutDatabaseName);
                var command = new MySqlCommand("show databases", connection);

                connection.Open();

                var rdr = command.ExecuteReader();

                while (rdr.Read())
                {
                    var dbName = rdr.GetString(0);
                    if (dbName == null) continue;
                    if (dbName != "information_schema" &&
                        dbName != "mysql" &&
                        dbName != "performance_schema" &&
                        dbName != "sakila" &&
                        dbName != "sys" &&
                        dbName != "world")
                    {
                        list.Add(dbName);
                    }
                }

                connection.Close();
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                var connection = new SqlConnection(connectionStringWithoutDatabaseName);
                var command = new SqlCommand("select name from master..sysdatabases order by name asc", connection);

                connection.Open();

                connection.ChangeDatabase("master");

                var dr = command.ExecuteReader();

                while (dr.Read())
                {
                    var dbName = dr["name"] as string;
                    if (dbName == null) continue;
                    if (dbName != "master" &&
                        dbName != "msdb" &&
                        dbName != "tempdb" &&
                        dbName != "model")
                    {
                        list.Add(dbName);
                    }
                }

                connection.Close();
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                var connection = new NpgsqlConnection(connectionStringWithoutDatabaseName);
                var command =
                    new NpgsqlCommand(
                        "select datname from pg_database where datistemplate = false order by datname asc",
                        connection);

                connection.Open();

                var dr = command.ExecuteReader();

                while (dr.Read())
                {
                    var dbName = dr["datname"] as string;
                    if (dbName == null) continue;

                    list.Add(dbName);
                }

                connection.Close();
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                var connection = new OracleConnection(connectionStringWithoutDatabaseName);
                connection.Open();
                connection.Close();
            }

            return list;
        }

        public bool IsConnectionStringWork(DatabaseType databaseType, string connectionString)
        {
            var retVal = false;
            try
            {
                var connection = DatoryUtils.GetConnection(databaseType, connectionString);
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    retVal = true;
                    connection.Close();
                }
            }
            catch
            {
                // ignored
            }

            return retVal;
        }

        private string GetOracleSequence(string tableName, string idColumnName)
        {
            var cacheKey = $"SiteServer.CMS.Provider.{nameof(DatabaseApi)}.{nameof(GetOracleSequence)}.{tableName}.{idColumnName}";
            var sequence = CacheUtils.Get<string>(cacheKey);
            if (string.IsNullOrEmpty(sequence))
            {
                using (var conn = new OracleConnection(WebConfigUtils.ConnectionString))
                {
                    conn.Open();
                    var cmd = new OracleCommand
                    {
                        CommandType = CommandType.Text,
                        CommandText = $"SELECT DATA_DEFAULT FROM all_tab_cols WHERE OWNER = '{WebConfigUtils.ConnectionStringUserId.ToUpper()}' and table_name = '{tableName.ToUpper()}' AND column_name = '{idColumnName.ToUpper()}'",
                        Connection = conn,
                        InitialLONGFetchSize = -1
                    };

                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            var dataDefault = rdr.GetValue(0).ToString();

                            if (dataDefault.Contains(".nextval"))
                            {
                                sequence = dataDefault.Replace(".nextval", string.Empty);
                            }
                        }
                        rdr.Close();
                    }
                }

                CacheUtils.Insert(cacheKey, sequence);
            }

            return sequence;
        }

        public string GetSelectSqlString(string tableName, string whereString)
        {
            return GetSelectSqlString(tableName, 0, new List<string> { "*" }, whereString, null);
        }

        public string GetSelectSqlString(string tableName, int totalNum, IList<string> columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(WebConfigUtils.ConnectionString, tableName, totalNum, columns, whereString, orderByString, string.Empty);
        }

        public string GetSqlString(DatabaseType databaseType, string connectionString, string tableName, IList<string> columnNames = null, string whereSqlString = null, string orderSqlString = null, int offset = 0, int limit = 0, bool distinct = false)
        {
            var select = distinct ? "SELECT DISTINCT" : "SELECT";
            var columns = columnNames != null && columnNames.Count > 0 ? string.Join(", ", columnNames) : "*";

            var retVal = string.Empty;

            if (offset == 0 && limit == 0)
            {
                return $@"{select} {columns} FROM {tableName} {whereSqlString} {orderSqlString}";
            }

            if (databaseType == DatabaseType.MySql)
            {
                retVal = limit == 0
                    ? $@"{select} {columns} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset}"
                    : $@"{select} {columns} FROM {tableName} {whereSqlString} {orderSqlString} LIMIT {limit} OFFSET {offset}";
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                var useLegacyPagination = DatoryUtils.IsUseLegacyPagination(databaseType, connectionString);

                if (useLegacyPagination)
                {
                    if (offset == 0)
                    {
                        retVal = $"{select} TOP {limit} {columns} FROM {tableName} {whereSqlString} {orderSqlString}";
                    }
                    else
                    {
                        var rowWhere = limit == 0
                            ? $@"WHERE [row_num] > {offset}"
                            : $@"WHERE [row_num] BETWEEN {offset + 1} AND {offset + limit}";

                        retVal = $@"SELECT * FROM (
    {select} {columns}, ROW_NUMBER() OVER ({orderSqlString}) AS [row_num] FROM [{tableName}] {whereSqlString}
) as T {rowWhere}";
                    }
                }
                else
                {
                    retVal = limit == 0
                        ? $"{select} {columns} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS"
                        : $"{select} {columns} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
                }
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                retVal = limit == 0
                    ? $@"{select} {columns} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset}"
                    : $@"{select} {columns} FROM {tableName} {whereSqlString} {orderSqlString} LIMIT {limit} OFFSET {offset}";
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                retVal = limit == 0
                    ? $"{select} {columns} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS"
                    : $"{select} {columns} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
            }

            return retVal;
        }

        private string GetSelectSqlString(string connectionString, string tableName, int totalNum, IList<string> columns, string whereString, string orderByString, string joinString)
        {
            if (!string.IsNullOrEmpty(whereString))
            {
                whereString = StringUtils.ReplaceStartsWith(whereString.Trim(), "AND", string.Empty);
                if (!StringUtils.StartsWithIgnoreCase(whereString, "WHERE "))
                {
                    whereString = "WHERE " + whereString;
                }
            }

            if (!string.IsNullOrEmpty(joinString))
            {
                whereString = joinString + " " + whereString;
            }

            return GetSqlString(WebConfigUtils.DatabaseType, connectionString, tableName, columns, whereString, orderByString, totalNum);
        }

        private string GetSelectSqlStringByQueryString(string queryString, int totalNum, string orderByString)
        {
            if (totalNum == 0 && string.IsNullOrEmpty(orderByString))
            {
                return queryString;
            }
            string sqlString;
            if (totalNum > 0)
            {
                sqlString = SqlUtils.ToTopSqlString(queryString, orderByString, totalNum);
            }
            else
            {
                sqlString = string.IsNullOrEmpty(orderByString) ? queryString : $"SELECT * FROM ({queryString}) tmp {orderByString}";
            }
            return sqlString;
        }

        public string GetSelectSqlStringByQueryString(string connectionString, string queryString, int startNum, int totalNum, string orderByString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = WebConfigUtils.ConnectionString;
            }

            if (startNum == 1 && totalNum == 0 && string.IsNullOrEmpty(orderByString))
            {
                return queryString;
            }
            //queryString = queryString.Trim().ToUpper();
            if (queryString.LastIndexOf(" ORDER ", StringComparison.Ordinal) != -1)
            {
                if (string.IsNullOrEmpty(orderByString))
                {
                    orderByString = queryString.Substring(queryString.LastIndexOf(" ORDER ", StringComparison.Ordinal) + 1);
                }
                queryString = queryString.Substring(0, queryString.LastIndexOf(" ORDER ", StringComparison.Ordinal));
            }
            orderByString = ParseOrderByString(orderByString);

            if (startNum <= 1)
            {
                return GetSelectSqlStringByQueryString(queryString, totalNum, orderByString);
            }

            string countSqlString = $"SELECT Count(*) FROM ({queryString}) tmp";
            var count = GetIntResult(connectionString, countSqlString);
            if (totalNum == 0)
            {
                totalNum = count;
            }

            if (startNum > count) return string.Empty;

            var topNum = startNum + totalNum - 1;

            if (count < topNum)
            {
                totalNum = count - startNum + 1;
                if (totalNum < 1)
                {
                    return GetSelectSqlStringByQueryString(queryString, totalNum, orderByString);
                }
            }

            var orderByStringOpposite = GetOrderByStringOpposite(orderByString);

            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({queryString}) tmp {orderByString} LIMIT {topNum}
    ) AS tmp {orderByStringOpposite} LIMIT {totalNum}
) AS tmp {orderByString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $@"
SELECT *
FROM (SELECT TOP {totalNum} *
        FROM (SELECT TOP {topNum} *
                FROM ({queryString}) tmp {orderByString}) tmp
        {orderByStringOpposite}) tmp
{orderByString}
";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({queryString}) tmp {orderByString} LIMIT {topNum}
    ) AS tmp {orderByStringOpposite} LIMIT {totalNum}
) AS tmp {orderByString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $@"
SELECT *
FROM (SELECT TOP {totalNum} *
        FROM (SELECT TOP {topNum} *
                FROM ({queryString}) tmp {orderByString}) tmp
        {orderByStringOpposite}) tmp
{orderByString}
";
            }

            return retVal;
        }

        private static string ParseOrderByString(string orderByString)
        {
            if (string.IsNullOrEmpty(orderByString)) return orderByString;

            orderByString = orderByString.ToUpper().Trim();
            if (!orderByString.StartsWith("ORDER BY"))
            {
                orderByString = "ORDER BY " + orderByString;
            }
            if (!orderByString.EndsWith("DESC") && !orderByString.EndsWith("ASC"))
            {
                orderByString = orderByString + " ASC";
            }
            return orderByString;
        }

        private string GetOrderByStringOpposite(string orderByString)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(orderByString))
            {
                retVal = orderByString.Replace(" DESC", " DESC_OPPOSITE").Replace(" ASC", " DESC").Replace(" DESC_OPPOSITE", " ASC");
            }
            return retVal;
        }

        public int GetCount(string tableName)
        {
            return GetIntResult($"select count(*) from {tableName}");
        }

        public IEnumerable<dynamic> GetObjects(string tableName)
        {
            IEnumerable<dynamic> objects;
            var sqlString = $"select * from {tableName}";

            using (var connection = DatoryUtils.GetConnection(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
            {
                connection.Open();

                objects = connection.Query(sqlString, null, null, false).ToList();
            }

            return objects;
        }

        public IEnumerable<dynamic> GetPageObjects(string tableName, string identityColumnName, int offset, int limit)
        {
            IEnumerable<dynamic> objects;
            var sqlString = GetSqlString(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, tableName, orderSqlString: $"ORDER BY {identityColumnName} ASC", offset: offset, limit: limit);

            using (var connection = DatoryUtils.GetConnection(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
            {
                connection.Open();

                objects = connection.Query(sqlString, null, null, false).ToList();
            }

            return objects;
        }

        public void InsertMultiple(string tableName, IEnumerable<JObject> items, List<TableColumn> tableColumns)
        {
            var columnNames = new StringBuilder();
            foreach (var tableColumn in tableColumns)
            {
                columnNames.Append($"{tableColumn.AttributeName},");
            }
            if (columnNames.Length > 0) columnNames.Length -= 1;

            var valuesList = new List<string>();
            var parameterList = new List<IDataParameter>();
            var index = 0;
            foreach (var item in items)
            {
                var dict = TranslateUtils.JsonGetDictionaryIgnoreCase(item);

                index++;
                var values = new StringBuilder();
                foreach (var tableColumn in tableColumns)
                {
                    if (string.IsNullOrEmpty(tableColumn?.AttributeName)) continue;

                    dict.TryGetValue(tableColumn.AttributeName, out var value);

                    if (tableColumn.DataType == DataType.Integer)
                    {
                        if (value == null) value = 0;
                        values.Append($"{Convert.ToInt32(value)},");
                    }
                    else if (tableColumn.DataType == DataType.Decimal)
                    {
                        if (value == null) value = 0;
                        values.Append($"{Convert.ToDecimal(value)},");
                    }
                    else if (tableColumn.DataType == DataType.Boolean)
                    {
                        var paramName = $"@{tableColumn.AttributeName}_{index}";
                        if (value == null) value = false;
                        values.Append($"{paramName},");
                        parameterList.Add(GetParameter(paramName, Convert.ToBoolean(value)));
                    }
                    else if (tableColumn.DataType == DataType.DateTime)
                    {
                        if (value == null) value = DateTime.Now;
                        values.Append($"{SqlUtils.GetComparableDateTime(Convert.ToDateTime(value))},");
                    }
                    else
                    {
                        var paramName = $"@{tableColumn.AttributeName}_{index}";
                        values.Append($"{paramName},");
                        parameterList.Add(GetParameter(paramName, Convert.ToString(value)));
                    }
                }

                if (values.Length > 0)
                {
                    values.Length -= 1;
                    valuesList.Add(values.ToString());

                    if (parameterList.Count > 1000)
                    {
                        InsertRows(tableName, columnNames.ToString(), valuesList, parameterList);
                        valuesList.Clear();
                        parameterList.Clear();
                    }
                }
            }

            if (valuesList.Count > 0 && parameterList.Count > 0)
            {
                InsertRows(tableName, columnNames.ToString(), valuesList, parameterList);
            }
        }

        private void InsertRows(string tableName, string columnNames, List<string> valuesList, List<IDataParameter> parameterList)
        {
            if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                var sqlStringBuilder = new StringBuilder($@"INSERT INTO {tableName} ({columnNames}) VALUES ");
                foreach (var values in valuesList)
                {
                    sqlStringBuilder.Append($"({values}), ");
                }
                sqlStringBuilder.Length -= 2;

                var sqlString = sqlStringBuilder.ToString();

                var isIdentityColumn = !StringUtils.EqualsIgnoreCase(tableName, DataProvider.Site.TableName);
                if (isIdentityColumn)
                {
                    sqlString = $@"
SET IDENTITY_INSERT {tableName} ON
{sqlString}
SET IDENTITY_INSERT {tableName} OFF
";
                }

                ExecuteNonQuery(WebConfigUtils.ConnectionString, sqlString, parameterList.ToArray());
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                var sqlStringBuilder = new StringBuilder("INSERT ALL");
                foreach (var values in valuesList)
                {
                    sqlStringBuilder.Append($@" INTO {tableName} ({columnNames}) VALUES ({values})");
                }

                sqlStringBuilder.Append(" SELECT 1 FROM DUAL");

                ExecuteNonQuery(WebConfigUtils.ConnectionString, sqlStringBuilder.ToString(), parameterList.ToArray());
            }
            else
            {
                var sqlStringBuilder = new StringBuilder($@"INSERT INTO {tableName} ({columnNames}) VALUES ");
                foreach (var values in valuesList)
                {
                    sqlStringBuilder.Append($"({values}), ");
                }
                sqlStringBuilder.Length -= 2;

                ExecuteNonQuery(WebConfigUtils.ConnectionString, sqlStringBuilder.ToString(), parameterList.ToArray());
            }
        }

        public int GetPageTotalCount(string tableName, string whereSqlString)
        {
            return GetIntResult($@"SELECT COUNT(*) FROM {tableName} {whereSqlString}");
        }

        public int GetPageTotalCount(string tableName, string whereSqlString, Dictionary<string, object> parameters)
        {
            int totalCount;

            using (var connection = DatoryUtils.GetConnection(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
            {
                totalCount = connection.QueryFirstOrDefault<int>($@"SELECT COUNT(*) FROM {tableName} {whereSqlString}", parameters);
            }

            return totalCount;
        }

        public virtual string GetString(IDataReader rdr, int i)
        {
            if (i < 0 || i >= rdr.FieldCount) return string.Empty;
            return rdr.IsDBNull(i) ? string.Empty : rdr.GetValue(i).ToString();
        }

        public int GetInt(IDataReader rdr, int i)
        {
            if (i < 0 || i >= rdr.FieldCount) return 0;
            return rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
        }
        
        protected enum AdoConnectionOwnership
        {
            Internal,
            External
        }

        private IDbDataAdapter GetDataAdapter()
        {
            return new SqlDataAdapter();
        }

        private IDataParameter GetParameter()
        {
            return new SqlParameter();
        }

        private void ClearCommand(IDbCommand command)
        {
            var canClear = true;

            foreach (IDataParameter commandParameter in command.Parameters)
            {
                if (commandParameter.Direction != ParameterDirection.Input)
                    canClear = false;

            }
            if (canClear)
            {
                command.Parameters.Clear();
            }
        }

        private IDataParameter GetBlobParameter(IDataParameter p)
        {
            // do nothing special for BLOBs...as far as we know now.
            return p;
        }
        
        public IDataParameter GetParameter(string name, object value)
        {
            var parameter = GetParameter();
            parameter.ParameterName = name;
            if (value is DateTime && (DateTime)value < DateUtils.SqlMinValue)
            {
                value = DateUtils.SqlMinValue;
            }
            parameter.Value = value;

            return parameter;
        }

        private void AttachParameters(IDbCommand command, IDataParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (commandParameters != null)
            {
                foreach (var p in commandParameters)
                {
                    if (p != null)
                    {
                        // Check for derived output value with no value assigned
                        if ((p.Direction == ParameterDirection.InputOutput ||
                             p.Direction == ParameterDirection.Input) &&
                            p.Value == null)
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p.DbType == DbType.Binary ? GetBlobParameter(p) : p);
                    }
                }
            }
        }

        private void PrepareCommand(IDbCommand command, IDbConnection connection, IDbTransaction transaction,
            string commandText, IDataParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException(nameof(commandText));
            if (WebConfigUtils.DatabaseType != DatabaseType.SqlServer)
            {
                commandText = commandText.Replace("[", string.Empty).Replace("]", string.Empty);
                if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
                {
                    commandText = commandText.Replace("@", ":");
                }
            }

            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                if (transaction.Connection == null)
                    throw new ArgumentException(
                        "The transaction was rolled back or commited, please provide an open transaction.",
                        nameof(transaction));
                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = CommandType.Text;

            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
        }

        private DataSet ExecuteDataset(IDbCommand command)
        {
            var mustCloseConnection = false;

            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
                mustCloseConnection = true;
            }

            // Create the DataAdapter & DataSet
            IDbDataAdapter da = null;
            try
            {
                da = GetDataAdapter();
                da.SelectCommand = command;

                var ds = new DataSet();

                // Fill the DataSet using default values for DataTable names, etc
                da.Fill(ds);

                // Detach the IDataParameters from the command object, so they can be used again
                // Don't do this...screws up output params -- cjb 
                //command.Parameters.Clear();

                // Return the DataSet
                return ds;
            }
            finally
            {
                if (mustCloseConnection)
                {
                    command.Connection.Close();
                }
                if (da != null)
                {
                    var id = da as IDisposable;
                    if (id != null)
                        id.Dispose();
                }
            }
        }

        public DataSet ExecuteDataset(string connectionString, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteDataset(connectionString, commandText, null);
        }

        private DataSet ExecuteDataset(string connectionString, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            // Create & open an IDbConnection, and dispose of it after we are done
            using (var connection = DatoryUtils.GetConnection(WebConfigUtils.DatabaseType, connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteDataset(connection, commandText, commandParameters);
            }
        }

        private DataSet ExecuteDataset(IDbConnection connection, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            // Create a command and prepare it for execution
            var cmd = connection.CreateCommand();
            bool mustCloseConnection;
            PrepareCommand(cmd, connection, null, commandText, commandParameters, out mustCloseConnection);

            var ds = ExecuteDataset(cmd);

            if (mustCloseConnection)
                connection.Close();

            // Return the DataSet
            return ds;
        }

        private void ExecuteNonQuery(IDbCommand command)
        {
            var mustCloseConnection = false;

            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
                mustCloseConnection = true;
            }

            if (command == null) throw new ArgumentNullException(nameof(command));

            command.ExecuteNonQuery();

            if (mustCloseConnection)
            {
                command.Connection.Close();
            }
        }

        public void ExecuteNonQuery(string connectionString, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            // Create & open an IDbConnection, and dispose of it after we are done
            using (var connection = DatoryUtils.GetConnection(WebConfigUtils.DatabaseType, connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                ExecuteNonQuery(connection, commandText, commandParameters);
            }
        }

        public int ExecuteNonQueryAndReturnId(string tableName, string idColumnName, string connectionString,
            string commandText,
            params IDataParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            int id;

            using (var conn = DatoryUtils.GetConnection(WebConfigUtils.DatabaseType, connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        id = ExecuteNonQueryAndReturnId(tableName, idColumnName, trans, commandText, commandParameters);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return id;
        }

        private int ExecuteNonQueryAndReturnId(string tableName, string idColumnName, IDbTransaction trans,
            string commandText, params IDataParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(commandText)) return 0;

            var id = 0;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                ExecuteNonQuery(trans, commandText, commandParameters);

                using (var rdr = ExecuteReader(trans, $"SELECT @@IDENTITY AS '{idColumnName}'"))
                {
                    if (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        id = rdr.GetInt32(0);
                    }
                    rdr.Close();
                }

                if (id == 0)
                {
                    trans.Rollback();
                }
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                ExecuteNonQuery(trans, commandText, commandParameters);

                using (var rdr = ExecuteReader(trans, $"SELECT @@IDENTITY AS '{idColumnName}'"))
                {
                    if (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        id = Convert.ToInt32(rdr[0]);
                    }
                    rdr.Close();
                }

                if (id == 0)
                {
                    trans.Rollback();
                }
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                commandText += " RETURNING " + idColumnName;

                using (var rdr = ExecuteReader(trans, commandText, commandParameters))
                {
                    if (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        id = rdr.GetInt32(0);
                    }
                    rdr.Close();
                }

                if (id == 0)
                {
                    trans.Rollback();
                }
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                ExecuteNonQuery(trans, commandText, commandParameters);

                var sequence = GetOracleSequence(tableName, idColumnName);

                if (!string.IsNullOrEmpty(sequence))
                {
                    using (var rdr = ExecuteReader(trans, $"SELECT {sequence}.currval from dual"))
                    {
                        if (rdr.Read() && !rdr.IsDBNull(0))
                        {
                            id = Convert.ToInt32(rdr[0]);
                        }
                        rdr.Close();
                    }
                }

                if (id == 0)
                {
                    trans.Rollback();
                }
            }

            return id;
        }

        private void ExecuteNonQuery(IDbConnection connection, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            // Create a command and prepare it for execution
            var cmd = connection.CreateCommand();
            PrepareCommand(cmd, connection, null, commandText, commandParameters, out var mustCloseConnection);

            // Finally, execute the command
            ExecuteNonQuery(cmd);

            if (mustCloseConnection)
                connection.Close();
        }

        private void ExecuteNonQuery(IDbTransaction transaction, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException(
                    "The transaction was rolled back or commited, please provide an open transaction.",
                    nameof(transaction));

            // Create a command and prepare it for execution
            var cmd = transaction.Connection.CreateCommand();
            PrepareCommand(cmd, transaction.Connection, transaction, commandText, commandParameters,
                out _);

            ExecuteNonQuery(cmd);
        }

        protected IDataReader ExecuteReader(IDbCommand command, AdoConnectionOwnership connectionOwnership)
        {
            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
                connectionOwnership = AdoConnectionOwnership.Internal;
            }

            // Create a reader

            // Call ExecuteReader with the appropriate CommandBehavior
            var dataReader = connectionOwnership == AdoConnectionOwnership.External
                ? command.ExecuteReader()
                : command.ExecuteReader(CommandBehavior.CloseConnection);

            ClearCommand(command);

            return dataReader;
        }

        private IDataReader ExecuteReader(IDbConnection connection, IDbTransaction transaction, string commandText,
            IDataParameter[] commandParameters, AdoConnectionOwnership connectionOwnership)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            var mustCloseConnection = false;
            // Create a command and prepare it for execution
            var cmd = connection.CreateCommand();
            try
            {
                PrepareCommand(cmd, connection, transaction, commandText, commandParameters, out mustCloseConnection);
                
                if (mustCloseConnection)
                {
                    connectionOwnership = AdoConnectionOwnership.Internal;
                }

                // Create a reader

                var dataReader = ExecuteReader(cmd, connectionOwnership);

                ClearCommand(cmd);

                return dataReader;
            }
            catch
            {
                if (mustCloseConnection)
                    connection.Close();
                throw;
            }
        }

        public IDataReader ExecuteReader(string connectionString, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteReader(connectionString, commandText, null);
        }

        public IDataReader ExecuteReader(string connectionString, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            IDbConnection connection = null;
            try
            {
                connection = DatoryUtils.GetConnection(WebConfigUtils.DatabaseType, connectionString);
                connection.Open();

                // Call the private overload that takes an internally owned connection in place of the connection string
                return ExecuteReader(connection, null, commandText, commandParameters, AdoConnectionOwnership.Internal);
            }
            catch
            {
                // If we fail to return the IDataReader, we need to close the connection ourselves
                connection?.Close();
                throw;
            }

        }

        private IDataReader ExecuteReader(IDbConnection connection, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteReader(connection, commandText, null);
        }

        private IDataReader ExecuteReader(IDbConnection connection, string commandText,
            params IDataParameter[] commandParameters)
        {
            // Pass through the call to the private overload using a null transaction value and an externally owned connection
            return ExecuteReader(connection, null, commandText, commandParameters, AdoConnectionOwnership.External);
        }

        private IDataReader ExecuteReader(IDbTransaction transaction, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteReader(transaction, commandText, null);
        }

        private IDataReader ExecuteReader(IDbTransaction transaction, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException(
                    "The transaction was rolled back or commited, please provide an open transaction.",
                    nameof(transaction));

            // Pass through to private overload, indicating that the connection is owned by the caller
            return ExecuteReader(transaction.Connection, transaction, commandText, commandParameters,
                AdoConnectionOwnership.External);
        }
    }
}
