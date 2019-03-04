using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class DatabaseDao : DataProviderBase
    {
        public virtual void DeleteDbLog()
        {
            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                ExecuteSql("PURGE MASTER LOGS BEFORE DATE_SUB( NOW( ), INTERVAL 3 DAY)");
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);

                const string sqlCheck = "SELECT SERVERPROPERTY('productversion')";
                var versions = ExecuteScalar(sqlCheck).ToString();

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
                    ExecuteNonQuery(sql);
                }
                else
                {
                    //2008+
                    string sql =
                        $@"ALTER DATABASE [{databaseName}] SET RECOVERY SIMPLE;DBCC shrinkfile ([{databaseName}_log], 1); ALTER DATABASE [{databaseName}] SET RECOVERY FULL; ";
                    ExecuteNonQuery(sql);
                }
            }
        }

        public void ExecuteSql(string sqlString)
        {
            if (string.IsNullOrEmpty(sqlString)) return;

            ExecuteNonQuery(sqlString);
        }

        public void ExecuteSql(List<string> sqlList)
        {
            if (sqlList == null || sqlList.Count <= 0) return;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var sql in sqlList)
                        {
                            ExecuteNonQuery(trans, sql);
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public void ExecuteSqlInFile(string pathToScriptFile, StringBuilder errorBuilder)
        {
            IDbConnection connection;

            if (false == File.Exists(pathToScriptFile))
            {
                throw new Exception("File " + pathToScriptFile + " does not exists");
            }
            using (Stream stream = File.OpenRead(pathToScriptFile))
            {
                var reader = new StreamReader(stream, Encoding.UTF8);

                connection = GetConnection();

                var command = SqlUtils.GetIDbCommand();

                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                string sqlString;
                while (null != (sqlString = SqlUtils.ReadNextSqlString(reader)))
                {
                    command.CommandText = sqlString;
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        errorBuilder.Append($@"
                    sql:{sqlString}
                    message:{ex.Message}
                    ");
                    }
                }

                reader.Close();
            }
            connection.Close();
        }

        public void ExecuteSqlInFile(string pathToScriptFile, string tableName, StringBuilder errorBuilder)
        {
            IDbConnection connection;

            if (false == File.Exists(pathToScriptFile))
            {
                throw new Exception("File " + pathToScriptFile + " does not exists");
            }
            using (Stream stream = File.OpenRead(pathToScriptFile))
            {
                var reader = new StreamReader(stream, Encoding.Default);

                connection = GetConnection();

                var command = SqlUtils.GetIDbCommand();

                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                string sqlString;
                while (null != (sqlString = SqlUtils.ReadNextSqlString(reader)))
                {
                    sqlString = string.Format(sqlString, tableName);
                    command.CommandText = sqlString;
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        errorBuilder.Append($@"
                    sql:{sqlString}
                    message:{ex.Message}
                    ");
                    }
                }

                reader.Close();
            }
            connection.Close();
        }

        public int GetIntResult(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var count = 0;

            using (var conn = GetConnection(connectionString))
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

            using (var conn = GetConnection())
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

        public int GetIntResult(string sqlString, IDataParameter[] parms)
        {
            var count = 0;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString, parms))
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

        public List<int> GetIntList(string sqlString)
        {
            var list = new List<int>();

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
                {
                    while (rdr.Read())
                    {
                        list.Add(GetInt(rdr, 0));
                    }
                    rdr.Close();
                }
            }
            return list;
        }

        public List<int> GetIntList(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var list = new List<int>();

            using (var conn = GetConnection(connectionString))
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
                {
                    while (rdr.Read())
                    {
                        list.Add(GetInt(rdr, 0));
                    }
                    rdr.Close();
                }
            }
            return list;
        }

        public string GetString(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var retval = string.Empty;

            using (var conn = GetConnection(connectionString))
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString))
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

        public string GetString(string sqlString)
        {
            var value = string.Empty;
            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    value = GetString(rdr, 0);
                }
                rdr.Close();
            }

            return value;
        }

        public List<string> GetStringList(string sqlString)
        {
            var list = new List<string>();

            using (var conn = GetConnection())
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

        public DateTime GetDateTime(string sqlString)
        {
            var datetime = DateTime.MinValue;
            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    datetime = GetDateTime(rdr, 0);
                }
                rdr.Close();
            }

            return datetime;
        }

        public DateTime GetDateTime(string sqlString, IDataParameter[] parms)
        {
            var datetime = DateTime.MinValue;
            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    datetime = GetDateTime(rdr, 0);
                }
                rdr.Close();
            }

            return datetime;
        }

        public DataSet GetDataSetByWhereString(string tableName, string whereString)
        {
            var sqlSelect = GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString);
            var dataset = ExecuteDataset(sqlSelect);
            return dataset;
        }

        public IDataReader GetDataReader(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            return string.IsNullOrEmpty(sqlString) ? null : ExecuteReader(connectionString, sqlString);
        }

        public IDataReader GetDataSource(string sqlString)
        {
            if (string.IsNullOrEmpty(sqlString)) return null;
            var enumerable = ExecuteReader(sqlString);
            return enumerable;
        }

        public IDataReader GetDataSource(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            if (string.IsNullOrEmpty(sqlString)) return null;
            var enumerable = ExecuteReader(connectionString, sqlString);
            return enumerable;
        }

        public DataTable GetDataTable(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            if (string.IsNullOrEmpty(sqlString)) return null;
            var dataset = ExecuteDataset(connectionString, sqlString);

            if (dataset == null || dataset.Tables.Count == 0) return null;

            return dataset.Tables[0];
        }

        public DataSet GetDataSet(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            if (string.IsNullOrEmpty(sqlString)) return null;
            return ExecuteDataset(connectionString, sqlString);
        }

        public DataSet GetDataSet(string sqlString)
        {
            if (string.IsNullOrEmpty(sqlString)) return null;
            return ExecuteDataset(sqlString);
        }

        public void ReadResultsToNameValueCollection(IDataReader rdr, NameValueCollection attributes)
        {
            for (var i = 0; i < rdr.FieldCount; i++)
            {
                var columnName = rdr.GetName(i);
                var value = Convert.ToString(rdr.GetValue(i));
                if (!string.IsNullOrEmpty(value))
                {
                    value = AttackUtils.UnFilterSql(value);
                }
                attributes.Set(columnName, value);
            }
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

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retval = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
) AS t2 {orderString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $@"
SELECT * FROM (
    SELECT TOP {recsToRetrieve} * FROM (
        SELECT TOP {itemsPerPage * (currentPageIndex + 1)} * FROM ({sqlString}) AS t0 {orderString}
    ) AS t1 {orderStringReverse}
) AS t2 {orderString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
) AS t2 {orderString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retval = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) WHERE ROWNUM <= {itemsPerPage * (currentPageIndex + 1)} {orderString}
    ) WHERE ROWNUM <= {recsToRetrieve} {orderStringReverse}
) {orderString}";
            }

            //            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            //            {
            //                return $@"
            //SELECT * FROM (
            //    SELECT * FROM (
            //        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
            //    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
            //) AS t2 {orderString}";
            //            }
            //            else
            //            {
            //                return $@"
            //SELECT * FROM (
            //    SELECT TOP {recsToRetrieve} * FROM (
            //        SELECT TOP {itemsPerPage * (currentPageIndex + 1)} * FROM ({sqlString}) AS t0 {orderString}
            //    ) AS t1 {orderStringReverse}
            //) AS t2 {orderString}";
            //            }

            return retval;
        }

        //public void Install(StringBuilder errorBuilder)
        //{
        //    var sqlPath = PathUtils.GetInstallSqlFilePath(WebConfigUtils.DatabaseType);
        //    DataProvider.DatabaseDao.ExecuteSqlInFile(sqlPath, errorBuilder);
        //    DataProvider.TableDao.CreateAllAuxiliaryTableIfNotExists();
        //}

        //public void Upgrade(DatabaseType databaseType, StringBuilder errorBuilder)
        //{
        //    var filePathUpgrade = PathUtils.GetUpgradeSqlFilePath(databaseType, false);
        //    var filePathUpgradeTable = PathUtils.GetUpgradeSqlFilePath(databaseType, true);

        //    DataProvider.DatabaseDao.ExecuteSqlInFile(filePathUpgrade, errorBuilder);

        //    if (FileUtils.IsFileExists(filePathUpgradeTable))
        //    {
        //        try
        //        {
        //            var tableList = DataProvider.TableDao.GetAuxiliaryTableListCreatedInDb();
        //            foreach (var table in tableList)
        //            {
        //                DataProvider.DatabaseDao.ExecuteSqlInFile(filePathUpgradeTable, table.TableName, errorBuilder);
        //            }
        //        }
        //        catch
        //        {
        //            // ignored
        //        }
        //    }

        //    DataProvider.TableDao.CreateAllAuxiliaryTableIfNotExists();
        //}

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

        public bool IsTableExists(string tableName)
        {
            bool exists;

            if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                tableName = tableName.ToUpper();
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.MySql || WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                tableName = tableName.ToLower();
            }

            try
            {
                // ANSI SQL way.  Works in PostgreSQL, MSSQL, MySQL.  
                if (WebConfigUtils.DatabaseType != DatabaseType.Oracle)
                {
                    exists = (int)ExecuteScalar($"select case when exists((select * from information_schema.tables where table_name = '{tableName}')) then 1 else 0 end") == 1;
                }
                else
                {
                    exists = GetIntResult($"SELECT COUNT(*) FROM ALL_OBJECTS WHERE OBJECT_TYPE = 'TABLE' AND OWNER = '{WebConfigUtils.ConnectionStringUserId.ToUpper()}' and OBJECT_NAME = '{tableName}'") == 1;
                }
            }
            catch
            {
                try
                {
                    // Other RDBMS.  Graceful degradation
                    exists = true;
                    ExecuteNonQuery($"select 1 from {tableName} where 1 = 0");
                }
                catch
                {
                    exists = false;
                }
            }

            return exists;
        }

        public void AlterPluginTable(string pluginId, string tableName, List<TableColumn> tableColumns)
        {
            var isAltered = false;
            var columnNameList = TableColumnManager.GetTableColumnNameList(tableName);
            foreach (var tableColumn in tableColumns)
            {
                if (StringUtils.ContainsIgnoreCase(columnNameList, tableColumn.AttributeName)) continue;

                var columnSqlString = SqlUtils.GetColumnSqlString(tableColumn);
                var sqlString = SqlUtils.GetAddColumnsSqlString(tableName, columnSqlString);

                try
                {
                    DataProvider.DatabaseDao.ExecuteSql(sqlString);
                    isAltered = true;
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLog(pluginId, ex, sqlString);
                }
            }

            if (isAltered)
            {
                TableColumnManager.ClearCache();
            }
        }

        public void CreatePluginTable(string pluginId, string tableName, List<TableColumn> tableColumns)
        {
            if (!tableColumns.Any(x => StringUtils.EqualsIgnoreCase(x.AttributeName, "Id")))
            {
                tableColumns.Insert(0, new TableColumn
                {
                    AttributeName = "Id",
                    DataType = DataType.Integer,
                    IsIdentity = true,
                    IsPrimaryKey = true
                });
            }

            if (!CreateTable(tableName, tableColumns, out var ex, out var sqlString))
            {
                LogUtils.AddErrorLog(pluginId, ex, sqlString);
            }

            //var sqlString = GetCreateTableSqlString(tableName, tableColumns);

            //try
            //{
            //    ExecuteNonQuery(sqlString);
            //    TableColumnManager.ClearCache();
            //}
            //catch (Exception ex)
            //{
            //    LogUtils.AddErrorLog(pluginId, ex, sqlString);
            //}


            //var sqlBuilder = new StringBuilder();

            //try
            //{
            //    sqlBuilder.Append($@"CREATE TABLE {tableName} (").AppendLine();

            //    sqlBuilder.Append($"Id {SqlUtils.GetAutoIncrementDataType()},").AppendLine();

            //    foreach (var tableColumn in tableColumns)
            //    {
            //        if (string.IsNullOrEmpty(tableColumn.AttributeName) ||
            //            StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, "Id")) continue;

            //        var columnSql = SqlUtils.GetColumnSqlString(tableColumn.DataType, tableColumn.AttributeName,
            //            tableColumn.DataLength);
            //        if (!string.IsNullOrEmpty(columnSql))
            //        {
            //            sqlBuilder.Append(columnSql).Append(",").AppendLine();
            //        }
            //    }

            //    sqlBuilder.Append(WebConfigUtils.DatabaseType == DatabaseType.MySql
            //        ? @"PRIMARY KEY (Id)"
            //        : $@"CONSTRAINT PK_{tableName} PRIMARY KEY (Id)").AppendLine();

            //    sqlBuilder.Append(WebConfigUtils.DatabaseType == DatabaseType.MySql
            //        ? ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4"
            //        : ")");

            //    ExecuteNonQuery(sqlBuilder.ToString());

            //    TableColumnManager.ClearCache();
            //}
            //catch (Exception ex)
            //{
            //    LogUtils.AddErrorLog(pluginId, ex, sqlBuilder.ToString());
            //}
        }

        public string GetCreateTableSqlString(string tableName, List<TableColumn> tableColumns)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@"CREATE TABLE {tableName} (").AppendLine();

            var primaryKeyColumns = new List<TableColumn>();
            TableColumn identityColumn = null;
            foreach (var tableColumn in tableColumns)
            {
                if (string.IsNullOrEmpty(tableColumn.AttributeName)) continue;

                if (tableColumn.IsIdentity)
                {
                    identityColumn = tableColumn;
                }

                if (tableColumn.IsPrimaryKey)
                {
                    primaryKeyColumns.Add(tableColumn);
                }

                var columnSql = SqlUtils.GetColumnSqlString(tableColumn);
                if (!string.IsNullOrEmpty(columnSql))
                {
                    sqlBuilder.Append(columnSql).Append(",");
                }
            }

            if (identityColumn != null)
            {
                var primarykeySql = SqlUtils.GetPrimaryKeySqlString(tableName, identityColumn.AttributeName);
                if (!string.IsNullOrEmpty(primarykeySql))
                {
                    sqlBuilder.Append(primarykeySql).Append(",");
                }
            }
            else if (primaryKeyColumns.Count > 0)
            {
                foreach (var tableColumn in primaryKeyColumns)
                {
                    var primarykeySql = SqlUtils.GetPrimaryKeySqlString(tableName, tableColumn.AttributeName);
                    if (!string.IsNullOrEmpty(primarykeySql))
                    {
                        sqlBuilder.Append(primarykeySql).Append(",");
                    }
                }
            }

            sqlBuilder.Length--;

            sqlBuilder.AppendLine().Append(WebConfigUtils.DatabaseType == DatabaseType.MySql
                ? ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4"
                : ")");

            return sqlBuilder.ToString();
        }

        public bool CreateTable(string tableName, List<TableColumn> tableColumns, out Exception ex, out string sqlString)
        {
            ex = null;
            sqlString = GetCreateTableSqlString(tableName, tableColumns);

            try
            {
                ExecuteNonQuery(sqlString);
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

        public void AlterSystemTable(string tableName, List<TableColumn> tableColumns, List<string> dropColumnNames = null)
        {
            var list = new List<string>();

            var columnNameList = TableColumnManager.GetTableColumnNameList(tableName);
            foreach (var tableColumn in tableColumns)
            {
                if (!StringUtils.ContainsIgnoreCase(columnNameList, tableColumn.AttributeName))
                {
                    list.Add(SqlUtils.GetAddColumnsSqlString(tableName, SqlUtils.GetColumnSqlString(tableColumn)));
                }
            }

            if (dropColumnNames != null)
            {
                foreach (var columnName in columnNameList)
                {
                    if (StringUtils.ContainsIgnoreCase(dropColumnNames, columnName))
                    {
                        list.Add(SqlUtils.GetDropColumnsSqlString(tableName, columnName));
                    }
                }
            }

            if (list.Count > 0)
            {
                foreach (var sqlString in list)
                {
                    try
                    {
                        DataProvider.DatabaseDao.ExecuteSql(sqlString);
                    }
                    catch (Exception ex)
                    {
                        LogUtils.AddErrorLog(ex, sqlString);
                    }
                }

                TableColumnManager.ClearCache();
            }
        }

        public void AlterOracleAutoIncresementIdToMaxValue(string tableName)
        {
            try
            {
                var sqlString =
                    $"ALTER TABLE {tableName} MODIFY Id GENERATED ALWAYS AS IDENTITY(START WITH LIMIT VALUE)";
                ExecuteNonQuery(sqlString);
            }
            catch
            {
                // ignored
            }
        }

        public List<string> GetDatabaseNameList(DatabaseType databaseType, string connectionStringWithoutDatabaseName)
        {
            if (string.IsNullOrEmpty(connectionStringWithoutDatabaseName))
            {
                connectionStringWithoutDatabaseName = ConnectionString;
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
            var retval = false;
            try
            {
                var connection = GetConnection(databaseType, connectionString);
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    retval = true;
                    connection.Close();
                }
            }
            catch
            {
                // ignored
            }

            return retval;
        }

        public string GetSqlServerDefaultConstraintName(string tableName, string columnName)
        {
            var defaultConstraintName = string.Empty;
            string sqlString =
                $"select b.name from syscolumns a,sysobjects b where a.id=object_id('{tableName}') and b.id=a.cdefault and a.name='{columnName}' and b.name like 'DF%'";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    defaultConstraintName = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return defaultConstraintName;
        }

        public List<TableColumn> GetTableColumnInfoList(string connectionString, string tableName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(WebConfigUtils.DatabaseType, connectionString);

            List<TableColumn> list = null;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                list = GetMySqlColumns(connectionString, databaseName, tableName);
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                list = GetSqlServerColumns(connectionString, databaseName, tableName);
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                list = GetPostgreSqlColumns(connectionString, databaseName, tableName);
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                list = GetOracleColumns(connectionString, tableName);
            }

            return list;
        }

        public string GetOracleSequence(string tableName, string idColumnName)
        {
            var cacheKey = $"BaiRong.Core.Provider.{nameof(DatabaseDao)}.{nameof(GetOracleSequence)}.{tableName}.{idColumnName}";
            var sequence = CacheUtils.Get<string>(cacheKey);
            if (string.IsNullOrEmpty(sequence))
            {
                using (var conn = new OracleConnection(ConnectionString))
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

        private List<TableColumn> GetOracleColumns(string connectionString, string tableName)
        {
            var owner = WebConfigUtils.GetConnectionStringUserId(connectionString).ToUpper();
            tableName = tableName.ToUpper();

            var list = new List<TableColumn>();
            var sqlString =
                $"SELECT COLUMN_NAME, DATA_TYPE, DATA_PRECISION, DATA_SCALE, CHAR_LENGTH, DATA_DEFAULT FROM all_tab_cols WHERE OWNER = '{owner}' and table_name = '{tableName}' and user_generated = 'YES' ORDER BY COLUMN_ID";
            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var columnName = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0);
                    var dataType = SqlUtils.ToDataType(rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1));
                    var percision = rdr.IsDBNull(2) ? 0 : rdr.GetInt32(2);
                    var scale = rdr.IsDBNull(3) ? 0 : rdr.GetInt32(3);
                    var charLength = rdr.IsDBNull(4) ? 0 : rdr.GetInt32(4);
                    var dataDefault = rdr.IsDBNull(5) ? string.Empty : rdr.GetString(5);
                    if (dataType == DataType.Integer)
                    {
                        if (scale == 2)
                        {
                            dataType = DataType.Decimal;
                        }
                        else if (percision == 1)
                        {
                            dataType = DataType.Boolean;
                        }
                    }
                    var isIdentity = dataDefault.Contains(".nextval");

                    var info = new TableColumn
                    {
                        AttributeName = columnName,
                        DataType = dataType,
                        DataLength = charLength,
                        IsPrimaryKey = false,
                        IsIdentity = isIdentity
                    };
                    list.Add(info);
                }
                rdr.Close();
            }

            sqlString =
                $@"select distinct cu.column_name from all_cons_columns cu inner join all_constraints au 
