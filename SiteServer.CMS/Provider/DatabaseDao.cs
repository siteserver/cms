using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

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
                //检测数据库版本
                const string sqlCheck = "SELECT SERVERPROPERTY('productversion')";
                var versions = ExecuteScalar(sqlCheck).ToString();
                //MM.nn.bbbb.rr
                //8 -- 2000
                //9 -- 2005
                //10 -- 2008
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

        public DataSet GetDataSet(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            if (string.IsNullOrEmpty(sqlString)) return null;
            var dataset = ExecuteDataset(connectionString, sqlString);
            return dataset;
        }

        public DataSet GetDataSet(string sqlString)
        {
            if (string.IsNullOrEmpty(sqlString)) return null;
            var dataset = ExecuteDataset(sqlString);
            return dataset;
        }

        public void ReadResultsToNameValueCollection(IDataReader rdr, NameValueCollection attributes)
        {
            for (var i = 0; i < rdr.FieldCount; i++)
            {
                var columnName = rdr.GetName(i);
                var value = Convert.ToString(rdr.GetValue(i));
                if (!string.IsNullOrEmpty(value))
                {
                    value = PageUtils.UnFilterSql(value);
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
                    exists = GetIntResult($"select count(*) from all_objects where object_type = 'TABLE' and object_name = '{tableName}'") == 1;
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

        public void CreatePluginTable(string pluginId, string tableName, List<TableColumn> tableColumns)
        {
            var sqlBuilder = new StringBuilder();

            try
            {
                sqlBuilder.Append($@"CREATE TABLE {tableName} (").AppendLine();

                sqlBuilder.Append($"Id {SqlUtils.GetAutoIncrementDataType()},").AppendLine();

                foreach (var tableColumn in tableColumns)
                {
                    if (string.IsNullOrEmpty(tableColumn.AttributeName) ||
                        StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, "Id")) continue;

                    var columnSql = SqlUtils.GetColumnSqlString(tableColumn.DataType, tableColumn.AttributeName,
                        tableColumn.DataLength);
                    if (!string.IsNullOrEmpty(columnSql))
                    {
                        sqlBuilder.Append(columnSql).Append(",").AppendLine();
                    }
                }

                //添加主键及索引
                sqlBuilder.Append(WebConfigUtils.DatabaseType == DatabaseType.MySql
                    ? @"PRIMARY KEY (Id)"
                    : $@"CONSTRAINT PK_{tableName} PRIMARY KEY (Id)").AppendLine();

                sqlBuilder.Append(WebConfigUtils.DatabaseType == DatabaseType.MySql
                    ? ") ENGINE=InnoDB DEFAULT CHARSET=utf8"
                    : ")");

                ExecuteNonQuery(sqlBuilder.ToString());

                TableColumnManager.ClearCache();
            }
            catch (Exception ex)
            {
                LogUtils.AddPluginErrorLog(pluginId, ex, sqlBuilder.ToString());
            }
        }

        public void AlterPluginTable(string pluginId, string tableName, List<TableColumn> tableColumns)
        {
            var isAltered = false;
            var columnNameList = TableColumnManager.GetTableColumnNameListLowercase(tableName);
            foreach (var tableColumn in tableColumns)
            {
                if (columnNameList.Contains(tableColumn.AttributeName.ToLower())) continue;

                var columnSqlString = SqlUtils.GetColumnSqlString(tableColumn.DataType, tableColumn.AttributeName, tableColumn.DataLength);
                var sqlString = SqlUtils.GetAddColumnsSqlString(tableName, columnSqlString);

                try
                {
                    DataProvider.DatabaseDao.ExecuteSql(sqlString);
                    isAltered = true;
                }
                catch (Exception ex)
                {
                    LogUtils.AddPluginErrorLog(pluginId, ex, sqlString);
                }
            }

            if (isAltered)
            {
                TableColumnManager.ClearCache();
            }
        }

        public void CreateSystemTable(string tableName, List<TableColumnInfo> tableColumns)
        {
            try
            {
                var sqlBuilder = new StringBuilder();

                sqlBuilder.Append($@"CREATE TABLE {tableName} (").AppendLine();

                var primaryKeyColumns = new List<TableColumnInfo>();
                foreach (var tableColumn in tableColumns)
                {
                    if (string.IsNullOrEmpty(tableColumn.ColumnName)) continue;

                    if (tableColumn.IsIdentity)
                    {
                        primaryKeyColumns.Add(tableColumn);
                        sqlBuilder.Append($@"{tableColumn.ColumnName} {SqlUtils.GetAutoIncrementDataType()},").AppendLine();
                    }
                    else
                    {
                        if (tableColumn.IsPrimaryKey)
                        {
                            primaryKeyColumns.Add(tableColumn);
                        }

                        var columnSql = SqlUtils.GetColumnSqlString(tableColumn.DataType, tableColumn.ColumnName,
                        tableColumn.Length);
                        if (!string.IsNullOrEmpty(columnSql))
                        {
                            sqlBuilder.Append(columnSql).Append(",").AppendLine();
                        }
                    }
                }

                foreach (var tableColumn in primaryKeyColumns)
                {
                    sqlBuilder.Append(WebConfigUtils.DatabaseType == DatabaseType.MySql
                        ? $@"PRIMARY KEY ({tableColumn.ColumnName}),"
                        : $@"CONSTRAINT PK_{tableName}_{tableColumn.ColumnName} PRIMARY KEY ({tableColumn.ColumnName}),");
                }
                if (primaryKeyColumns.Count > 0)
                {
                    sqlBuilder.Length--;
                }

                sqlBuilder.AppendLine().Append(WebConfigUtils.DatabaseType == DatabaseType.MySql
                    ? ") ENGINE=InnoDB DEFAULT CHARSET=utf8"
                    : ")");

                ExecuteNonQuery(sqlBuilder.ToString());

                TableColumnManager.ClearCache();
            }
            catch (Exception ex)
            {
                LogUtils.AddSystemErrorLog(ex, tableName);
            }
        }

        public void AlterSystemTable(string tableName, List<TableColumnInfo> tableColumns)
        {
            var list = new List<string>();

            var columnNameList = TableColumnManager.GetTableColumnNameListLowercase(tableName);
            foreach (var tableColumn in tableColumns)
            {
                if (columnNameList.Contains(tableColumn.ColumnName.ToLower())) continue;

                list.Add(SqlUtils.GetAddColumnsSqlString(tableName, SqlUtils.GetColumnSqlString(tableColumn.DataType, tableColumn.ColumnName, tableColumn.Length)));
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
                        LogUtils.AddSystemErrorLog(ex, sqlString);
                    }
                }

                TableColumnManager.ClearCache();
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

        public Dictionary<string, int> GetTablesAndViewsDictionary(string connectionString, string databaseName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            string sqlString =
                $"select name, id from [{databaseName}].dbo.sysobjects where type in ('U','V') and category<>2 Order By Name";

            var dict = new Dictionary<string, int>();

            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var name = GetString(rdr, 0);
                    var id = GetInt(rdr, 1);
                    if (!string.IsNullOrEmpty(name))
                    {
                        dict[name] = id;
                    }
                }
                rdr.Close();
            }

            return dict;
        }

        //public string GetTableId(string connectionString, string databaseName, string tableName)
        //{
        //    if (WebConfigUtils.DatabaseType == DatabaseType.MySql) return tableName;

        //    if (string.IsNullOrEmpty(connectionString))
        //    {
        //        connectionString = ConnectionString;
        //    }

        //    var tableId = SqlUtils.Cache_GetTableIDCache(databaseName, tableName);

        //    if (string.IsNullOrEmpty(tableId))
        //    {
        //        string sqlString =
        //            $"select id from [{databaseName}].dbo.sysobjects where type in ('U','V') and category<>2 and name='{tableName}'";

        //        using (var rdr = ExecuteReader(connectionString, sqlString))
        //        {
        //            if (rdr.Read())
        //            {
        //                tableId = GetString(rdr, 0);
        //                SqlUtils.Cache_CacheTableID(databaseName, tableName, tableId);
        //            }
        //            rdr.Close();
        //        }
        //    }

        //    return tableId;
        //}

        //public string GetTableName(string databaseName, string tableId)
        //{
        //    if (WebConfigUtils.DatabaseType == DatabaseType.MySql) return tableId;

        //    var tableName = string.Empty;
        //    string cmd =
        //        $"select O.name from [{databaseName}].dbo.sysobjects O, [{databaseName}].dbo.sysusers U where O.id={tableId} and U.uid=O.uid";

        //    using (var rdr = ExecuteReader(cmd))
        //    {
        //        if (rdr.Read())
        //        {
        //            tableName = GetString(rdr, 0);
        //        }
        //        rdr.Close();
        //    }
        //    return tableName;
        //}

        //public string GetTableName(string connectionString, string databaseName, string tableId)
        //{
        //    if (WebConfigUtils.DatabaseType == DatabaseType.MySql) return tableId;

        //    if (string.IsNullOrEmpty(connectionString))
        //    {
        //        connectionString = ConnectionString;
        //    }

        //    var tableName = string.Empty;
        //    string sqlString =
        //        $"select O.name from [{databaseName}].dbo.sysobjects O, [{databaseName}].dbo.sysusers U where O.id={tableId} and U.uid=O.uid";

        //    using (var rdr = ExecuteReader(connectionString, sqlString))
        //    {
        //        if (rdr.Read())
        //        {
        //            tableName = GetString(rdr, 0);
        //        }
        //        rdr.Close();
        //    }
        //    return tableName;
        //}

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

        public List<TableColumnInfo> GetTableColumnInfoListLowercase(string connectionString, string tableName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(WebConfigUtils.DatabaseType, connectionString);

            List<TableColumnInfo> list = null;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                list = GetMySqlColumnsLowercase(connectionString, databaseName, tableName);
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                list = GetSqlServerColumnsLowercase(connectionString, databaseName, tableName);
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                list = GetPostgreSqlColumnsLowercase(connectionString, databaseName, tableName);
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                list = GetOracleColumnsLowercase(connectionString, tableName);
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
                        CommandText = $"SELECT DATA_DEFAULT FROM all_tab_cols WHERE table_name = '{tableName.ToUpper()}' AND column_name = '{idColumnName.ToUpper()}'",
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

        private List<TableColumnInfo> GetOracleColumnsLowercase(string connectionString, string tableName)
        {
            var list = new List<TableColumnInfo>();
            var sqlString =
                $"SELECT COLUMN_NAME, DATA_TYPE, DATA_PRECISION, DATA_SCALE, CHAR_LENGTH, DATA_DEFAULT FROM all_tab_cols WHERE table_name = '{tableName.ToUpper()}' ORDER BY COLUMN_ID";
            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var columnName = (rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0)).ToLower();
                    var dataType = SqlUtils.ToDataType(DatabaseType.Oracle, rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1));
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

                    var info = new TableColumnInfo(columnName, dataType, charLength, false, isIdentity);
                    list.Add(info);
                }
                rdr.Close();
            }

            sqlString =
                $@"select distinct cu.column_name from all_cons_columns cu inner join all_constraints au 
