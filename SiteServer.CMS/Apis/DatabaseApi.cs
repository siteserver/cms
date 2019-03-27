// ===============================================================================
// Microsoft Data Access Application Block for .NET 3.0
//
// AdoHelper.cs
//
// This file contains an abstract implementations of the AdoHelper class.
//
// For more information see the Documentation. 
// ===============================================================================
// Release history
// VERSION	DESCRIPTION
//   2.0	Added support for FillDataset, UpdateDataset and "Param" helper methods
//   3.0	New abstract class supporting the same methods using ADO.NET interfaces
//
// ===============================================================================
// Copyright (C) 2000-2001 Microsoft Corporation
// All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Database.Wrapper;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;
using SqlKata.Compilers;

namespace SiteServer.CMS.Apis
{
    /// <summary>
    /// The AdoHelper class is intended to encapsulate high performance, scalable best practices for 
    /// common data access uses.   It uses the Abstract Factory pattern to be easily extensible
    /// to any ADO.NET provider.  The current implementation provides helpers for SQL Server, ODBC,
    /// OLEDB, and Oracle.
    /// </summary>
    public abstract class DatabaseApi: IDatabaseApi
    {
        private static DatabaseApi _instance;

        public static DatabaseApi Instance
        {
            get
            {
                if (_instance != null) return _instance;

                if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
                {
                    _instance = new Database.Core.MySql();
                }
                else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
                {
                    _instance = new SqlServer();
                }
                else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
                {
                    _instance = new PostgreSql();
                }
                else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
                {
                    _instance = new Database.Core.Oracle();
                }

                return _instance;
            }
            set => _instance = value;
        }

        private Compiler _compiler;

        public Compiler SqlCompiler => _compiler ?? (_compiler = SqlDifferences.GetCompiler(WebConfigUtils.DatabaseType));

        public IDbConnection GetConnection(DatabaseType databaseType, string connectionString)
        {
            return SqlDifferences.GetIDbConnection(databaseType, connectionString);
        }

        public IDbConnection GetConnection()
        {
            return SqlDifferences.GetIDbConnection(DatabaseType, ConnectionString);
        }

        private DatabaseType DatabaseType => WebConfigUtils.DatabaseType;

        private string ConnectionString { get; } = WebConfigUtils.ConnectionString;

        #region DatabaseDao

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
                var versions = ExecuteScalar(ConnectionString, sqlCheck).ToString();