on cu.constraint_name = au.constraint_name
and au.constraint_type = 'P' and cu.OWNER = '{owner}' and cu.table_name = '{tableName}'";

            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var columnName = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0);

                    foreach (var tableColumnInfo in list)
                    {
                        if (columnName == tableColumnInfo.AttributeName)
                        {
                            tableColumnInfo.IsPrimaryKey = true;
                            break;
                        }
                    }
                }
                rdr.Close();
            }

            return list;
        }

        private List<TableColumn> GetPostgreSqlColumns(string connectionString, string databaseName, string tableName)
        {
            var list = new List<TableColumn>();
            var sqlString =
                $"SELECT COLUMN_NAME, UDT_NAME, CHARACTER_MAXIMUM_LENGTH, COLUMN_DEFAULT FROM information_schema.columns WHERE table_catalog = '{databaseName}' AND table_name = '{tableName.ToLower()}' ORDER BY ordinal_position";
            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var columnName = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0);
                    var dataType = SqlUtils.ToDataType(rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1));
                    var length = rdr.IsDBNull(2) ? 0 : rdr.GetInt32(2);
                    var columnDefault = rdr.IsDBNull(3) ? string.Empty : rdr.GetString(3);

                    var isIdentity = columnDefault.StartsWith("nextval(");

                    var info = new TableColumn
                    {
                        AttributeName = columnName,
                        DataType = dataType,
                        DataLength = length,
                        IsPrimaryKey = false,
                        IsIdentity = isIdentity
                    };
                    list.Add(info);
                }
                rdr.Close();
            }

            sqlString =
                $"select column_name, constraint_name from information_schema.key_column_usage where table_catalog = '{databaseName}' and table_name = '{tableName.ToLower()}';";
            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var columnName = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0);
                    var constraintName = rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1);

                    var isPrimary = constraintName.StartsWith("pk");

                    if (isPrimary)
                    {
                        foreach (var tableColumnInfo in list)
                        {
                            if (columnName == tableColumnInfo.AttributeName)
                            {
                                tableColumnInfo.IsPrimaryKey = true;
                                break;
                            }
                        }
                    }
                }
                rdr.Close();
            }

            return list;
        }

        private List<TableColumn> GetSqlServerColumns(string connectionString, string databaseName, string tableName)
        {
            var list = new List<TableColumn>();

            var isIdentityExist = false;
            var tableId = string.Empty;
            var sqlStringTableId =
                $"select id from [{databaseName}]..sysobjects where type = 'U' and category<>2 and name='{tableName}'";

            using (var rdr = ExecuteReader(connectionString, sqlStringTableId))
            {
                if (rdr.Read())
                {
                    tableId = GetString(rdr, 0);
                }
                rdr.Close();
            }

            var sqlString =
                $"select C.name, T.name, C.length, C.colstat, case when C.autoval is null then 0 else 1 end, SC.text, (select CForgin.name from [{databaseName}]..sysreferences Sr,[{databaseName}]..sysobjects O,[{databaseName}]..syscolumns CForgin where Sr.fkeyid={tableId} and Sr.fkey1=C.colid and Sr.rkeyid=O.id and CForgin.id=O.id and CForgin.colid=Sr.rkey1), (select O.name from [{databaseName}]..sysreferences Sr,[{databaseName}]..sysobjects O,[{databaseName}]..syscolumns CForgin where Sr.fkeyid={tableId} and Sr.fkey1=C.colid and Sr.rkeyid=O.id and CForgin.id=O.id and CForgin.colid=Sr.rkey1), (select Sr.rkeyid from [{databaseName}]..sysreferences Sr,[{databaseName}]..sysobjects O,[{databaseName}]..syscolumns CForgin where Sr.fkeyid={tableId} and Sr.fkey1=C.colid and Sr.rkeyid=O.id and CForgin.id=O.id and CForgin.colid=Sr.rkey1) from [{databaseName}]..systypes T, [{databaseName}]..syscolumns C left join [{databaseName}]..syscomments SC on C.cdefault=SC.id where C.id={tableId} and C.xtype=T.xusertype order by C.colid";

            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var columnName = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0);
                    if (columnName == "msrepl_tran_version")
                    {
                        continue;
                    }

                    var dataTypeName = rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1);
                    var dataType = SqlUtils.ToDataType(dataTypeName);
                    var length = Convert.ToInt32(rdr.GetValue(2));
                    if (dataType == DataType.VarChar && dataTypeName == "nvarchar")
                    {
                        length = Convert.ToInt32(length / 2);
                    }
                    var isPrimaryKeyInt = Convert.ToInt32(rdr.GetValue(3));
                    var isIdentityInt = Convert.ToInt32(rdr.GetValue(4));

                    var isPrimaryKey = isPrimaryKeyInt == 1;
                    //var isIdentity = isIdentityInt == 1 || StringUtils.EqualsIgnoreCase(columnName, "Id");
                    var isIdentity = isIdentityInt == 1;
                    if (isIdentity)
                    {
                        isIdentityExist = true;
                    }

                    var info = new TableColumn
                    {
                        AttributeName = columnName,
                        DataType = dataType,
                        DataLength = length,
                        IsPrimaryKey = isPrimaryKey,
                        IsIdentity = isIdentity
                    };
                    list.Add(info);
                }
                rdr.Close();
            }

            if (!isIdentityExist)
            {
                var sqlIdentity = "select name from syscolumns where id = object_id(N'" + tableName +
                                  "') and COLUMNPROPERTY(id, name,'IsIdentity')= 1";
                var clName = "";
                using (var rdr = ExecuteReader(sqlIdentity))
                {
                    if (rdr.Read())
                    {
                        clName = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0);
                    }
                    rdr.Close();
                }

                foreach (var info in list)
                {
                    if (clName == info.AttributeName)
                    {
                        info.IsIdentity = true;
                    }
                }
            }

            return list;
        }

        private List<TableColumn> GetMySqlColumns(string connectionString, string databaseName, string tableName)
        {
            var list = new List<TableColumn>();

            string sqlString =
                $"select COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, COLUMN_KEY, EXTRA from information_schema.columns where table_schema = '{databaseName}' and table_name = '{tableName}' order by table_name,ordinal_position; ";
            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var columnName = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0);
                    var dataType = SqlUtils.ToDataType(rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1));
                    var length = rdr.IsDBNull(2) || dataType == DataType.Text ? 0 : Convert.ToInt32(rdr.GetValue(2));
                    var isPrimaryKey = Convert.ToString(rdr.GetValue(3)) == "PRI";
                    var isIdentity = Convert.ToString(rdr.GetValue(4)) == "auto_increment";

                    var info = new TableColumn
                    {
                        AttributeName = columnName,
                        DataType = dataType,
                        DataLength = length,
                        IsPrimaryKey = isPrimaryKey,
                        IsIdentity = isIdentity
                    };
                    list.Add(info);
                }
                rdr.Close();
            }

            return list;
        }

        public string GetSelectSqlString(string tableName, string columns, string whereString)
        {
            return GetSelectSqlString(tableName, 0, columns, whereString, null);
        }

        public string GetSelectSqlString(string tableName, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(tableName, 0, columns, whereString, orderByString);
        }

        public string GetSelectSqlString(string tableName, int totalNum, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(ConnectionString, tableName, totalNum, columns, whereString, orderByString);
        }

        public string GetSelectSqlString(string connectionString, string tableName, int totalNum, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(ConnectionString, tableName, totalNum, columns, whereString, orderByString, string.Empty);
        }

        public string GetSelectSqlString(string connectionString, string tableName, int totalNum, string columns, string whereString, string orderByString, string joinString)
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