on cu.constraint_name = au.constraint_name
and au.constraint_type = 'P' and cu.table_name = '{tableName.ToUpper()}'";

            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var columnName = (rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0)).ToLower();

                    foreach (var tableColumnInfo in list)
                    {
                        if (columnName == tableColumnInfo.ColumnName)
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

        private List<TableColumnInfo> GetPostgreSqlColumnsLowercase(string connectionString, string databaseName, string tableName)
        {
            var list = new List<TableColumnInfo>();
            string sqlString =
                $"SELECT COLUMN_NAME, UDT_NAME, CHARACTER_MAXIMUM_LENGTH, COLUMN_DEFAULT FROM information_schema.columns WHERE table_catalog = '{databaseName}' AND table_name = '{tableName.ToLower()}' ORDER BY ordinal_position";
            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var columnName = (rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0)).ToLower();
                    var dataType = SqlUtils.ToDataType(DatabaseType.PostgreSql, rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1));
                    var length = rdr.IsDBNull(2) ? 0 : rdr.GetInt32(2);
                    var columnDefault = rdr.IsDBNull(3) ? string.Empty : rdr.GetString(3);

                    var isIdentity = columnDefault.StartsWith("nextval(");

                    var info = new TableColumnInfo(columnName, dataType, length, false, isIdentity);
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
                    var columnName = (rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0)).ToLower();
                    var constraintName = rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1);

                    var isPrimary = constraintName.StartsWith("pk");

                    if (isPrimary)
                    {
                        foreach (var tableColumnInfo in list)
                        {
                            if (columnName == tableColumnInfo.ColumnName)
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

        private List<TableColumnInfo> GetSqlServerColumnsLowercase(string connectionString, string databaseName, string tableName)
        {
            var list = new List<TableColumnInfo>();

            var isIdentityExist = false;
            var tableId = string.Empty;
            string sqlStringTableId =
                $"select id from [{databaseName}].dbo.sysobjects where type in ('U','V') and category<>2 and name='{tableName}'";

            using (var rdr = ExecuteReader(connectionString, sqlStringTableId))
            {
                if (rdr.Read())
                {
                    tableId = GetString(rdr, 0);
                }
                rdr.Close();
            }

            string sqlString =
                $"select C.name, T.name, C.length, C.colstat, case when C.autoval is null then 0 else 1 end, SC.text, (select CForgin.name from [{databaseName}].dbo.sysreferences Sr,[{databaseName}].dbo.sysobjects O,[{databaseName}].dbo.syscolumns CForgin where Sr.fkeyid={tableId} and Sr.fkey1=C.colid and Sr.rkeyid=O.id and CForgin.id=O.id and CForgin.colid=Sr.rkey1), (select O.name from [{databaseName}].dbo.sysreferences Sr,[{databaseName}].dbo.sysobjects O,[{databaseName}].dbo.syscolumns CForgin where Sr.fkeyid={tableId} and Sr.fkey1=C.colid and Sr.rkeyid=O.id and CForgin.id=O.id and CForgin.colid=Sr.rkey1), (select Sr.rkeyid from [{databaseName}].dbo.sysreferences Sr,[{databaseName}].dbo.sysobjects O,[{databaseName}].dbo.syscolumns CForgin where Sr.fkeyid={tableId} and Sr.fkey1=C.colid and Sr.rkeyid=O.id and CForgin.id=O.id and CForgin.colid=Sr.rkey1) from [{databaseName}].dbo.systypes T, [{databaseName}].dbo.syscolumns C left join [{databaseName}].dbo.syscomments SC on C.cdefault=SC.id where C.id={tableId} and C.xtype=T.xusertype order by C.colid";

            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var columnName = (rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0)).ToLower();
                    if (columnName == "msrepl_tran_version") //sqlserver 发布订阅字段，忽略
                    {
                        continue;
                    }
                    var dataType = SqlUtils.ToDataType(DatabaseType.SqlServer, rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1));
                    var length = Convert.ToInt32(rdr.GetValue(2));
                    if (dataType == DataType.VarChar)
                    {
                        length = Convert.ToInt32(length / 2);
                    }
                    var isPrimaryKeyInt = Convert.ToInt32(rdr.GetValue(3));
                    var isIdentityInt = Convert.ToInt32(rdr.GetValue(4));

                    var isPrimaryKey = isPrimaryKeyInt == 1;
                    var isIdentity = isIdentityInt == 1 || StringUtils.EqualsIgnoreCase(columnName, "Id");
                    //sqlserver 2005 返回isIdentity结果不正确,so 在此假设所有ID字段为Idenity字段
                    if (isIdentity)
                    {
                        isIdentityExist = true;
                    }

                    var info = new TableColumnInfo(columnName, dataType, length, isPrimaryKey, isIdentity);
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
                        clName = (rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0)).ToLower();
                    }
                    rdr.Close();
                }

                foreach (var info in list)
                {
                    if (clName == info.ColumnName)
                    {
                        info.IsIdentity = true;
                    }
                }
            }

            return list;
        }

        private List<TableColumnInfo> GetMySqlColumnsLowercase(string connectionString, string databaseName, string tableName)
        {
            var list = new List<TableColumnInfo>();

            string sqlString =
                $"select COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, COLUMN_KEY from information_schema.columns where table_schema = '{databaseName}' and table_name = '{tableName}' order by table_name,ordinal_position; ";
            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var columnName = (rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0)).ToLower();
                    var dataType = SqlUtils.ToDataType(DatabaseType.MySql, rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1));
                    var length = rdr.IsDBNull(2) || dataType == DataType.Text ? 0 : Convert.ToInt32(rdr.GetValue(2));
                    var isPrimaryKey = Convert.ToString(rdr.GetValue(3)) == "PRI";

                    var isIdentity = isPrimaryKey && StringUtils.EqualsIgnoreCase(columnName, "Id");

                    var info = new TableColumnInfo(columnName, dataType, length, isPrimaryKey, isIdentity);
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

        public string GetSelectSqlString(string tableName, int startNum, int totalNum, string columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(ConnectionString, tableName, startNum, totalNum, columns, whereString, orderByString);
        }

        public string GetSelectSqlString(string connectionString, string tableName, int startNum, int totalNum, string columns, string whereString, string orderByString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            if (startNum <= 1)
            {
                return GetSelectSqlString(connectionString, tableName, totalNum, columns, whereString, orderByString);
            }

            string countSqlString = $"SELECT Count(*) FROM {tableName} {whereString}";
            var allCount = DataProvider.DatabaseDao.GetIntResult(connectionString, countSqlString);
            if (totalNum == 0)
            {
                totalNum = allCount;
            }

            if (startNum > allCount) return string.Empty;

            var topNum = startNum + totalNum - 1;

            if (allCount < topNum)
            {
                totalNum = allCount - startNum + 1;
                if (totalNum < 1)
                {
                    return GetSelectSqlString(connectionString, tableName, totalNum, columns, whereString, orderByString);
                }
            }

            var orderByStringOpposite = GetOrderByStringOpposite(orderByString);

            var retval = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retval = $@"
SELECT {columns} FROM (
    SELECT {columns} FROM (
        SELECT {columns} FROM {tableName} {whereString} {orderByString} LIMIT {topNum}
    ) AS tmp {orderByStringOpposite} LIMIT {totalNum}
) AS tmp {orderByString}
";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $@"
SELECT {columns}
FROM (SELECT TOP {totalNum} {columns}
        FROM (SELECT TOP {topNum} {columns}
                FROM {tableName} {whereString} {orderByString}) tmp
        {orderByStringOpposite}) tmp
{orderByString}
";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $@"
SELECT {columns} FROM (
    SELECT {columns} FROM (
        SELECT {columns} FROM {tableName} {whereString} {orderByString} LIMIT {topNum}
    ) AS tmp {orderByStringOpposite} LIMIT {totalNum}
) AS tmp {orderByString}
";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retval = $@"
SELECT {columns} FROM (
    SELECT {columns} FROM (
        SELECT {columns} FROM {tableName} {whereString} {orderByString} LIMIT {topNum}
    ) AS tmp {orderByStringOpposite} LIMIT {totalNum}
) AS tmp {orderByString}
";
            }

            return retval;
        }

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
                sqlList.Add($"ALTER TABLE [{tableName}] DROP COLUMN [{attributeName}]");
            }

            return sqlList;
        }
    }
}