                var version = 8;
                var arr = versions.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length > 0)
                {
                    version = TranslateUtils.ToInt(arr[0], 8);
                }
                if (version < 10)
                {
                    //2000,2005
                    var sql = $"BACKUP LOG [{databaseName}] WITH NO_LOG";
                    ExecuteNonQuery(ConnectionString, sql);
                }
                else
                {
                    //2008+
                    var sql =
                        $@"ALTER DATABASE [{databaseName}] SET RECOVERY SIMPLE;DBCC shrinkfile ([{databaseName}_log], 1); ALTER DATABASE [{databaseName}] SET RECOVERY FULL; ";
                    ExecuteNonQuery(ConnectionString, sql);
                }
            }
        }

        public void ExecuteSql(string sqlString)
        {
            if (string.IsNullOrEmpty(sqlString)) return;

            ExecuteNonQuery(ConnectionString, sqlString);
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

        public int GetIntResult(string sqlString, IDataParameter[] parameters)
        {
            var count = 0;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, sqlString, parameters))
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


        public string GetString(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
            }

            var retVal = string.Empty;

            using (var conn = GetConnection(connectionString))
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

        public string GetString(string sqlString)
        {
            var value = string.Empty;
            using (var rdr = ExecuteReader(ConnectionString, sqlString))
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
            var enumerable = ExecuteReader(ConnectionString, sqlString);
            return enumerable;
        }

        public DataTable GetDataTable(string connectionString, string sqlString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConnectionString;
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
                connectionString = ConnectionString;
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
                    exists = (int)ExecuteScalar(ConnectionString, $"select case when exists((select * from information_schema.tables where table_name = '{tableName}')) then 1 else 0 end") == 1;
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
                    // Other DB.  Graceful degradation
                    exists = true;
                    ExecuteNonQuery(ConnectionString, $"select 1 from {tableName} where 1 = 0");
                }
                catch
                {
                    exists = false;
                }
            }

            return exists;
        }

        private IEnumerable<TableColumn> GetRealTableColumns(IEnumerable<TableColumn> tableColumns)
        {
            var realTableColumns = new List<TableColumn>();
            foreach (var tableColumn in tableColumns)
            {
                if (string.IsNullOrEmpty(tableColumn.AttributeName) || StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(DynamicEntity.Id)) || StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(DynamicEntity.Guid)) || StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(DynamicEntity.LastModifiedDate)))
                {
                    continue;
                }

                if (tableColumn.DataType == DataType.VarChar && tableColumn.DataLength == 0)
                {
                    tableColumn.DataLength = 2000;
                }
                realTableColumns.Add(tableColumn);
            }

            realTableColumns.InsertRange(0, new List<TableColumn>
            {
                new TableColumn
                {
                    AttributeName = nameof(DynamicEntity.Id),
                    DataType = DataType.Integer,
                    IsIdentity = true,
                    IsPrimaryKey = true
                },
                new TableColumn
                {
                    AttributeName = nameof(DynamicEntity.Guid),
                    DataType = DataType.VarChar,
                    DataLength = 50
                },
                new TableColumn
                {
                    AttributeName = nameof(DynamicEntity.LastModifiedDate),
                    DataType = DataType.DateTime
                }
            });

            return realTableColumns;
        }

        public bool CreateTable(string tableName, List<TableColumn> tableColumns, string pluginId, bool isContentTable, out Exception ex, out string sqlString)
        {
            ex = null;
            sqlString = string.Empty;

            var list = new List<string>();
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@"CREATE TABLE {tableName} (").AppendLine();

            var primaryKeyColumns = new List<TableColumn>();
            TableColumn identityColumn = null;
            foreach (var tableColumn in GetRealTableColumns(tableColumns))
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
                var primaryKeySql = SqlUtils.GetPrimaryKeySqlString(tableName, identityColumn.AttributeName);
                if (!string.IsNullOrEmpty(primaryKeySql))
                {
                    sqlBuilder.Append(primaryKeySql).Append(",");
                }
            }
            else if (primaryKeyColumns.Count > 0)
            {
                foreach (var tableColumn in primaryKeyColumns)
                {
                    var primaryKeySql = SqlUtils.GetPrimaryKeySqlString(tableName, tableColumn.AttributeName);
                    if (!string.IsNullOrEmpty(primaryKeySql))
                    {
                        sqlBuilder.Append(primaryKeySql).Append(",");
                    }
                }
            }

            sqlBuilder.Length--;

            sqlBuilder.AppendLine().Append(WebConfigUtils.DatabaseType == DatabaseType.MySql
                ? ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4"
                : ")");

            list.Add(sqlBuilder.ToString());
            if (isContentTable)
            {
                list.Add($@"CREATE INDEX {SqlUtils.GetQuotedIdentifier($"IX_{tableName}_General")} ON {SqlUtils.GetQuotedIdentifier(tableName)}({SqlUtils.GetQuotedIdentifier(ContentAttribute.IsTop)} DESC, {SqlUtils.GetQuotedIdentifier(ContentAttribute.Taxis)} DESC, {SqlUtils.GetQuotedIdentifier(ContentAttribute.Id)} DESC)");

                list.Add($@"CREATE INDEX {SqlUtils.GetQuotedIdentifier($"IX_{tableName}_Taxis")} ON {SqlUtils.GetQuotedIdentifier(tableName)}({SqlUtils.GetQuotedIdentifier(ContentAttribute.Taxis)} DESC)");
            }

            sqlString = TranslateUtils.ObjectCollectionToString(list, @"
GO
");

            if (list.Count <= 0) return false;

            foreach (var sql in list)
            {
                try
                {
                    ExecuteNonQuery(ConnectionString, sql);
                }
                catch (Exception e)
                {
                    ex = e;
                    LogUtils.AddErrorLog(pluginId, ex, sqlString);
                    return false;
                }
            }

            TableColumnManager.ClearCache();
            return true;
        }

        public void AlterTable(string tableName, List<TableColumn> tableColumns, string pluginId, List<string> dropColumnNames = null)
        {
            var list = new List<string>();

            var columnNameList = TableColumnManager.GetTableColumnNameList(tableName);
            foreach (var tableColumn in GetRealTableColumns(tableColumns))
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

            if (list.Count <= 0) return;

            var isAltered = false;
            foreach (var sqlString in list)
            {
                try
                {
                    ExecuteSql(sqlString);
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

        public void DropTable(string tableName)
        {
            var isAltered = false;
            try
            {
                ExecuteNonQuery(ConnectionString, $"DROP TABLE {tableName}");
                isAltered = true;
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
            }

            if (isAltered)
            {
                TableColumnManager.ClearCache();
            }
        }

        public void AlterOracleAutoIdToMaxValue(string tableName)
        {
            try
            {
                var sqlString =
                    $"ALTER TABLE {tableName} MODIFY Id GENERATED ALWAYS AS IDENTITY(START WITH LIMIT VALUE)";
                ExecuteNonQuery(ConnectionString, sqlString);
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
            var retVal = false;
            try
            {
                var connection = GetConnection(databaseType, connectionString);
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
            var cacheKey = $"SiteServer.CMS.Provider.{nameof(DatabaseApi)}.{nameof(GetOracleSequence)}.{tableName}.{idColumnName}";
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
                    var dataType = SqlUtils.ToOracleDataType(rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1));
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
                    var dataType = SqlUtils.ToPostgreSqlDataType(rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1));
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
                    var lengthName = rdr.GetValue(2).ToString();
                    var dataType = SqlUtils.ToSqlServerDataType(dataTypeName, lengthName);
                    var length = dataType == DataType.VarChar ? TranslateUtils.ToInt(lengthName) : 0;
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
                using (var rdr = ExecuteReader(ConnectionString, sqlIdentity))
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

            var sqlString =
                $"select COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, COLUMN_KEY, EXTRA from information_schema.columns where table_schema = '{databaseName}' and table_name = '{tableName}' order by table_name,ordinal_position; ";
            using (var rdr = ExecuteReader(connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var columnName = rdr.IsDBNull(0) ? string.Empty : rdr.GetString(0);
                    var dataType = SqlUtils.ToMySqlDataType(rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1));
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

        public string GetSelectSqlString(string tableName, string whereString)
        {
            return GetSelectSqlString(tableName, 0, new List<string> { "*" }, whereString, null);
        }

        public string GetSelectSqlString(string tableName, int totalNum, IList<string> columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(ConnectionString, tableName, totalNum, columns, whereString, orderByString);
        }

        public string GetSelectSqlString(string connectionString, string tableName, int totalNum, IList<string> columns, string whereString, string orderByString)
        {
            return GetSelectSqlString(ConnectionString, tableName, totalNum, columns, whereString, orderByString, string.Empty);
        }

        public string GetSelectSqlString(string connectionString, string tableName, int totalNum, IList<string> columns, string whereString, string orderByString, string joinString)
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

            return SqlDifferences.GetSqlString(tableName, columns, whereString, orderByString, totalNum);
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

        public List<string> GetTableNameList()
        {
            var list = new List<string>();

            var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                var sqlString = $"SELECT table_name FROM information_schema.tables WHERE table_schema='{databaseName}' ORDER BY table_name";

                using (var rdr = ExecuteReader(ConnectionString, sqlString))
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

                using (var rdr = ExecuteReader(ConnectionString, sqlString))
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

                using (var rdr = ExecuteReader(ConnectionString, sqlString))
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

                using (var rdr = ExecuteReader(ConnectionString, sqlString))
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
                ExecuteSql(sqlString);

                columns.Insert(0, new TableColumn
                {
                    AttributeName = identityColumnName,
                    DataType = DataType.Integer,
                    IsPrimaryKey = false,
                    IsIdentity = true
                });
            }

            return identityColumnName;
        }

        public IEnumerable<dynamic> GetPageObjects(string tableName, string identityColumnName, int offset, int limit)
        {
            IEnumerable<dynamic> objects;
            var sqlString = SqlDifferences.GetSqlString(tableName, orderSqlString: $"ORDER BY {identityColumnName} ASC", offset: offset, limit: limit);

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

        private ETriState _sqlServerVersionState = ETriState.All;

        public bool IsSqlServerGreaterEqual2012
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

        #endregion

        public virtual string GetString(IDataReader rdr, int i)
        {
            if (i < 0 || i >= rdr.FieldCount) return string.Empty;
            return rdr.IsDBNull(i) ? string.Empty : rdr.GetValue(i).ToString();
        }

        public virtual bool GetBoolean(IDataReader rdr, int i)
        {
            if (i < 0 || i >= rdr.FieldCount) return false;
            return !rdr.IsDBNull(i) && rdr.GetBoolean(i);
        }

        public virtual int GetInt(IDataReader rdr, int i)
        {
            if (i < 0 || i >= rdr.FieldCount) return 0;
            return rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
        }

        public virtual decimal GetDecimal(IDataReader rdr, int i)
        {
            if (i < 0 || i >= rdr.FieldCount) return 0;
            return rdr.IsDBNull(i) ? 0 : rdr.GetDecimal(i);
        }

        public virtual DateTime GetDateTime(IDataReader rdr, int i)
        {
            if (i < 0 || i >= rdr.FieldCount) return DateTime.MinValue;
            return rdr.IsDBNull(i) ? DateTime.MinValue : rdr.GetDateTime(i);
        }

        public virtual string GetString(IDataReader rdr, string name)
        {
            var i = rdr.GetOrdinal(name);
            return GetString(rdr, i);
        }

        public virtual bool GetBoolean(IDataReader rdr, string name)
        {
            var i = rdr.GetOrdinal(name);
            return GetBoolean(rdr, i);
        }

        public virtual int GetInt(IDataReader rdr, string name)
        {
            var i = rdr.GetOrdinal(name);
            return GetInt(rdr, i);
        }

        public virtual decimal GetDecimal(IDataReader rdr, string name)
        {
            var i = rdr.GetOrdinal(name);
            return GetDecimal(rdr, i);
        }

        public virtual DateTime GetDateTime(IDataReader rdr, string name)
        {
            var i = rdr.GetOrdinal(name);
            return GetDateTime(rdr, i);
        }

        /// <summary>
        /// This enum is used to indicate whether the connection was provided by the caller, or created by AdoHelper, so that
        /// we can set the appropriate CommandBehavior when calling ExecuteReader()
        /// </summary>
        protected enum AdoConnectionOwnership
        {
            /// <summary>Connection is owned and managed by ADOHelper</summary>
            Internal,

            /// <summary>Connection is owned and managed by the caller</summary>
            External
        }

        #region Declare members

        // necessary for handling the general case of needing event handlers for RowUpdating/ed events
        /// <summary>
        /// Internal handler used for bubbling up the event to the user
        /// </summary>
        protected RowUpdatingHandler MRowUpdating;

        /// <summary>
        /// Internal handler used for bubbling up the event to the user
        /// </summary>
        protected RowUpdatedHandler MRowUpdated;

        #endregion

        #region Provider specific abstract methods

        /// <summary>
        /// Returns an IDbConnection object for the given connection string
        /// </summary>
        /// <param name="connectionString">The connection string to be used to create the connection</param>
        /// <returns>An IDbConnection object</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionString is null</exception>
        public abstract IDbConnection GetConnection(string connectionString);

        /// <summary>
        /// Returns an IDbDataAdapter object
        /// </summary>
        /// <returns>The IDbDataAdapter</returns>
        public abstract IDbDataAdapter GetDataAdapter();

        /// <summary>
        /// Calls the CommandBuilder.DeriveParameters method for the specified provider, doing any setup and cleanup necessary
        /// </summary>
        /// <param name="cmd">The IDbCommand referencing the stored procedure from which the parameter information is to be derived. The derived parameters are added to the Parameters collection of the IDbCommand. </param>
        public abstract void DeriveParameters(IDbCommand cmd);

        /// <summary>
        /// Returns an IDataParameter object
        /// </summary>
        /// <returns>The IDataParameter object</returns>
        public abstract IDataParameter GetParameter();

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset) against the provided IDbConnection. 
        /// </summary>
        /// <example>
        /// <code>
        /// XmlReader r = helper.ExecuteXmlReader(command);
        /// </code></example>
        /// <param name="cmd">The IDbCommand to execute</param>
        /// <returns>An XmlReader containing the resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if command is null.</exception>
        public abstract XmlReader ExecuteXmlReader(IDbCommand cmd);

        /// <summary>
        /// Provider specific code to set up the updating/ed event handlers used by UpdateDataset
        /// </summary>
        /// <param name="dataAdapter">DataAdapter to attach the event handlers to</param>
        /// <param name="rowUpdatingHandler">The handler to be called when a row is updating</param>
        /// <param name="rowUpdatedHandler">The handler to be called when a row is updated</param>
        protected abstract void AddUpdateEventHandlers(IDbDataAdapter dataAdapter, RowUpdatingHandler rowUpdatingHandler,
            RowUpdatedHandler rowUpdatedHandler);

        /// <summary>
        /// Returns an array of IDataParameters of the specified size
        /// </summary>
        /// <param name="size">size of the array</param>
        /// <returns>The array of IDataParameters</returns>
        protected abstract IDataParameter[] GetDataParameters(int size);

        /// <summary>
        /// Handle any provider-specific issues with BLOBs here by "washing" the IDataParameter and returning a new one that is set up appropriately for the provider.
        /// </summary>
        /// <param name="connection">The IDbConnection to use in cleansing the parameter</param>
        /// <param name="p">The parameter before cleansing</param>
        /// <returns>The parameter after it's been cleansed.</returns>
        protected abstract IDataParameter GetBlobParameter(IDbConnection connection, IDataParameter p);

        #endregion

        #region Delegates

        // also used in our general case of RowUpdating/ed events
        /// <summary>
        /// Delegate for creating a RowUpdatingEvent handler
        /// </summary>
        /// <param name="obj">The object that published the event</param>
        /// <param name="e">The RowUpdatingEventArgs for the event</param>
        public delegate void RowUpdatingHandler(object obj, RowUpdatingEventArgs e);

        /// <summary>
        /// Delegate for creating a RowUpdatedEvent handler
        /// </summary>
        /// <param name="obj">The object that published the event</param>
        /// <param name="e">The RowUpdatedEventArgs for the event</param>
        public delegate void RowUpdatedHandler(object obj, RowUpdatedEventArgs e);

        #endregion

        #region Factory

        /// <summary>
        /// Create an AdoHelper for working with a specific provider (i.e. Sql, Odbc, OleDb, Oracle)
        /// </summary>
        /// <param name="providerAssembly">Assembly containing the specified helper subclass</param>
        /// <param name="providerType">Specific type of the provider</param>
        /// <returns>An AdoHelper instance of the specified type</returns>
        /// <example><code>
        /// AdoHelper helper = AdoHelper.CreateHelper("GotDotNet.ApplicationBlocks.Data", "GotDotNet.ApplicationBlocks.Data.OleDb");
        /// </code></example>
        public static DatabaseApi CreateHelper(string providerAssembly, string providerType)
        {
            var assembly = Assembly.Load(providerAssembly);
            var provider = assembly.CreateInstance(providerType);
            if (provider is DatabaseApi)
            {
                return provider as DatabaseApi;
            }
            else
            {
                throw new InvalidOperationException(
                    "The provider specified does not extend the AdoHelper abstract class.");
            }
        }

        #endregion

        #region GetParameter

        /// <summary>
        /// Get an IDataParameter for use in a SQL command
        /// </summary>
        /// <param name="name">The name of the parameter to create</param>
        /// <param name="value">The value of the specified parameter</param>
        /// <returns>An IDataParameter object</returns>
        public virtual IDataParameter GetParameter(string name, object value)
        {
            var parameter = GetParameter();
            parameter.ParameterName = name;
            if (value is DateTime && (DateTime) value < DateUtils.SqlMinValue)
            {
                value = DateUtils.SqlMinValue;
            }
            parameter.Value = value;

            return parameter;
        }

        /// <summary>
        /// Get an IDataParameter for use in a SQL command
        /// </summary>
        /// <param name="name">The name of the parameter to create</param>
        /// <param name="dbType">The System.Data.DbType of the parameter</param>
        /// <param name="size">The size of the parameter</param>
        /// <param name="direction">The System.Data.ParameterDirection of the parameter</param>
        /// <returns>An IDataParameter object</returns>
        public virtual IDataParameter GetParameter(string name, DbType dbType, int size, ParameterDirection direction)
        {
            var dataParameter = GetParameter();
            dataParameter.DbType = dbType;
            dataParameter.Direction = direction;
            dataParameter.ParameterName = name;

            if (size > 0 && dataParameter is IDbDataParameter)
            {
                var dbDataParameter = (IDbDataParameter) dataParameter;
                dbDataParameter.Size = size;
            }
            return dataParameter;
        }

        /// <summary>
        /// Get an IDataParameter for use in a SQL command
        /// </summary>
        /// <param name="name">The name of the parameter to create</param>
        /// <param name="dbType">The System.Data.DbType of the parameter</param>
        /// <param name="size">The size of the parameter</param>
        /// <param name="sourceColumn">The source column of the parameter</param>
        /// <param name="sourceVersion">The System.Data.DataRowVersion of the parameter</param>
        /// <returns>An IDataParameter object</returns>
        public virtual IDataParameter GetParameter(string name, DbType dbType, int size, string sourceColumn,
            DataRowVersion sourceVersion)
        {
            var dataParameter = GetParameter();
            dataParameter.DbType = dbType;
            dataParameter.ParameterName = name;
            dataParameter.SourceColumn = sourceColumn;
            dataParameter.SourceVersion = sourceVersion;

            if (size > 0 && dataParameter is IDbDataParameter)
            {
                var dbDataParameter = (IDbDataParameter) dataParameter;
                dbDataParameter.Size = size;
            }
            return dataParameter;
        }

        #endregion

        #region private utility methods

        /// <summary>
        /// This method is used to attach array of IDataParameters to an IDbCommand.
        /// 
        /// This method will assign a value of DbNull to any parameter with a direction of
        /// InputOutput and a value of null.  
        /// 
        /// This behavior will prevent default values from being used, but
        /// this will be the less common case than an intended pure output parameter (derived as InputOutput)
        /// where the user provided no input value.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">An array of IDataParameterParameters to be added to command</param>
        /// <exception cref="System.ArgumentNullException">Thrown if command is null.</exception>
        protected virtual void AttachParameters(IDbCommand command, IDataParameter[] commandParameters)
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
                        command.Parameters.Add(p.DbType == DbType.Binary ? GetBlobParameter(command.Connection, p) : p);
                    }
                }
            }
        }

        /// <summary>
        /// This method assigns dataRow column values to an IDataParameterCollection
        /// </summary>
        /// <param name="commandParameters">The IDataParameterCollection to be assigned values</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values</param>
        /// <exception cref="System.InvalidOperationException">Thrown if any of the parameter names are invalid.</exception>
        protected internal void AssignParameterValues(IDataParameterCollection commandParameters, DataRow dataRow)
        {
            if (commandParameters == null || dataRow == null)
            {
                // Do nothing if we get no data
                return;
            }

            var columns = dataRow.Table.Columns;

            var i = 0;
            // Set the parameters values
            foreach (IDataParameter commandParameter in commandParameters)
            {
                // Check the parameter name
                if (commandParameter.ParameterName == null ||
                    commandParameter.ParameterName.Length <= 1)
                    throw new InvalidOperationException(
                        $"Please provide a valid parameter name on the parameter #{i}, the ParameterName property has the following value: '{commandParameter.ParameterName}'.");

                if (columns.Contains(commandParameter.ParameterName))
                    commandParameter.Value = dataRow[commandParameter.ParameterName];
                else if (columns.Contains(commandParameter.ParameterName.Substring(1)))
                    commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];

                i++;
            }
        }

        /// <summary>
        /// This method assigns dataRow column values to an array of IDataParameters
        /// </summary>
        /// <param name="commandParameters">Array of IDataParameters to be assigned values</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values</param>
        /// <exception cref="System.InvalidOperationException">Thrown if any of the parameter names are invalid.</exception>
        protected void AssignParameterValues(IDataParameter[] commandParameters, DataRow dataRow)
        {
            if ((commandParameters == null) || (dataRow == null))
            {
                // Do nothing if we get no data
                return;
            }

            var columns = dataRow.Table.Columns;

            var i = 0;
            // Set the parameters values
            foreach (var commandParameter in commandParameters)
            {
                // Check the parameter name
                if (commandParameter.ParameterName == null ||
                    commandParameter.ParameterName.Length <= 1)
                    throw new InvalidOperationException(
                        $"Please provide a valid parameter name on the parameter #{i}, the ParameterName property has the following value: '{commandParameter.ParameterName}'.");

                if (columns.Contains(commandParameter.ParameterName))
                    commandParameter.Value = dataRow[commandParameter.ParameterName];
                else if (columns.Contains(commandParameter.ParameterName.Substring(1)))
                    commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];

                i++;
            }
        }

        /// <summary>
        /// This method assigns an array of values to an array of IDataParameters
        /// </summary>
        /// <param name="commandParameters">Array of IDataParameters to be assigned values</param>
        /// <param name="parameterValues">Array of objects holding the values to be assigned</param>
        /// <exception cref="System.ArgumentException">Thrown if an incorrect number of parameters are passed.</exception>
        protected void AssignParameterValues(IDataParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                // Do nothing if we get no data
                return;
            }

            // We must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            // Iterate through the IDataParameters, assigning the values from the corresponding position in the 
            // value array
            for (int i = 0, j = commandParameters.Length, k = 0; i < j; i++)
            {
                if (commandParameters[i].Direction != ParameterDirection.ReturnValue)
                {
                    // If the current array value derives from IDataParameter, then assign its Value property
                    if (parameterValues[k] is IDataParameter)
                    {
                        var paramInstance = (IDataParameter) parameterValues[k];
                        if (paramInstance.Direction == ParameterDirection.ReturnValue)
                        {
                            paramInstance = (IDataParameter) parameterValues[++k];
                        }
                        commandParameters[i].Value = paramInstance.Value ?? DBNull.Value;
                    }
                    else if (parameterValues[k] == null)
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i].Value = parameterValues[k];
                    }
                    k++;
                }
            }
        }

        /// <summary>
        /// This method cleans up the parameter syntax for the provider
        /// </summary>
        /// <param name="command">The IDbCommand containing the parameters to clean up.</param>
        public virtual void CleanParameterSyntax(IDbCommand command)
        {
            // do nothing by default
        }

        /// <summary>
        /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
        /// to the provided command
        /// </summary>
        /// <param name="command">The IDbCommand to be prepared</param>
        /// <param name="connection">A valid IDbConnection, on which to execute this command</param>
        /// <param name="transaction">A valid IDbTransaction, or 'null'</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">An array of IDataParameters to be associated with the command or 'null' if no parameters are required</param>
        /// <param name="mustCloseConnection"><c>true</c> if the connection was opened by the method, otherwose is false.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if command is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null.</exception>
        protected virtual void PrepareCommand(IDbCommand command, IDbConnection connection, IDbTransaction transaction,
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

        /// <summary>
        /// This method clears (if necessary) the connection, transaction, command type and parameters 
        /// from the provided command
        /// </summary>
        /// <remarks>
        /// Not implemented here because the behavior of this method differs on each data provider. 
        /// </remarks>
        /// <param name="command">The IDbCommand to be cleared</param>
        protected virtual void ClearCommand(IDbCommand command)
        {
            // do nothing by default
        }

        #endregion private utility methods

        #region ExecuteDataset

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <param name="command">The IDbCommand object to use</param>
        /// <returns>A DataSet containing the resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if command is null.</exception>
        public virtual DataSet ExecuteDataset(IDbCommand command)
        {
            var mustCloseConnection = false;

            // Clean Up Parameter Syntax
            CleanParameterSyntax(command);

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

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <example>
        /// <code>
        /// DataSet ds = helper.ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders");
        /// </code></example>
        /// <param name="connectionString">A valid connection string for an IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <returns>A DataSet containing the resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionString is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <returns>A DataSet containing the resultset generated by the command</returns>
        public virtual DataSet ExecuteDataset(string connectionString, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteDataset(connectionString, commandText, null);
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <example>
        /// <code>
        /// DataSet ds = helper.ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new IDbParameter("@prodid", 24));
        /// </code></example>
        /// <param name="connectionString">A valid connection string for an IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">An array of IDbParamters used to execute the command</param>
        /// <returns>A DataSet containing the resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionString is null</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if any of the IDataParameters.ParameterNames are null, or if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentException">Thrown if the parameter count does not match the number of values supplied</exception>
        public virtual DataSet ExecuteDataset(string connectionString, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            // Create & open an IDbConnection, and dispose of it after we are done
            using (var connection = GetConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteDataset(connection, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset and takes no parameters) against the provided IDbConnection. 
        /// </summary>
        /// <example>
        /// <code>
        /// DataSet ds = helper.ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders");
        /// </code></example>
        /// <param name="connection">A valid IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <returns>A DataSet containing the resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if connection is null</exception>
        public virtual DataSet ExecuteDataset(IDbConnection connection, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteDataset(connection, commandText, null);
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset) against the specified IDbConnection 
        /// using the provided parameters.
        /// </summary>
        /// <example>
        /// <code>
        /// DataSet ds = helper.ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new IDataParameter("@prodid", 24));
        /// </code></example>
        /// <param name="connection">A valid IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">An array of IDataParameters used to execute the command</param>
        /// <returns>A DataSet containing the resultset generated by the command</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if any of the IDataParameters.ParameterNames are null, or if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentException">Thrown if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if connection is null</exception>
        public virtual DataSet ExecuteDataset(IDbConnection connection, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            // Create a command and prepare it for execution
            var cmd = connection.CreateCommand();
            bool mustCloseConnection;
            PrepareCommand(cmd, connection, null, commandText, commandParameters, out mustCloseConnection);
            CleanParameterSyntax(cmd);

            var ds = ExecuteDataset(cmd);

            if (mustCloseConnection)
                connection.Close();

            // Return the DataSet
            return ds;
        }



        /// <summary>
        /// Execute an IDbCommand (that returns a resultset and takes no parameters) against the provided IDbTransaction. 
        /// </summary>
        /// <example><code>
        ///  DataSet ds = helper.ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders");
        /// </code></example>
        /// <param name="transaction">A valid IDbTransaction</param>
        /// <param name="commandText">SQL command</param>
        /// <returns>A DataSet containing the resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if transaction is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if transaction.Connection is null</exception>
        public virtual DataSet ExecuteDataset(IDbTransaction transaction, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteDataset(transaction, commandText, null);
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset) against the specified IDbTransaction
        /// using the provided parameters.
        /// </summary>
        /// <example>
        /// <code>
        /// DataSet ds = helper.ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new IDataParameter("@prodid", 24));
        /// </code></example>
        /// <param name="transaction">A valid IDbTransaction</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">An array of IDataParameters used to execute the command</param>
        /// <returns>A DataSet containing the resultset generated by the command</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if any of the IDataParameters.ParameterNames are null, or if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentException">Thrown if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if transaction is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if transaction.Connection is null</exception>
        public virtual DataSet ExecuteDataset(IDbTransaction transaction, string commandText,
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
            CleanParameterSyntax(cmd);

            return ExecuteDataset(cmd);

        }

        #endregion ExecuteDataset

        #region ExecuteNonQuery

        /// <summary>
        /// Execute an IDbCommand (that returns no resultset) against the database
        /// </summary>
        /// <param name="command">The IDbCommand to execute</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if command is null.</exception>
        public virtual int ExecuteNonQuery(IDbCommand command)
        {
            var mustCloseConnection = false;

            // Clean Up Parameter Syntax
            CleanParameterSyntax(command);

            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
                mustCloseConnection = true;
            }

            if (command == null) throw new ArgumentNullException(nameof(command));

            int returnVal = command.ExecuteNonQuery();

            if (mustCloseConnection)
            {
                command.Connection.Close();
            }

            return returnVal;
        }

        /// <summary>
        /// Execute an IDbCommand (that returns no resultset and takes no parameters) against the database specified in 
        /// the connection string
        /// </summary>
        /// <param name="connectionString">A valid connection string for an IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionString is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        public virtual int ExecuteNonQuery(string connectionString, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteNonQuery(connectionString, commandText, null);
        }

        /// <summary>
        /// Execute an IDbCommand (that returns no resultset) against the database specified in the connection string 
        /// using the provided parameters
        /// </summary>
        /// <param name="connectionString">A valid connection string for an IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">An array of IDataParameters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionString is null</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if any of the IDataParameters.ParameterNames are null, or if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentException">Thrown if the parameter count does not match the number of values supplied</exception>
        public virtual int ExecuteNonQuery(string connectionString, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            // Create & open an IDbConnection, and dispose of it after we are done
            using (var connection = GetConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteNonQuery(connection, commandText, commandParameters);
            }
        }

        public virtual int ExecuteNonQueryAndReturnId(string tableName, string idColumnName, string connectionString,
            string commandText,
            params IDataParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            int id;

            using (var conn = GetConnection(connectionString))
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

        public virtual int ExecuteNonQueryAndReturnId(string tableName, string idColumnName, IDbTransaction trans,
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

        public virtual int ExecuteCurrentId(string connectionString, string tableName, string idColumnName)
        {
            var id = 0;

            if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                var sequence = GetOracleSequence(tableName, idColumnName);

                if (!string.IsNullOrEmpty(sequence))
                {
                    using (var rdr = ExecuteReader(connectionString, $"SELECT {sequence}.currval from dual"))
                    {
                        if (rdr.Read() && !rdr.IsDBNull(0))
                        {
                            id = Convert.ToInt32(rdr[0]);
                        }
                        rdr.Close();
                    }
                }
            }
            else
            {
                using (var rdr = ExecuteReader(connectionString, $"SELECT @@IDENTITY AS '{idColumnName}'"))
                {
                    if (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        id = rdr.GetInt32(0);
                    }
                    rdr.Close();
                }
            }

            return id;
        }

        public virtual int ExecuteCurrentId(IDbConnection connection, string tableName, string idColumnName)
        {
            var id = 0;

            if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                var sequence = GetOracleSequence(tableName, idColumnName);

                if (!string.IsNullOrEmpty(sequence))
                {
                    using (var rdr = ExecuteReader(connection, $"SELECT {sequence}.currval from dual"))
                    {
                        if (rdr.Read() && !rdr.IsDBNull(0))
                        {
                            id = Convert.ToInt32(rdr[0]);
                        }
                        rdr.Close();
                    }
                }
            }
            else
            {
                using (var rdr = ExecuteReader(connection, $"SELECT @@IDENTITY AS '{idColumnName}'"))
                {
                    if (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        id = rdr.GetInt32(0);
                    }
                    rdr.Close();
                }
            }

            return id;
        }

        public virtual int ExecuteCurrentId(IDbTransaction trans, string tableName, string idColumnName)
        {
            var id = 0;

            if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
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
            else
            {
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

            return id;
        }

        /// <summary>
        /// Execute an IDbCommand (that returns no resultset and takes no parameters) against the provided IDbConnection. 
        /// </summary>
        /// <param name="connection">A valid IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if connection is null</exception>
        public virtual int ExecuteNonQuery(IDbConnection connection, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteNonQuery(connection, commandText, null);
        }

        /// <summary>
        /// Execute an IDbCommand (that returns no resultset) against the specified IDbConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="connection">A valid IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">An array of IDbParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if any of the IDataParameters.ParameterNames are null, or if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentException">Thrown if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if connection is null</exception>
        public virtual int ExecuteNonQuery(IDbConnection connection, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            // Create a command and prepare it for execution
            var cmd = connection.CreateCommand();
            bool mustCloseConnection;
            PrepareCommand(cmd, connection, null, commandText, commandParameters, out mustCloseConnection);
            CleanParameterSyntax(cmd);

            // Finally, execute the command
            var retval = ExecuteNonQuery(cmd);

            // Detach the IDataParameters from the command object, so they can be used again
            // don't do this...screws up output parameters -- cjbreisch
            // cmd.Parameters.Clear();
            if (mustCloseConnection)
                connection.Close();
            return retval;
        }

        /// <summary>
        /// Execute an IDbCommand (that returns no resultset and takes no parameters) against the provided IDbTransaction. 
        /// </summary>
        /// <param name="transaction">A valid IDbTransaction</param>
        /// <param name="commandText">SQL command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if transaction is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if transaction.Connection is null</exception>
        public virtual int ExecuteNonQuery(IDbTransaction transaction, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteNonQuery(transaction, commandText, null);
        }

        /// <summary>
        /// Execute an IDbCommand (that returns no resultset) against the specified IDbTransaction
        /// using the provided parameters.
        /// </summary>
        /// <param name="transaction">A valid IDbTransaction</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">An array of IDataParameters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if any of the IDataParameters.ParameterNames are null, or if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentException">Thrown if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if transaction is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if transaction.Connection is null</exception>
        public virtual int ExecuteNonQuery(IDbTransaction transaction, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException(
                    "The transaction was rolled back or commited, please provide an open transaction.",
                    nameof(transaction));

            // Create a command and prepare it for execution
            var cmd = transaction.Connection.CreateCommand();
            bool mustCloseConnection;
            PrepareCommand(cmd, transaction.Connection, transaction, commandText, commandParameters,
                out mustCloseConnection);
            CleanParameterSyntax(cmd);

            // Finally, execute the command
            var retval = ExecuteNonQuery(cmd);

            // Detach the IDataParameters from the command object, so they can be used again
            // don't do this...screws up output parameters -- cjbreisch
            // cmd.Parameters.Clear();
            return retval;
        }

        #endregion ExecuteNonQuery

        #region ExecuteReader

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <param name="command">The IDbCommand object to use</param>
        /// <returns>A IDataReader containing the resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if command is null.</exception>
        public virtual IDataReader ExecuteReader(IDbCommand command)
        {
            return ExecuteReader(command, AdoConnectionOwnership.External);
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <param name="command">The IDbCommand object to use</param>
        /// <param name="connectionOwnership">Enum indicating whether the connection was created internally or externally.</param>
        /// <returns>A IDataReader containing the resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if command is null.</exception>
        protected virtual IDataReader ExecuteReader(IDbCommand command, AdoConnectionOwnership connectionOwnership)
        {
            // Clean Up Parameter Syntax
            CleanParameterSyntax(command);

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

        /// <summary>
        /// Create and prepare an IDbCommand, and call ExecuteReader with the appropriate CommandBehavior.
        /// </summary>
        /// <remarks>
        /// If we created and opened the connection, we want the connection to be closed when the DataReader is closed.
        /// 
        /// If the caller provided the connection, we want to leave it to them to manage.
        /// </remarks>
        /// <param name="connection">A valid IDbConnection, on which to execute this command</param>
        /// <param name="transaction">A valid IDbTransaction, or 'null'</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">An array of IDataParameters to be associated with the command or 'null' if no parameters are required</param>
        /// <param name="connectionOwnership">Indicates whether the connection parameter was provided by the caller, or created by AdoHelper</param>
        /// <returns>IDataReader containing the results of the command</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if any of the IDataParameters.ParameterNames are null, or if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentException">Thrown if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if connection is null</exception>
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
                CleanParameterSyntax(cmd);

                // override conenctionOwnership if we created the connection in PrepareCommand -- cjbreisch
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

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <param name="connectionString">A valid connection string for an IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <returns>A IDataReader containing the resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionString is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        public virtual IDataReader ExecuteReader(string connectionString, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteReader(connectionString, commandText, null);
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <param name="connectionString">A valid connection string for an IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">An array of IDataParameters used to execute the command</param>
        /// <returns>A IDataReader containing the resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionString is null</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if any of the IDataParameters.ParameterNames are null, or if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentException">Thrown if the parameter count does not match the number of values supplied</exception>
        public virtual IDataReader ExecuteReader(string connectionString, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            IDbConnection connection = null;
            try
            {
                connection = GetConnection(connectionString);
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

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset and takes no parameters) against the provided IDbConnection. 
        /// </summary>
        /// <example>
        /// <code>
        /// IDataReader dr = helper.ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders");
        /// </code></example>
        /// <param name="connection">A valid IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <returns>an IDataReader containing the resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        public virtual IDataReader ExecuteReader(IDbConnection connection, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteReader(connection, commandText, null);
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset) against the specified IDbConnection 
        /// using the provided parameters.
        /// </summary>
        /// <example>
        /// <code>
        /// IDataReader dr = helper.ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new IDataParameter("@prodid", 24));
        /// </code></example>
        /// <param name="connection">A valid IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">An array of IDataParameters used to execute the command</param>
        /// <returns>an IDataReader containing the resultset generated by the command</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if any of the IDataParameters.ParameterNames are null, or if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentException">Thrown if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if connection is null</exception>
        public virtual IDataReader ExecuteReader(IDbConnection connection, string commandText,
            params IDataParameter[] commandParameters)
        {
            // Pass through the call to the private overload using a null transaction value and an externally owned connection
            return ExecuteReader(connection, null, commandText, commandParameters, AdoConnectionOwnership.External);
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset and takes no parameters) against the provided IDbTransaction. 
        /// </summary>
        /// <example><code>
        ///  IDataReader dr = helper.ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders");
        /// </code></example>
        /// <param name="transaction">A valid IDbTransaction</param>
        /// <param name="commandText">SQL command</param>
        /// <returns>A IDataReader containing the resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        public virtual IDataReader ExecuteReader(IDbTransaction transaction, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteReader(transaction, commandText, null);
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset) against the specified IDbTransaction
        /// using the provided parameters.
        /// </summary>
        /// <example>
        /// <code>
        /// IDataReader dr = helper.ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new IDataParameter("@prodid", 24));
        /// </code></example>
        /// <param name="transaction">A valid IDbTransaction</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">An array of IDataParameters used to execute the command</param>
        /// <returns>A IDataReader containing the resultset generated by the command</returns>
        public virtual IDataReader ExecuteReader(IDbTransaction transaction, string commandText,
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

        #endregion ExecuteReader

        #region ExecuteScalar

        /// <summary>
        /// Execute an IDbCommand (that returns a 1x1 resultset) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <param name="command">The IDbCommand to execute</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if command is null.</exception>
        public virtual object ExecuteScalar(IDbCommand command)
        {
            var mustCloseConnection = false;

            // Clean Up Parameter Syntax
            CleanParameterSyntax(command);

            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
                mustCloseConnection = true;
            }

            // Execute the command & return the results
            var retval = command.ExecuteScalar();

            // Detach the IDataParameters from the command object, so they can be used again
            // don't do this...screws up output params -- cjbreisch
            // command.Parameters.Clear();

            if (mustCloseConnection)
            {
                command.Connection.Close();
            }

            return retval;
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a 1x1 resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <example>
        /// <code>
        /// int orderCount = (int)helper.ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount");
        /// </code></example>
        /// <param name="connectionString">A valid connection string for an IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionString is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        public virtual object ExecuteScalar(string connectionString, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteScalar(connectionString, commandText, null);
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a 1x1 resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <param name="connectionString">A valid connection string for an IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">An array of IDataParameters used to execute the command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionString is null</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if any of the IDataParameters.ParameterNames are null, or if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentException">Thrown if the parameter count does not match the number of values supplied</exception>
        public virtual object ExecuteScalar(string connectionString, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            // Create & open an IDbConnection, and dispose of it after we are done
            IDbConnection connection = null;
            try
            {
                connection = GetConnection(connectionString);
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteScalar(connection, commandText, commandParameters);
            }
            finally
            {
                var id = connection as IDisposable;
                if (id != null) id.Dispose();
            }
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a 1x1 resultset and takes no parameters) against the provided IDbConnection. 
        /// </summary>
        /// <example>
        /// <code>
        /// int orderCount = (int)helper.ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount");
        /// </code></example>
        /// <param name="connection">A valid IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        public virtual object ExecuteScalar(IDbConnection connection, string commandText)
        {
            // Pass through the call providing null for the set of IDbParameters
            return ExecuteScalar(connection, commandText, null);
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a 1x1 resultset) against the specified IDbConnection 
        /// using the provided parameters.
        /// </summary>
        /// <param name="connection">A valid IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">An array of IDataParameters used to execute the command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if any of the IDataParameters.ParameterNames are null, or if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentException">Thrown if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if connection is null</exception>
        public virtual object ExecuteScalar(IDbConnection connection, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            // Create a command and prepare it for execution
            var cmd = connection.CreateCommand();

            bool mustCloseConnection;
            PrepareCommand(cmd, connection, null, commandText, commandParameters, out mustCloseConnection);
            CleanParameterSyntax(cmd);

            // Execute the command & return the results
            var retval = ExecuteScalar(cmd);

            // Detach the IDataParameters from the command object, so they can be used again
            // don't do this...screws up output parameters -- cjbreisch
            // cmd.Parameters.Clear();

            if (mustCloseConnection)
                connection.Close();

            return retval;
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a 1x1 resultset and takes no parameters) against the provided IDbTransaction. 
        /// </summary>
        /// <example>
        /// <code>
        /// int orderCount = (int)helper.ExecuteScalar(tran, CommandType.StoredProcedure, "GetOrderCount");
        /// </code></example>
        /// <param name="transaction">A valid IDbTransaction</param>
        /// <param name="commandText">SQL command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        public virtual object ExecuteScalar(IDbTransaction transaction, string commandText)
        {
            // Pass through the call providing null for the set of IDataParameters
            return ExecuteScalar(transaction, commandText, null);
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a 1x1 resultset) against the specified IDbTransaction
        /// using the provided parameters.
        /// </summary>
        /// <param name="transaction">A valid IDbTransaction</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">An array of IDbParamters used to execute the command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if any of the IDataParameters.ParameterNames are null, or if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentException">Thrown if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if transaction is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if transaction.Connection is null</exception>
        public virtual object ExecuteScalar(IDbTransaction transaction, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException(
                    "The transaction was rolled back or commited, please provide an open transaction.",
                    nameof(transaction));

            // Create a command and prepare it for execution
            var cmd = transaction.Connection.CreateCommand();
            bool mustCloseConnection;
            PrepareCommand(cmd, transaction.Connection, transaction, commandText, commandParameters,
                out mustCloseConnection);
            CleanParameterSyntax(cmd);

            // Execute the command & return the results
            var retval = ExecuteScalar(cmd);

            // Detach the IDataParameters from the command object, so they can be used again
            // don't do this...screws up output parameters -- cjbreisch
            // cmd.Parameters.Clear();
            return retval;
        }

        #endregion ExecuteScalar	

        #region ExecuteInt

        public int ExecuteInt(string connectionString, string commandText)
        {
            var count = 0;

            using (var conn = GetConnection(connectionString))
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, commandText))
                {
                    if (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        count = rdr.GetInt32(0);
                    }
                    rdr.Close();
                }
            }
            return count;
        }

        public int ExecuteInt(string connectionString, string commandText, IDataParameter[] commandParameters)
        {
            var count = 0;

            using (var conn = GetConnection(connectionString))
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, commandText, commandParameters))
                {
                    if (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        count = rdr.GetInt32(0);
                    }
                    rdr.Close();
                }
            }
            return count;
        }

        #endregion ExecuteInt

        #region FillDataset

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <param name="command">The IDbCommand to execute</param>
        /// <param name="dataSet">A DataSet wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)</param>
        /// <exception cref="System.ArgumentNullException">Thrown if command is null.</exception>
        public virtual void FillDataset(IDbCommand command, DataSet dataSet, string[] tableNames)
        {
            var mustCloseConnection = false;

            // Clean Up Parameter Syntax
            CleanParameterSyntax(command);

            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
                mustCloseConnection = true;
            }

            // Create the DataAdapter & DataSet
            IDbDataAdapter dataAdapter = null;
            try
            {
                dataAdapter = GetDataAdapter();
                dataAdapter.SelectCommand = command;

                // Add the table mappings specified by the user
                if (tableNames != null && tableNames.Length > 0)
                {
                    var tableName = "Table";
                    for (var index = 0; index < tableNames.Length; index++)
                    {
                        if (tableNames[index] == null || tableNames[index].Length == 0)
                            throw new ArgumentException(
                                "The tableNames parameter must contain a list of tables, a value was provided as null or empty string.",
                                nameof(tableNames));
                        dataAdapter.TableMappings.Add(
                            tableName + (index == 0 ? "" : index.ToString()),
                            tableNames[index]);
                    }
                }

                // Fill the DataSet using default values for DataTable names, etc
                dataAdapter.Fill(dataSet);

                if (mustCloseConnection)
                {
                    command.Connection.Close();
                }

                // Detach the IDataParameters from the command object, so they can be used again
                // don't do this...screws up output params  --cjb
                // command.Parameters.Clear();
            }
            finally
            {
                var id = dataAdapter as IDisposable;
                if (id != null) id.Dispose();
            }

        }

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <example>
        /// <code>
        /// helper.FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});
        /// </code></example>
        /// <param name="connectionString">A valid connection string for an IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="dataSet">A DataSet wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)</param>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionString is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        public virtual void FillDataset(string connectionString, string commandText, DataSet dataSet,
            string[] tableNames)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            if (dataSet == null) throw new ArgumentNullException(nameof(dataSet));

            // Create & open an IDbConnection, and dispose of it after we are done
            IDbConnection connection = null;
            try
            {
                connection = GetConnection(connectionString);
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                FillDataset(connection, commandText, dataSet, tableNames);
            }
            finally
            {
                var id = connection as IDisposable;
                if (id != null) id.Dispose();
            }
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <param name="connectionString">A valid connection string for an IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">An array of IDataParameters used to execute the command</param>
        /// <param name="dataSet">A DataSet wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionString is null</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if any of the IDataParameters.ParameterNames are null, or if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentException">Thrown if the parameter count does not match the number of values supplied</exception>
        public virtual void FillDataset(string connectionString,
            string commandText, DataSet dataSet, string[] tableNames,
            params IDataParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            if (dataSet == null) throw new ArgumentNullException(nameof(dataSet));
            // Create & open an IDbConnection, and dispose of it after we are done
            IDbConnection connection = null;
            try
            {
                connection = GetConnection(connectionString);
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                FillDataset(connection, commandText, dataSet, tableNames, commandParameters);
            }
            finally
            {
                var id = connection as IDisposable;
                if (id != null) id.Dispose();
            }
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset and takes no parameters) against the provided IDbConnection. 
        /// </summary>
        /// <example>
        /// <code>
        /// helper.FillDataset(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});
        /// </code></example>
        /// <param name="connection">A valid IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>    
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if connection is null</exception>
        public virtual void FillDataset(IDbConnection connection,
            string commandText, DataSet dataSet, string[] tableNames)
        {
            FillDataset(connection, commandText, dataSet, tableNames, null);
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset) against the specified IDbConnection 
        /// using the provided parameters.
        /// </summary>
        /// <param name="connection">A valid IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="dataSet">A DataSet wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>
        /// <param name="commandParameters">An array of IDataParameters used to execute the command</param>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if connection is null</exception>
        public virtual void FillDataset(IDbConnection connection,
            string commandText, DataSet dataSet, string[] tableNames,
            params IDataParameter[] commandParameters)
        {
            FillDataset(connection, null, commandText, dataSet, tableNames, commandParameters);
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset and takes no parameters) against the provided IDbTransaction. 
        /// </summary>
        /// <example>
        /// <code>
        /// helper.FillDataset(tran, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});
        /// </code></example>
        /// <param name="transaction">A valid IDbTransaction</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>    
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if transaction is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if transaction.Connection is null</exception>
        public virtual void FillDataset(IDbTransaction transaction,
            string commandText,
            DataSet dataSet, string[] tableNames)
        {
            FillDataset(transaction, commandText, dataSet, tableNames, null);
        }

        /// <summary>
        /// Execute an IDbCommand (that returns a resultset) against the specified IDbTransaction
        /// using the provided parameters.
        /// </summary>
        /// <param name="transaction">A valid IDbTransaction</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="dataSet">A DataSet wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>
        /// <param name="commandParameters">An array of IDataParameters used to execute the command</param>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if transaction is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if transaction.Connection is null</exception>
        public virtual void FillDataset(IDbTransaction transaction,
            string commandText, DataSet dataSet, string[] tableNames,
            params IDataParameter[] commandParameters)
        {
            FillDataset(transaction.Connection, transaction, commandText, dataSet, tableNames, commandParameters);
        }

        /// <summary>
        /// Private helper method that execute an IDbCommand (that returns a resultset) against the specified IDbTransaction and IDbConnection
        /// using the provided parameters.
        /// </summary>
        /// <param name="connection">A valid IDbConnection</param>
        /// <param name="transaction">A valid IDbTransaction</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="dataSet">A DataSet wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>
        /// <param name="commandParameters">An array of IDataParameters used to execute the command</param>
        private void FillDataset(IDbConnection connection, IDbTransaction transaction,
            string commandText, DataSet dataSet, string[] tableNames,
            params IDataParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (dataSet == null) throw new ArgumentNullException(nameof(dataSet));

            // Create a command and prepare it for execution
            var command = connection.CreateCommand();
            bool mustCloseConnection;
            PrepareCommand(command, connection, transaction, commandText, commandParameters, out mustCloseConnection);
            CleanParameterSyntax(command);

            FillDataset(command, dataSet, tableNames);

            if (mustCloseConnection)
                connection.Close();
        }

        #endregion

        #region UpdateDataset

        /// <summary>
        /// This method consumes the RowUpdatingEvent and passes it on to the consumer specifed in the call to UpdateDataset
        /// </summary>
        /// <param name="obj">The object that generated the event</param>
        /// <param name="e">The System.Data.Common.RowUpdatingEventArgs</param>
        protected void RowUpdating(object obj, RowUpdatingEventArgs e)
        {
            if (MRowUpdating != null)
                MRowUpdating(obj, e);
        }

        /// <summary>
        /// This method consumes the RowUpdatedEvent and passes it on to the consumer specifed in the call to UpdateDataset
        /// </summary>
        /// <param name="obj">The object that generated the event</param>
        /// <param name="e">The System.Data.Common.RowUpdatingEventArgs</param>
        protected void RowUpdated(object obj, RowUpdatedEventArgs e)
        {
            if (MRowUpdated != null)
                MRowUpdated(obj, e);
        }

        /// <summary>
        /// Set up a command for updating a DataSet.
        /// </summary>
        /// <param name="command">command object to prepare</param>
        /// <param name="mustCloseConnection">output parameter specifying whether the connection used should be closed by the DAAB</param>
        /// <returns>An IDbCommand object</returns>
        protected virtual IDbCommand SetCommand(IDbCommand command, out bool mustCloseConnection)
        {
            mustCloseConnection = false;
            if (command != null)
            {
                var commandParameters = new IDataParameter[command.Parameters.Count];
                command.Parameters.CopyTo(commandParameters, 0);
                command.Parameters.Clear();
                PrepareCommand(command, command.Connection, null, command.CommandText, commandParameters,
                    out mustCloseConnection);
                CleanParameterSyntax(command);
            }

            return command;
        }

        /// <summary>
        /// Executes the respective command for each inserted, updated, or deleted row in the DataSet.
        /// </summary>
        /// <example>
        /// <code>
        /// helper.UpdateDataset(conn, insertCommand, deleteCommand, updateCommand, dataSet, "Order");
        /// </code></example>
        /// <param name="insertCommand">A valid SQL statement or stored procedure to insert new records into the data source</param>
        /// <param name="deleteCommand">A valid SQL statement or stored procedure to delete records from the data source</param>
        /// <param name="updateCommand">A valid SQL statement or stored procedure used to update records in the data source</param>
        /// <param name="dataSet">The DataSet used to update the data source</param>
        /// <param name="tableName">The DataTable used to update the data source.</param>
        public virtual void UpdateDataset(IDbCommand insertCommand, IDbCommand deleteCommand, IDbCommand updateCommand,
            DataSet dataSet, string tableName)
        {
            UpdateDataset(insertCommand, deleteCommand, updateCommand, dataSet, tableName, null, null);
        }

        /// <summary> 
        /// Executes the IDbCommand for each inserted, updated, or deleted row in the DataSet also implementing RowUpdating and RowUpdated Event Handlers 
        /// </summary> 
        /// <example> 
        /// <code>
        /// RowUpdatingEventHandler rowUpdatingHandler = new RowUpdatingEventHandler( OnRowUpdating ); 
        /// RowUpdatedEventHandler rowUpdatedHandler = new RowUpdatedEventHandler( OnRowUpdated ); 
        /// helper.UpdateDataSet(sqlInsertCommand, sqlDeleteCommand, sqlUpdateCommand, dataSet, "Order", rowUpdatingHandler, rowUpdatedHandler); 
        /// </code></example> 
        /// <param name="insertCommand">A valid SQL statement or stored procedure to insert new records into the data source</param> 
        /// <param name="deleteCommand">A valid SQL statement or stored procedure to delete records from the data source</param> 
        /// <param name="updateCommand">A valid SQL statement or stored procedure used to update records in the data source</param> 
        /// <param name="dataSet">The DataSet used to update the data source</param> 
        /// <param name="tableName">The DataTable used to update the data source.</param> 
        /// <param name="rowUpdatingHandler">RowUpdatingEventHandler</param> 
        /// <param name="rowUpdatedHandler">RowUpdatedEventHandler</param> 
        public void UpdateDataset(IDbCommand insertCommand, IDbCommand deleteCommand, IDbCommand updateCommand,
            DataSet dataSet, string tableName, RowUpdatingHandler rowUpdatingHandler,
            RowUpdatedHandler rowUpdatedHandler)
        {
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException(nameof(tableName));

            // Create an IDbDataAdapter, and dispose of it after we are done
            IDbDataAdapter dataAdapter = null;
            try
            {
                bool mustCloseUpdateConnection;
                bool mustCloseInsertConnection;
                bool mustCloseDeleteConnection;

                dataAdapter = GetDataAdapter();

                // Set the data adapter commands
                dataAdapter.UpdateCommand = SetCommand(updateCommand, out mustCloseUpdateConnection);
                dataAdapter.InsertCommand = SetCommand(insertCommand, out mustCloseInsertConnection);
                dataAdapter.DeleteCommand = SetCommand(deleteCommand, out mustCloseDeleteConnection);

                AddUpdateEventHandlers(dataAdapter, rowUpdatingHandler, rowUpdatedHandler);

                if (dataAdapter is DbDataAdapter)
                {
                    // UpdateObject the DataSet changes in the data source
                    ((DbDataAdapter) dataAdapter).Update(dataSet, tableName);
                }
                else
                {
                    dataAdapter.TableMappings.Add(tableName, "Table");

                    // UpdateObject the DataSet changes in the data source
                    dataAdapter.Update(dataSet);
                }

                // Commit all the changes made to the DataSet
                dataSet.Tables[tableName].AcceptChanges();

                if (mustCloseUpdateConnection)
                {
                    updateCommand.Connection.Close();
                }
                if (mustCloseInsertConnection)
                {
                    insertCommand.Connection.Close();
                }
                if (mustCloseDeleteConnection)
                {
                    deleteCommand.Connection.Close();
                }
            }
            finally
            {
                var id = dataAdapter as IDisposable;
                id?.Dispose();
            }
        }

        #endregion

        #region CreateCommand

        /// <summary>
        /// Simplify the creation of an IDbCommand object by allowing
        /// a stored procedure and optional parameters to be provided
        /// </summary>
        /// <example>
        /// <code>
        /// IDbCommand command = helper.CreateCommand(conn, "AddCustomer", "CustomerID", "CustomerName");
        /// </code></example>
        /// <param name="connectionString">A valid connection string for an IDbConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="sourceColumns">An array of string to be assigned as the source columns of the stored procedure parameters</param>
        /// <returns>A valid IDbCommand object</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionString is null</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if any of the IDataParameters.ParameterNames are null, or if the parameter count does not match the number of values supplied</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if spName is null</exception>
        /// <exception cref="System.ArgumentException">Thrown if the parameter count does not match the number of values supplied</exception>
        public virtual IDbCommand CreateSpCommand(string connectionString, string spName, params string[] sourceColumns)
        {
            return CreateSpCommand(GetConnection(connectionString), spName, sourceColumns);
        }

        /// <summary>
        /// Simplify the creation of an IDbCommand object by allowing
        /// a stored procedure and optional parameters to be provided
        /// </summary>
        /// <example>
        /// <code>
        /// IDbCommand command = helper.CreateCommand(conn, "AddCustomer", "CustomerID", "CustomerName");
        /// </code></example>
        /// <param name="connection">A valid IDbConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="sourceColumns">An array of string to be assigned as the source columns of the stored procedure parameters</param>
        /// <returns>A valid IDbCommand object</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if spName is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if connection is null</exception>
        public virtual IDbCommand CreateSpCommand(IDbConnection connection, string spName, params string[] sourceColumns)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrEmpty(spName)) throw new ArgumentNullException(nameof(spName));

            // Create an IDbCommand
            var cmd = connection.CreateCommand();
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;

            // If we receive parameter values, we need to figure out where they go
            if ((sourceColumns != null) && (sourceColumns.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                var commandParameters = GetSpParameterSet(connection, spName);

                // Assign the provided source columns to these parameters based on parameter order
                for (var index = 0; index < sourceColumns.Length; index++)
                    if (commandParameters[index].SourceColumn == String.Empty)
                        commandParameters[index].SourceColumn = sourceColumns[index];

                // Attach the discovered parameters to the IDbCommand object
                AttachParameters(cmd, commandParameters);
            }

            return cmd;
        }

        /// <summary>
        /// Simplify the creation of an IDbCommand object by allowing
        /// a stored procedure and optional parameters to be provided
        /// </summary>
        /// <param name="connectionString">A valid connection string for an IDbConnection</param>
        /// <param name="commandText">A valid SQL statement</param>
        /// <param name="commandParameters">The parameters for the SQL statement</param>
        /// <returns>A valid IDbCommand object</returns>
        public virtual IDbCommand CreateCommand(string connectionString, string commandText,
            params IDataParameter[] commandParameters)
        {
            return CreateCommand(GetConnection(connectionString), commandText, commandParameters);
        }

        /// <summary>
        /// Simplify the creation of an IDbCommand object by allowing
        /// a stored procedure and optional parameters to be provided
        /// </summary>
        /// <example><code>
        /// IDbCommand command = helper.CreateCommand(conn, "AddCustomer", "CustomerID", "CustomerName");
        /// </code></example>
        /// <param name="connection">A valid IDbConnection object</param>
        /// <param name="commandText">A valid SQL statement</param>
        /// <param name="commandParameters">The parameters for the SQL statement</param>
        /// <returns>A valid IDbCommand object</returns>
        public virtual IDbCommand CreateCommand(IDbConnection connection, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException(nameof(commandText));

            // Create an IDbCommand
            var cmd = connection.CreateCommand();
            cmd.CommandText = commandText;
            cmd.CommandType = CommandType.Text;

            // If we receive parameter values, we need to figure out where they go
            if (commandParameters != null && commandParameters.Length > 0)
            {
                // Assign the provided source columns to these parameters based on parameter order
                for (var index = 0; index < commandParameters.Length; index++)
                    commandParameters[index].SourceColumn =
                        commandParameters[index].ParameterName.TrimStart(new char[] {'@'});

                // Attach the discovered parameters to the IDbCommand object
                AttachParameters(cmd, commandParameters);
            }

            return cmd;
        }

        #endregion

        #region ExecuteNonQueryTypedParams

        /// <summary>
        /// Execute a stored procedure via an IDbCommand (that returns no resultset) 
        /// against the database specified in the connection string using the 
        /// dataRow column values as the stored procedure's parameters values.
        /// This method will assign the parameter values based on row values.
        /// </summary>
        /// <param name="command">The IDbCommand to execute</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if command is null.</exception>
        public virtual int ExecuteNonQueryTypedParams(IDbCommand command, DataRow dataRow)
        {
            int retVal;

            // Clean Up Parameter Syntax
            CleanParameterSyntax(command);

            // If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Set the parameters values
                AssignParameterValues(command.Parameters, dataRow);

                retVal = ExecuteNonQuery(command);
            }
            else
            {
                retVal = ExecuteNonQuery(command);
            }

            return retVal;
        }

        #endregion

        #region ExecuteDatasetTypedParams

        /// <summary>
        /// Execute a stored procedure via an IDbCommand (that returns a resultset) against the database specified in 
        /// the connection string using the dataRow column values as the stored procedure's parameters values.
        /// This method will assign the paraemter values based on row values.
        /// </summary>
        /// <param name="command">The IDbCommand to execute</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>A DataSet containing the resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if command is null.</exception>
        public virtual DataSet ExecuteDatasetTypedParams(IDbCommand command, DataRow dataRow)
        {
            DataSet ds;

            // Clean Up Parameter Syntax
            CleanParameterSyntax(command);

            // If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Set the parameters values
                AssignParameterValues(command.Parameters, dataRow);


                ds = ExecuteDataset(command);
            }
            else
            {
                ds = ExecuteDataset(command);
            }

            return ds;
        }

        #endregion

        #region ExecuteReaderTypedParams

        /// <summary>
        /// Execute a stored procedure via an IDbCommand (that returns a resultset) against the database specified in 
        /// the connection string using the dataRow column values as the stored procedure's parameters values.
        /// This method will assign the parameter values based on parameter order.
        /// </summary>
        /// <param name="command">The IDbCommand to execute</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>A IDataReader containing the resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if command is null.</exception>
        public virtual IDataReader ExecuteReaderTypedParams(IDbCommand command, DataRow dataRow)
        {
            IDataReader reader;

            // Clean Up Parameter Syntax
            CleanParameterSyntax(command);

            // If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Set the parameters values
                AssignParameterValues(command.Parameters, dataRow);

                reader = ExecuteReader(command);
            }
            else
            {
                reader = ExecuteReader(command);
            }

            return reader;
        }

        #endregion

        #region ExecuteScalarTypedParams

        /// <summary>
        /// Execute a stored procedure via an IDbCommand (that returns a 1x1 resultset) against the database specified in 
        /// the connection string using the dataRow column values as the stored procedure's parameters values.
        /// This method will assign the parameter values based on parameter order.
        /// </summary>
        /// <param name="command">The IDbCommand to execute</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if command is null.</exception>
        public virtual object ExecuteScalarTypedParams(IDbCommand command, DataRow dataRow)
        {
            object retVal;

            // Clean Up Parameter Syntax
            CleanParameterSyntax(command);

            // If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Set the parameters values
                AssignParameterValues(command.Parameters, dataRow);

                retVal = ExecuteScalar(command);
            }
            else
            {
                retVal = ExecuteScalar(command);
            }

            return retVal;
        }

        #endregion

        #region Parameter Discovery Functions

        /// <summary>
        /// Retrieves the set of IDataParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connectionString">A valid connection string for an IDbConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <returns>An array of IDataParameterParameters</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionString is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if spName is null</exception>
        public virtual IDataParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }

        /// <summary>
        /// Retrieves the set of IDataParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connectionString">A valid connection string for an IDbConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of IDataParameters</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionString is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if spName is null</exception>
        public virtual IDataParameter[] GetSpParameterSet(string connectionString, string spName,
            bool includeReturnValueParameter)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            if (string.IsNullOrEmpty(spName)) throw new ArgumentNullException(nameof(spName));

            using (var connection = GetConnection(connectionString))
            {
                return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// Retrieves the set of IDataParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connection">A valid IDataConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <returns>An array of IDataParameters</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if spName is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if connection is null</exception>
        public virtual IDataParameter[] GetSpParameterSet(IDbConnection connection, string spName)
        {
            return GetSpParameterSet(connection, spName, false);
        }

        /// <summary>
        /// Retrieves the set of IDataParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connection">A valid IDbConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of IDataParameters</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if spName is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if connection is null</exception>
        public virtual IDataParameter[] GetSpParameterSet(IDbConnection connection, string spName,
            bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (!(connection is ICloneable))
                throw new ArgumentException(
                    "can't discover parameters if the connection doesn't implement the ICloneable interface",
                    nameof(connection));

            var clonedConnection = (IDbConnection) ((ICloneable) connection).Clone();
            return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
        }

        /// <summary>
        /// Retrieves the set of IDataParameters appropriate for the stored procedure
        /// </summary>
        /// <param name="connection">A valid IDbConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of IDataParameters</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if spName is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if connection is null</exception>
        private IDataParameter[] GetSpParameterSetInternal(IDbConnection connection, string spName,
            bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrEmpty(spName)) throw new ArgumentNullException(nameof(spName));

            // string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter":"");

            var cachedParameters = GetCachedParameterSet(connection,
                spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : ""));

            if (cachedParameters == null)
            {
                var spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
                CacheParameterSet(connection,
                    spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : ""), spParameters);

                cachedParameters = AdoHelperParameterCache.CloneParameters(spParameters);
            }

            return cachedParameters;
        }

        /// <summary>
        /// Retrieve a parameter array from the cache
        /// </summary>
        /// <param name="connectionString">A valid connection string for an IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <returns>An array of IDataParameters</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionString is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        public IDataParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            using (var connection = GetConnection(connectionString))
            {
                return GetCachedParameterSetInternal(connection, commandText);
            }
        }

        /// <summary>
        /// Retrieve a parameter array from the cache
        /// </summary>
        /// <param name="connection">A valid IDbConnection object</param>
        /// <param name="commandText">SQL command</param>
        /// <returns>An array of IDataParameters</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if connection is null</exception>
        public IDataParameter[] GetCachedParameterSet(IDbConnection connection, string commandText)
        {
            return GetCachedParameterSetInternal(connection, commandText);
        }

        /// <summary>
        /// Retrieve a parameter array from the cache
        /// </summary>
        /// <param name="connection">A valid IDbConnection object</param>
        /// <param name="commandText">SQL command</param>
        /// <returns>An array of IDataParameters</returns>
        private IDataParameter[] GetCachedParameterSetInternal(IDbConnection connection, string commandText)
        {
            var mustCloseConnection = false;
            // this way we control the connection, and therefore the connection string that gets saved as a hash key
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                mustCloseConnection = true;
            }

            var parameters = AdoHelperParameterCache.GetCachedParameterSet(connection.ConnectionString, commandText);

            if (mustCloseConnection)
            {
                connection.Close();
            }

            return parameters;
        }

        /// <summary>
        /// Add parameter array to the cache
        /// </summary>
        /// <param name="connectionString">A valid connection string for an IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">An array of IDataParameters to be cached</param>
        public void CacheParameterSet(string connectionString, string commandText,
            params IDataParameter[] commandParameters)
        {
            using (var connection = GetConnection(connectionString))
            {
                CacheParameterSetInternal(connection, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Add parameter array to the cache
        /// </summary>
        /// <param name="connection">A valid IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">An array of IDataParameters to be cached</param>
        public void CacheParameterSet(IDbConnection connection, string commandText,
            params IDataParameter[] commandParameters)
        {
            if (connection is ICloneable)
            {
                using (var clonedConnection = (IDbConnection) ((ICloneable) connection).Clone())
                {
                    CacheParameterSetInternal(clonedConnection, commandText, commandParameters);
                }
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        /// <summary>
        /// Add parameter array to the cache
        /// </summary>
        /// <param name="connection">A valid IDbConnection</param>
        /// <param name="commandText">SQL command</param>
        /// <param name="commandParameters">An array of IDataParameters to be cached</param>
        private void CacheParameterSetInternal(IDbConnection connection, string commandText,
            params IDataParameter[] commandParameters)
        {
            // this way we control the connection, and therefore the connection string that gets saved as a hask key
            connection.Open();
            AdoHelperParameterCache.CacheParameterSet(connection.ConnectionString, commandText, commandParameters);
            connection.Close();
        }

        /// <summary>
        /// Resolve at run time the appropriate set of IDataParameters for a stored procedure
        /// </summary>
        /// <param name="connection">A valid IDbConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">Whether or not to include their return value parameter</param>
        /// <returns>The parameter array discovered.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if spName is null</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if connection is null</exception>
        private IDataParameter[] DiscoverSpParameterSet(IDbConnection connection, string spName,
            bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrEmpty(spName)) throw new ArgumentNullException(nameof(spName));

            var cmd = connection.CreateCommand();
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            DeriveParameters(cmd);
            connection.Close();

            if (!includeReturnValueParameter)
            {
                // not all providers have return value parameters...don't just remove this parameter indiscriminately
                if (cmd.Parameters.Count > 0 &&
                    ((IDataParameter) cmd.Parameters[0]).Direction == ParameterDirection.ReturnValue)
                {
                    cmd.Parameters.RemoveAt(0);
                }
            }

            var discoveredParameters = new IDataParameter[cmd.Parameters.Count];

            cmd.Parameters.CopyTo(discoveredParameters, 0);

            // Init the parameters with a DBNull value
            foreach (var discoveredParameter in discoveredParameters)
            {
                discoveredParameter.Value = DBNull.Value;
            }
            return discoveredParameters;
        }

        #endregion Parameter Discovery Functions

        #region Utility Functions

        public string GetPageSqlString(string tableName, string columnNames,
            string whereSqlString, string orderSqlString, int offset, int limit)
        {
            return SqlDifferences.GetSqlString(tableName, TranslateUtils.StringCollectionToStringList(columnNames),
            whereSqlString, orderSqlString, offset, limit);
        }

        public string ToPlusSqlString(string fieldName, int plusNum)
        {
            return $"{fieldName} = {SqlDifferences.ColumnIncrement(fieldName, plusNum)}";
        }

        public string ToMinusSqlString(string fieldName, int minusNum)
        {
            return $"{fieldName} = {SqlDifferences.ColumnDecrement(fieldName, minusNum)}";
        }

        public string ToNowSqlString()
        {
            return SqlUtils.GetComparableNow();
        }

        public string ToDateSqlString(DateTime date)
        {
            return SqlUtils.GetComparableDate(date);
        }

        public string ToDateTimeSqlString(DateTime dateTime)
        {
            return SqlUtils.GetComparableDate(dateTime);
        }

        public string ToBooleanSqlString(bool val)
        {
            return val ? "1" : "0";
        }

        

        #endregion Utility Functions

    }

    #region ParameterCache
    /// <summary>
    /// ADOHelperParameterCache provides functions to leverage a static cache of procedure parameters, and the
    /// ability to discover parameters for stored procedures at run-time.
    /// </summary>
    public sealed class AdoHelperParameterCache
	{
		private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// Deep copy of cached IDataParameter array
		/// </summary>
		/// <param name="originalParameters"></param>
		/// <returns></returns>
		internal static IDataParameter[] CloneParameters(IDataParameter[] originalParameters)
		{
			var clonedParameters = new IDataParameter[originalParameters.Length];

			for (int i = 0, j = originalParameters.Length; i < j; i++)
			{
				clonedParameters[i] = (IDataParameter)((ICloneable)originalParameters[i]).Clone();
			}
			
			return clonedParameters;
		}

#region caching functions

		/// <summary>
		/// Add parameter array to the cache
		/// </summary>
		/// <param name="connectionString">A valid connection string for an IDbConnection</param>
		/// <param name="commandText">SQL command</param>
		/// <param name="commandParameters">An array of IDataParameters to be cached</param>
		/// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
		/// <exception cref="System.ArgumentNullException">Thrown if connectionString is null</exception>
		internal static void CacheParameterSet(string connectionString, string commandText, params IDataParameter[] commandParameters)
		{
			if( string.IsNullOrEmpty(connectionString) ) throw new ArgumentNullException( nameof(connectionString) );
			if( string.IsNullOrEmpty(commandText) ) throw new ArgumentNullException( nameof(commandText) );

			var hashKey = connectionString + ":" + commandText;

			paramCache[hashKey] = commandParameters;
		}

		/// <summary>
		/// Retrieve a parameter array from the cache
		/// </summary>
		/// <param name="connectionString">A valid connection string for an IDbConnection</param>
		/// <param name="commandText">SQL command</param>
		/// <returns>An array of IDataParameters</returns>
		/// <exception cref="System.ArgumentNullException">Thrown if commandText is null</exception>
		/// <exception cref="System.ArgumentNullException">Thrown if connectionString is null</exception>
		internal static IDataParameter[] GetCachedParameterSet(string connectionString, string commandText)
		{
			if( string.IsNullOrEmpty(connectionString) ) throw new ArgumentNullException( nameof(connectionString) );
			if( string.IsNullOrEmpty(commandText) ) throw new ArgumentNullException( nameof(commandText) );

			var hashKey = connectionString + ":" + commandText;

			var cachedParameters = paramCache[hashKey] as IDataParameter[];
			if (cachedParameters == null)
			{			
				return null;
			}
			else
			{
				return CloneParameters(cachedParameters);
			}
		}

#endregion caching functions
	}
#endregion
}