//        public string GetSelectSqlString(string tableName, int startNum, int totalNum, string columns, string whereString, string orderByString)
//        {
//            return GetSelectSqlString(ConnectionString, tableName, startNum, totalNum, columns, whereString, orderByString);
//        }

//        public string GetSelectSqlString(string connectionString, string tableName, int startNum, int totalNum, string columns, string whereString, string orderByString)
//        {
//            if (string.IsNullOrEmpty(connectionString))
//            {
//                connectionString = ConnectionString;
//            }

//            if (startNum <= 1)
//            {
//                return GetSelectSqlString(connectionString, tableName, totalNum, columns, whereString, orderByString);
//            }

//            string countSqlString = $"SELECT Count(*) FROM {tableName} {whereString}";
//            var allCount = DataProvider.DatabaseDao.GetIntResult(connectionString, countSqlString);
//            if (totalNum == 0)
//            {
//                totalNum = allCount;
//            }

//            if (startNum > allCount) return string.Empty;

//            var topNum = startNum + totalNum - 1;

//            if (allCount < topNum)
//            {
//                totalNum = allCount - startNum + 1;
//                if (totalNum < 1)
//                {
//                    return GetSelectSqlString(connectionString, tableName, totalNum, columns, whereString, orderByString);
//                }
//            }

