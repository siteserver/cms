using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Newtonsoft.Json.Linq;
using SS.CMS.Core.Cache;
using SS.CMS.Plugin.Data;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.Common
{
    public static class DatabaseUtils
    {
        public static void DeleteDbLog()
        {
            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                Execute("PURGE MASTER LOGS BEFORE DATE_SUB( NOW( ), INTERVAL 3 DAY)");
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(AppSettings.DatabaseType, AppSettings.ConnectionString);

                const string sqlCheck = "SELECT SERVERPROPERTY('productversion')";
                // var versions = ExecuteScalar(sqlCheck).ToString();

                string versions;
                using (var connection = new Connection(AppSettings.DatabaseType, AppSettings.ConnectionString))
                {
                    versions = connection.ExecuteScalar(sqlCheck).ToString();
                }

                var version = 8;
                var arr = versions.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length > 0)
                {
                    version = TranslateUtils.ToInt(arr[0], 8);
                }
                if (version < 10)
                {
                    //2000,2005
                    string sql = $"BACKUP LOG [{databaseName}] WITH NO_LOG";
                    Execute(sql);
                }
                else
                {
                    //2008+
                    string sql =
                        $@"ALTER DATABASE [{databaseName}] SET RECOVERY SIMPLE;DBCC shrinkfile ([{databaseName}_log], 1); ALTER DATABASE [{databaseName}] SET RECOVERY FULL; ";
                    Execute(sql);
                }
            }
        }

        public static void Execute(string sqlString)
        {
            if (string.IsNullOrEmpty(sqlString)) return;

            using (var connection = new Connection(AppSettings.DatabaseType, AppSettings.ConnectionString))
            {
                connection.Execute(sqlString);
            }
        }

        public static void Execute(string sqlString, Dictionary<string, object> dbArgs)
        {
            if (string.IsNullOrEmpty(sqlString)) return;

            using (var connection = new Connection(AppSettings.DatabaseType, AppSettings.ConnectionString))
            {
                connection.Execute(sqlString, dbArgs);
            }
        }

        public static int GetIntResult(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = AppSettings.ConnectionString;
            }

            var count = 0;

            using (var connection = new Connection(AppSettings.DatabaseType, connectionString))
            {
                using (var rdr = connection.ExecuteReader(sqlString))
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

        public static int GetIntResult(string sqlString)
        {
            var count = 0;

            using (var connection = new Connection(AppSettings.DatabaseType, AppSettings.ConnectionString))
            {
                using (var rdr = connection.ExecuteReader(sqlString))
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


        public static string GetString(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = AppSettings.ConnectionString;
            }

            var retval = string.Empty;

            using (var connection = new Connection(AppSettings.DatabaseType, connectionString))
            {
                using (var rdr = connection.ExecuteReader(sqlString))
                {
                    if (rdr.Read())
                    {
                        retval = GetString(rdr, 0);
                    }
                    rdr.Close();
                }
            }
            return retval;
        }

        public static string GetString(string sqlString)
        {
            var value = string.Empty;
            using (var connection = new Connection(AppSettings.DatabaseType, AppSettings.ConnectionString))
            {
                using (var rdr = connection.ExecuteReader(sqlString))
                {
                    if (rdr.Read())
                    {
                        value = GetString(rdr, 0);
                    }
                    rdr.Close();
                }
            }

            return value;
        }

        public static DataTable GetDataTable(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = AppSettings.ConnectionString;
            }

            if (string.IsNullOrEmpty(sqlString)) return null;

            var dataTable = new DataTable();
            using (var connection = new Connection(AppSettings.DatabaseType, connectionString))
            {
                using (var rdr = connection.ExecuteReader(sqlString))
                {
                    dataTable.Load(rdr);
                }
            }

            return dataTable;
        }

        public static DataSet GetDataSet(string sqlString)
        {
            return GetDataSet(AppSettings.ConnectionString, sqlString);
        }

        public static DataSet GetDataSet(string connectionString, string sqlString)
        {
            var dataTable = GetDataTable(connectionString, sqlString);
            return dataTable?.DataSet;
        }

        public static int GetPageTotalCount(string sqlString)
        {
            var temp = sqlString.ToLower();
            var pos = temp.LastIndexOf("order by", StringComparison.Ordinal);
            if (pos > -1)
                sqlString = sqlString.Substring(0, pos);

            var cmdText = AppSettings.DatabaseType == DatabaseType.Oracle
                ? $"SELECT COUNT(*) FROM ({sqlString})"
                : $"SELECT COUNT(*) FROM ({sqlString}) AS T0";
            return GetIntResult(cmdText);
        }

        public static bool ConnectToServer(DatabaseType databaseType, string connectionStringWithoutDatabaseName, out List<string> databaseNameList, out string errorMessage)
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

        public static bool IsTableExists(string tableName)
        {
            return DatoryUtils.IsTableExists(AppSettings.DatabaseType, AppSettings.ConnectionString, tableName);
        }

        public static bool CreateTable(string tableName, List<TableColumn> tableColumns, out Exception ex)
        {
            ex = null;

            try
            {
                DatoryUtils.CreateTable(AppSettings.DatabaseType, AppSettings.ConnectionString, tableName, tableColumns);
                TableColumnManager.ClearCache();
                return true;
            }
            catch (Exception e)
            {
                ex = e;
                LogUtils.AddErrorLog(ex, tableName);
                return false;
            }
        }

        public static void AlterOracleAutoIncresementIdToMaxValue(string tableName)
        {
            try
            {
                var sqlString =
                    $"ALTER TABLE {tableName} MODIFY Id GENERATED ALWAYS AS IDENTITY(START WITH LIMIT VALUE)";
                Execute(sqlString);
            }
            catch
            {
                // ignored
            }
        }

        public static List<string> GetDatabaseNameList(DatabaseType databaseType, string connectionStringWithoutDatabaseName)
        {
            if (string.IsNullOrEmpty(connectionStringWithoutDatabaseName))
            {
                connectionStringWithoutDatabaseName = AppSettings.ConnectionString;
            }

            var list = new List<string>();

            if (databaseType == DatabaseType.MySql)
            {
                using (var connection = new Connection(databaseType, connectionStringWithoutDatabaseName))
                {
                    using (var rdr = connection.ExecuteReader("show databases"))
                    {
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
                    }
                }
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                using (var connection = new Connection(databaseType, connectionStringWithoutDatabaseName))
                {
                    connection.ChangeDatabase("master");

                    using (var dr = connection.ExecuteReader("select name from master..sysdatabases order by name asc"))
                    {
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
                    }
                }
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                using (var connection = new Connection(databaseType, connectionStringWithoutDatabaseName))
                {
                    using (var dr = connection.ExecuteReader("select datname from pg_database where datistemplate = false order by datname asc"))
                    {
                        while (dr.Read())
                        {
                            var dbName = dr["datname"] as string;
                            if (dbName == null) continue;

                            list.Add(dbName);
                        }
                    }
                }
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                using (var connection = new Connection(databaseType, connectionStringWithoutDatabaseName))
                {
                    connection.Open();
                    connection.Close();
                }
            }

            return list;
        }

        public static bool IsConnectionStringWork(DatabaseType databaseType, string connectionString)
        {
            var retval = false;
            try
            {
                using (var connection = new Connection(databaseType, connectionString))
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        retval = true;
                        connection.Close();
                    }
                }
            }
            catch
            {
                // ignored
            }

            return retval;
        }

        public static List<TableColumn> GetTableColumnInfoList(string connectionString, string tableName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = AppSettings.ConnectionString;
            }

            return DatoryUtils.GetTableColumns(AppSettings.DatabaseType, connectionString, tableName);
        }

        public static string GetOracleSequence(string tableName, string idColumnName)
        {
            var cacheKey = $"SiteServer.CMS.Core.{nameof(DatabaseUtils)}.{nameof(GetOracleSequence)}.{tableName}.{idColumnName}";
            var sequence = CacheUtils.Get<string>(cacheKey);
            if (string.IsNullOrEmpty(sequence))
            {
                using (var connection = new Connection(AppSettings.DatabaseType, AppSettings.ConnectionString))
                {
                    using (var rdr = connection.ExecuteReader($"SELECT DATA_DEFAULT FROM all_tab_cols WHERE OWNER = '{AppSettings.ConnectionStringUserId.ToUpper()}' and table_name = '{tableName.ToUpper()}' AND column_name = '{idColumnName.ToUpper()}'"))
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

        public static string GetSelectSqlString(string tableName, string columns, string whereString)
        {
            return GetSelectSqlString(tableName, 0, columns, whereString, null);
        }

        public static string GetSelectSqlString(string tableName, int totalNum, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(AppSettings.ConnectionString, tableName, totalNum, columns, whereString, orderByString);
        }

        public static string GetSelectSqlString(string connectionString, string tableName, int totalNum, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(connectionString, tableName, totalNum, columns, whereString, orderByString, string.Empty);
        }

        public static string GetSelectSqlString(string connectionString, string tableName, int totalNum, string columns, string whereString, string orderByString, string joinString)
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

            return SqlUtils.ToTopSqlString(tableName, columns, whereString, orderByString, totalNum);
        }

        public static string GetSelectSqlStringByQueryString(string connectionString, string queryString, int totalNum, string orderByString)
        {
            if (totalNum == 0 && string.IsNullOrEmpty(orderByString))
            {
                return queryString;
            }
            string sqlString;
            if (totalNum > 0)
            {
                sqlString = SqlUtils.ToTopSqlString(queryString, orderByString, totalNum);

                //sqlString = WebConfigUtils.DatabaseType == DatabaseType.MySql ? $"SELECT * FROM ({queryString}) AS tmp {orderByString} LIMIT {totalNum}" : $"SELECT TOP {totalNum} * FROM ({queryString}) tmp {orderByString}";
            }
            else
            {
                sqlString = string.IsNullOrEmpty(orderByString) ? queryString : $"SELECT * FROM ({queryString}) {orderByString}";
            }
            return sqlString;
        }

        public static string GetSelectSqlStringByQueryString(string connectionString, string queryString, int startNum, int totalNum, string orderByString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = AppSettings.ConnectionString;
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
                return GetSelectSqlStringByQueryString(connectionString, queryString, totalNum, orderByString);
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
                    return GetSelectSqlStringByQueryString(connectionString, queryString, totalNum, orderByString);
                }
            }

            var orderByStringOpposite = GetOrderByStringOpposite(orderByString);

            var retval = string.Empty;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({queryString}) tmp {orderByString} LIMIT {topNum}
    ) AS tmp {orderByStringOpposite} LIMIT {totalNum}
) AS tmp {orderByString}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $@"
SELECT *
FROM (SELECT TOP {totalNum} *
        FROM (SELECT TOP {topNum} *
                FROM ({queryString}) tmp {orderByString}) tmp
        {orderByStringOpposite}) tmp
{orderByString}
";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({queryString}) tmp {orderByString} LIMIT {topNum}
    ) AS tmp {orderByStringOpposite} LIMIT {totalNum}
) AS tmp {orderByString}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $@"
SELECT *
FROM (SELECT TOP {totalNum} *
        FROM (SELECT TOP {topNum} *
                FROM ({queryString}) tmp {orderByString}) tmp
        {orderByStringOpposite}) tmp
{orderByString}
";
            }

            return retval;
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

        private static string GetOrderByStringOpposite(string orderByString)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(orderByString))
            {
                retval = orderByString.Replace(" DESC", " DESC_OPPOSITE").Replace(" ASC", " DESC").Replace(" DESC_OPPOSITE", " ASC");
            }
            return retval;
        }

        public static int GetCount(string tableName)
        {
            return GetIntResult($"select count(*) from {tableName}");
        }

        public static IEnumerable<dynamic> GetObjects(string tableName)
        {
            IEnumerable<dynamic> objects;
            var sqlString = $"select * from {tableName}";

            using (var connection = new Connection(AppSettings.DatabaseType, AppSettings.ConnectionString))
            {
                objects = connection.Query(sqlString, null, null, false).ToList();
            }

            return objects;
        }

        public static string AddIdentityColumnIdIfNotExists(string tableName, List<TableColumn> columns)
        {
            var identityColumnName = string.Empty;
            foreach (var column in columns)
            {
                if (column.IsIdentity || StringUtils.EqualsIgnoreCase(column.AttributeName, "id"))
                {
                    identityColumnName = column.AttributeName;
                    break;
                }
            }

            if (string.IsNullOrEmpty(identityColumnName))
            {
                identityColumnName = "Id";
                var sqlString =
                    SqlUtils.GetAddColumnsSqlString(tableName, $"{identityColumnName} {SqlUtils.GetAutoIncrementDataType(true)}");
                Execute(sqlString);

                columns.Insert(0, new TableColumn
                {
                    AttributeName = identityColumnName,
                    DataType = DataType.Integer,
                    DataLength = 0,
                    IsPrimaryKey = false,
                    IsIdentity = true
                });
            }

            return identityColumnName;
        }

        public static IEnumerable<dynamic> GetPageObjects(string tableName, string identityColumnName, int offset, int limit)
        {
            IEnumerable<dynamic> objects;
            var sqlString = GetPageSqlString(tableName, "*", string.Empty, $"ORDER BY {identityColumnName} ASC", offset, limit);

            using (var connection = new Connection(AppSettings.DatabaseType, AppSettings.ConnectionString))
            {
                objects = connection.Query(sqlString, null, null, false).ToList();
            }

            return objects;
        }

        public static void InsertMultiple(string tableName, IEnumerable<JObject> items, List<TableColumn> tableColumns)
        {
            var columnNames = new StringBuilder();
            foreach (var tableColumn in tableColumns)
            {
                columnNames.Append($"{tableColumn.AttributeName},");
            }
            if (columnNames.Length > 0) columnNames.Length -= 1;

            var valuesList = new List<string>();
            var parameters = new Dictionary<string, object>();
            var index = 0;
            foreach (var item in items)
            {
                var dict = TranslateUtils.JsonGetDictionaryIgnorecase(item);

                index++;
                var values = new StringBuilder();
                foreach (var tableColumn in tableColumns)
                {
                    if (string.IsNullOrEmpty(tableColumn?.AttributeName)) continue;

                    object val;
                    dict.TryGetValue(tableColumn.AttributeName, out val);

                    if (tableColumn.DataType == DataType.Integer)
                    {
                        if (val == null) val = 0;
                        values.Append($"{Convert.ToInt32(val)},");
                    }
                    else if (tableColumn.DataType == DataType.Decimal)
                    {
                        if (val == null) val = 0;
                        values.Append($"{Convert.ToDecimal(val)},");
                    }
                    else if (tableColumn.DataType == DataType.Boolean)
                    {
                        var paramName = $"@{tableColumn.AttributeName}_{index}";
                        if (val == null) val = false;
                        values.Append($"{paramName},");

                        parameters[paramName] = Convert.ToBoolean(val);
                    }
                    else if (tableColumn.DataType == DataType.DateTime)
                    {
                        if (val == null) val = DateTime.Now;
                        values.Append($"{SqlUtils.GetComparableDateTime(Convert.ToDateTime(val))},");
                    }
                    else
                    {
                        var paramName = $"@{tableColumn.AttributeName}_{index}";
                        values.Append($"{paramName},");

                        parameters[paramName] = Convert.ToString(val);
                    }
                }

                if (values.Length > 0)
                {
                    values.Length -= 1;
                    valuesList.Add(values.ToString());

                    if (parameters.Count > 1000)
                    {
                        InsertRows(tableName, columnNames.ToString(), valuesList, parameters);
                        valuesList.Clear();
                        parameters.Clear();
                    }
                }
            }

            if (valuesList.Count > 0 && parameters.Count > 0)
            {
                InsertRows(tableName, columnNames.ToString(), valuesList, parameters);
            }
        }

        private static void InsertRows(string tableName, string columnNames, List<string> valuesList, Dictionary<string, object> parameters)
        {
            if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                var sqlStringBuilder = new StringBuilder($@"INSERT INTO {tableName} ({columnNames}) VALUES ");
                foreach (var values in valuesList)
                {
                    sqlStringBuilder.Append($"({values}), ");
                }
                sqlStringBuilder.Length -= 2;

                var sqlString = sqlStringBuilder.ToString();

                var isIdentityColumn = !StringUtils.EqualsIgnoreCase(tableName, DataProvider.SiteDao.TableName);
                if (isIdentityColumn)
                {
                    sqlString = $@"
SET IDENTITY_INSERT {tableName} ON
{sqlString}
SET IDENTITY_INSERT {tableName} OFF
";
                }

                Execute(sqlString, parameters);
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                var sqlStringBuilder = new StringBuilder("INSERT ALL");
                foreach (var values in valuesList)
                {
                    sqlStringBuilder.Append($@" INTO {tableName} ({columnNames}) VALUES ({values})");
                }

                sqlStringBuilder.Append(" SELECT 1 FROM DUAL");

                Execute(sqlStringBuilder.ToString(), parameters);
            }
            else
            {
                var sqlStringBuilder = new StringBuilder($@"INSERT INTO {tableName} ({columnNames}) VALUES ");
                foreach (var values in valuesList)
                {
                    sqlStringBuilder.Append($"({values}), ");
                }
                sqlStringBuilder.Length -= 2;

                Execute(sqlStringBuilder.ToString(), parameters);
            }
        }

        private static ETriState _sqlServerVersionState = ETriState.All;

        public static bool IsSqlServer2012
        {
            get
            {
                if (_sqlServerVersionState != ETriState.All) return _sqlServerVersionState == ETriState.True;

                if (AppSettings.DatabaseType != DatabaseType.SqlServer)
                {
                    _sqlServerVersionState = ETriState.False;
                }

                try
                {
                    var version =
                        TranslateUtils.ToDecimal(
                            GetString("select left(cast(serverproperty('productversion') as varchar), 4)"));
                    _sqlServerVersionState = version >= 11 ? ETriState.True : ETriState.False;
                }
                catch
                {
                    _sqlServerVersionState = ETriState.False;
                }

                return _sqlServerVersionState == ETriState.True;
            }
        }

        public static int GetPageTotalCount(string tableName, string whereSqlString)
        {
            return GetIntResult($@"SELECT COUNT(*) FROM {tableName} {whereSqlString}");
        }

        public static int GetPageTotalCount(string tableName, string whereSqlString, Dictionary<string, object> parameters)
        {
            int totalCount;

            using (var connection = new Connection(AppSettings.DatabaseType, AppSettings.ConnectionString))
            {
                totalCount = connection.QueryFirstOrDefault<int>($@"SELECT COUNT(*) FROM {tableName} {whereSqlString}", parameters);
            }

            return totalCount;
        }

        public static string GetStlPageSqlString(string sqlString, string orderString, int totalCount, int itemsPerPage, int currentPageIndex)
        {
            var retval = string.Empty;

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

            var recsToRetrieve = itemsPerPage;
            if (currentPageIndex == pageCount - 1)
                recsToRetrieve = recordsInLastPage;

            orderString = orderString.ToUpper();
            var orderStringReverse = orderString.Replace(" DESC", " DESC2");
            orderStringReverse = orderStringReverse.Replace(" ASC", " DESC");
            orderStringReverse = orderStringReverse.Replace(" DESC2", " ASC");

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
) AS t2 {orderString}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $@"
SELECT * FROM (
    SELECT TOP {recsToRetrieve} * FROM (
        SELECT TOP {itemsPerPage * (currentPageIndex + 1)} * FROM ({sqlString}) AS t0 {orderString}
    ) AS t1 {orderStringReverse}
) AS t2 {orderString}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
) AS t2 {orderString}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) WHERE ROWNUM <= {itemsPerPage * (currentPageIndex + 1)} {orderString}
    ) WHERE ROWNUM <= {recsToRetrieve} {orderStringReverse}
) {orderString}";
            }

            return retval;
        }

        public static string GetPageSqlString(string tableName, string columnNames, string whereSqlString, string orderSqlString, int offset, int limit)
        {
            var retval = string.Empty;

            if (string.IsNullOrEmpty(orderSqlString))
            {
                orderSqlString = "ORDER BY Id DESC";
            }

            if (offset == 0 && limit == 0)
            {
                return $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString}";
            }

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                if (limit == 0)
                {
                    limit = int.MaxValue;
                }
                retval = $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} LIMIT {limit} OFFSET {offset}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer && IsSqlServer2012)
            {
                retval = limit == 0
                    ? $"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS"
                    : $"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer && !IsSqlServer2012)
            {
                if (offset == 0)
                {
                    retval = $"SELECT TOP {limit} {columnNames} FROM {tableName} {whereSqlString} {orderSqlString}";
                }
                else
                {
                    var rowWhere = limit == 0
                        ? $@"WHERE [row_num] > {offset}"
                        : $@"WHERE [row_num] BETWEEN {offset + 1} AND {offset + limit}";

                    retval = $@"SELECT * FROM (
    SELECT {columnNames}, ROW_NUMBER() OVER ({orderSqlString}) AS [row_num] FROM [{tableName}] {whereSqlString}
) as T {rowWhere}";
                }
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = limit == 0
                    ? $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset}"
                    : $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} LIMIT {limit} OFFSET {offset}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = limit == 0
                    ? $"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS"
                    : $"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
            }

            return retval;
        }

        public static string GetString(IDataReader rdr, int i)
        {
            var value = rdr.IsDBNull(i) ? string.Empty : rdr.GetValue(i).ToString();
            if (!string.IsNullOrEmpty(value))
            {
                value = AttackUtils.UnFilterSql(value);
            }
            if (AppSettings.DatabaseType == DatabaseType.Oracle && value == SqlUtils.OracleEmptyValue)
            {
                value = string.Empty;
            }
            return value;
        }

        public static int GetInt(IDataReader rdr, int i)
        {
            return rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
        }

        public static DateTime GetDateTime(IDataReader rdr, int i)
        {
            return rdr.IsDBNull(i) ? DateTime.Now : rdr.GetDateTime(i);
        }
    }
}