//            var orderByStringOpposite = GetOrderByStringOpposite(orderByString);

//            var retval = string.Empty;

//            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
//            {
//                retval = $@"
//SELECT {columns} FROM (
//    SELECT {columns} FROM (
//        SELECT {columns} FROM {tableName} {whereString} {orderByString} LIMIT {topNum}
//    ) AS tmp {orderByStringOpposite} LIMIT {totalNum}
//) AS tmp {orderByString}
//";
//            }
//            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
//            {
//                retval = $@"
//SELECT {columns}
//FROM (SELECT TOP {totalNum} {columns}
//        FROM (SELECT TOP {topNum} {columns}
//                FROM {tableName} {whereString} {orderByString}) tmp
//        {orderByStringOpposite}) tmp
//{orderByString}
//";
//            }
//            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
//            {
//                retval = $@"
//SELECT {columns} FROM (
//    SELECT {columns} FROM (
//        SELECT {columns} FROM {tableName} {whereString} {orderByString} LIMIT {topNum}
//    ) AS tmp {orderByStringOpposite} LIMIT {totalNum}
//) AS tmp {orderByString}
//";
//            }
//            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
//            {
//                retval = $@"
//SELECT {columns} FROM (
//    SELECT {columns} FROM (
//        SELECT {columns} FROM {tableName} {whereString} {orderByString} LIMIT {topNum}
//    ) AS tmp {orderByStringOpposite} LIMIT {totalNum}
//) AS tmp {orderByString}
//";
//            }

//            return retval;
//        }

        public string GetSelectSqlStringByQueryString(string connectionString, string queryString, int totalNum, string orderByString)
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


        public string GetSelectSqlStringByQueryString(string connectionString, string queryString, int startNum, int totalNum, string orderByString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
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
            var count = DataProvider.DatabaseDao.GetIntResult(connectionString, countSqlString);
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

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retval = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({queryString}) tmp {orderByString} LIMIT {topNum}
    ) AS tmp {orderByStringOpposite} LIMIT {totalNum}
) AS tmp {orderByString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
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
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({queryString}) tmp {orderByString} LIMIT {topNum}
    ) AS tmp {orderByStringOpposite} LIMIT {totalNum}
) AS tmp {orderByString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
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

        private string GetOrderByStringOpposite(string orderByString)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(orderByString))
            {
                retval = orderByString.Replace(" DESC", " DESC_OPPOSITE").Replace(" ASC", " DESC").Replace(" DESC_OPPOSITE", " ASC");
            }
            return retval;
        }

        public List<string> GetDropColumnsSqlString(string tableName, string attributeName)
        {
            var sqlList = new List<string>();

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                sqlList.Add($"ALTER TABLE [{tableName}] DROP COLUMN [{attributeName}]");
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                var defaultConstraintName = GetSqlServerDefaultConstraintName(tableName, attributeName);
                if (!string.IsNullOrEmpty(defaultConstraintName))
                {
                    sqlList.Add($"ALTER TABLE [{tableName}] DROP CONSTRAINT [{defaultConstraintName}]");
                }
                sqlList.Add($"ALTER TABLE [{tableName}] DROP COLUMN [{attributeName}]");
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                sqlList.Add($"ALTER TABLE [{tableName}] DROP COLUMN [{attributeName}]");
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                sqlList.Add($"ALTER TABLE {tableName} DROP COLUMN {attributeName}");
            }

            return sqlList;
        }

        public List<string> GetTableNameList()
        {
            var list = new List<string>();

            var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                var sqlString = $"SELECT table_name FROM information_schema.tables WHERE table_schema='{databaseName}' ORDER BY table_name";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var name = GetString(rdr, 0);
                        if (!string.IsNullOrEmpty(name))
                        {
                            list.Add(name);
                        }
                    }
                    rdr.Close();
                }
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                var sqlString =
                    $"SELECT name FROM [{databaseName}]..sysobjects WHERE type = 'U' AND category<>2 ORDER BY Name";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var name = GetString(rdr, 0);
                        if (!string.IsNullOrEmpty(name))
                        {
                            list.Add(name);
                        }
                    }
                    rdr.Close();
                }
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                var sqlString =
                    $"SELECT table_name FROM information_schema.tables WHERE table_catalog = '{databaseName}' AND table_type = 'BASE TABLE' AND table_schema NOT IN ('pg_catalog', 'information_schema')";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var name = GetString(rdr, 0);
                        if (!string.IsNullOrEmpty(name))
                        {
                            list.Add(name);
                        }
                    }
                    rdr.Close();
                }
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                const string sqlString = "select TABLE_NAME from user_tables";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var name = GetString(rdr, 0);
                        if (!string.IsNullOrEmpty(name))
                        {
                            list.Add(name);
                        }
                    }
                    rdr.Close();
                }
            }

            return list;
        }

        public int GetCount(string tableName)
        {
            return GetIntResult($"select count(*) from {tableName}");
        }

        public IEnumerable<dynamic> GetObjects(string tableName)
        {
            IEnumerable<dynamic> objects;
            var sqlString = $"select * from {tableName}";

            using (var connection = GetConnection())
            {
                connection.Open();

                objects = connection.Query(sqlString, null, null, false).ToList();
            }

            return objects;
        }

        public string AddIdentityColumnIdIfNotExists(string tableName, List<TableColumn> columns)
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
                DataProvider.DatabaseDao.ExecuteSql(sqlString);

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

        public IEnumerable<dynamic> GetPageObjects(string tableName, string identityColumnName, int offset, int limit)
        {
            IEnumerable<dynamic> objects;
            var sqlString = GetPageSqlString(tableName, "*", string.Empty, $"ORDER BY {identityColumnName} ASC", offset, limit);

            using (var connection = GetConnection())
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
                        parameterList.Add(GetParameter(paramName, tableColumn.DataType, Convert.ToBoolean(val)));
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
                        parameterList.Add(GetParameter(paramName, tableColumn.DataType, Convert.ToString(val)));
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

                var isIdentityColumn = !StringUtils.EqualsIgnoreCase(tableName, DataProvider.SiteDao.TableName);
                if (isIdentityColumn)
                {
                    sqlString = $@"
SET IDENTITY_INSERT {tableName} ON
{sqlString}
SET IDENTITY_INSERT {tableName} OFF
";
                }

                ExecuteNonQuery(sqlString, parameterList.ToArray());
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                var sqlStringBuilder = new StringBuilder("INSERT ALL");
                foreach (var values in valuesList)
                {
                    sqlStringBuilder.Append($@" INTO {tableName} ({columnNames}) VALUES ({values})");
                }

                sqlStringBuilder.Append(" SELECT 1 FROM DUAL");

                ExecuteNonQuery(sqlStringBuilder.ToString(), parameterList.ToArray());
            }
            else
            {
                var sqlStringBuilder = new StringBuilder($@"INSERT INTO {tableName} ({columnNames}) VALUES ");
                foreach (var values in valuesList)
                {
                    sqlStringBuilder.Append($"({values}), ");
                }
                sqlStringBuilder.Length -= 2;

                ExecuteNonQuery(sqlStringBuilder.ToString(), parameterList.ToArray());
            }
        }

        private ETriState _sqlServerVersionState = ETriState.All;

        public bool IsSqlServer2012
        {
            get
            {
                if (_sqlServerVersionState != ETriState.All) return _sqlServerVersionState == ETriState.True;

                if (WebConfigUtils.DatabaseType != DatabaseType.SqlServer)
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

        public int GetPageTotalCount(string tableName, string whereSqlString)
        {
            return GetIntResult($@"SELECT COUNT(*) FROM {tableName} {whereSqlString}");
        }

        public int GetPageTotalCount(string tableName, string whereSqlString, Dictionary<string, object> parameters)
        {
            int totalCount;

            using (var connection = GetConnection())
            {
                totalCount = connection.QueryFirstOrDefault<int>($@"SELECT COUNT(*) FROM {tableName} {whereSqlString}", parameters);
            }

            return totalCount;
        }

        public string GetPageSqlString(string tableName, string columnNames, string whereSqlString, string orderSqlString, int offset, int limit)
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

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retval = limit == 0
                    ? $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset}"
                    : $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} LIMIT {limit} OFFSET {offset}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer && IsSqlServer2012)
            {
                retval = limit == 0
                    ? $"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS"
                    : $"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer && !IsSqlServer2012)
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
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = limit == 0
                    ? $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset}"
                    : $@"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} LIMIT {limit} OFFSET {offset}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retval = limit == 0
                    ? $"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS"
                    : $"SELECT {columnNames} FROM {tableName} {whereSqlString} {orderSqlString} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
            }

            return retval;
        }
    }
}

