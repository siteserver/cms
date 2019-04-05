using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using Datory.Utils;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;

namespace Datory
{
    public static class DatoryUtils
    {
        public const int VarCharDefaultLength = 500;

        public static DatabaseType GetDatabaseType()
        {
            var databaseType = ConfigurationManager.AppSettings["DatabaseType"];

            var retVal = DatabaseType.SqlServer;

            if (ConvertUtils.EqualsIgnoreCase(databaseType, DatabaseType.MySql.Value))
            {
                retVal = DatabaseType.MySql;
            }
            else if (ConvertUtils.EqualsIgnoreCase(databaseType, DatabaseType.SqlServer.Value))
            {
                retVal = DatabaseType.SqlServer;
            }
            else if (ConvertUtils.EqualsIgnoreCase(databaseType, DatabaseType.PostgreSql.Value))
            {
                retVal = DatabaseType.PostgreSql;
            }
            else if (ConvertUtils.EqualsIgnoreCase(databaseType, DatabaseType.Oracle.Value))
            {
                retVal = DatabaseType.Oracle;
            }

            return retVal;
        }

        public static string GetConnectionString()
        {
            return ConfigurationManager.AppSettings["ConnectionString"];
        }

        private static readonly ConcurrentDictionary<DatabaseType, bool> UseLegacyPagination = new ConcurrentDictionary<DatabaseType, bool>();

        public static bool IsUseLegacyPagination(DatabaseType databaseType, string connectionString)
        {
            if (UseLegacyPagination.TryGetValue(databaseType, out var useLegacyPagination)) return useLegacyPagination;
            useLegacyPagination = false;

            if (databaseType == DatabaseType.SqlServer)
            {
                const string sqlString = "select left(cast(serverproperty('productversion') as varchar), 4)";

                try
                {
                    using (var conn = GetConnection(databaseType, connectionString))
                    {
                        var version = conn.ExecuteScalar<string>(sqlString);

                        useLegacyPagination = ConvertUtils.ToDecimal(version) < 11;
                    }
                }
                catch
                {
                    // ignored
                }
            }

            UseLegacyPagination[databaseType] = useLegacyPagination;

            return useLegacyPagination;
        }

        public static IDbConnection GetConnection(DatabaseType databaseType, string connectionString)
        {
            IDbConnection conn = null;

            if (databaseType == DatabaseType.MySql)
            {
                conn = new MySqlConnection(connectionString);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                conn = new SqlConnection(connectionString);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                conn = new NpgsqlConnection(connectionString);
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                conn = new OracleConnection(connectionString);
            }

            return conn;
        }

        public static bool IsTableExists(DatabaseType databaseType, string connectionString, string tableName)
        {
            bool exists;

            if (databaseType == DatabaseType.Oracle)
            {
                tableName = tableName.ToUpper();
            }
            else if (databaseType == DatabaseType.MySql || databaseType == DatabaseType.PostgreSql)
            {
                tableName = tableName.ToLower();
            }

            try
            {
                // ANSI SQL way.  Works in PostgreSQL, MSSQL, MySQL.  
                if (databaseType != DatabaseType.Oracle)
                {
                    using (var conn = GetConnection(databaseType, connectionString))
                    {
                        var sql = $"select case when exists((select * from information_schema.tables where table_name = '{tableName}')) then 1 else 0 end";

                        exists = conn.ExecuteScalar<int>(sql) == 1;
                    }
                }
                else
                {
                    using (var conn = GetConnection(databaseType, connectionString))
                    {
                        var sql = $"SELECT COUNT(*) FROM ALL_OBJECTS WHERE OBJECT_TYPE = 'TABLE' AND OWNER = '{SqlUtils.GetConnectionStringUserId(connectionString).ToUpper()}' and OBJECT_NAME = '{tableName}'";

                        exists = conn.ExecuteScalar<int>(sql) == 1;
                    }
                }
            }
            catch
            {
                try
                {
                    // Other DB.  Graceful degradation
                    using (var conn = GetConnection(databaseType, connectionString))
                    {
                        var sql = $"select 1 from {tableName} where 1 = 0";

                        exists = conn.ExecuteScalar<int>(sql) == 1;
                    }
                }
                catch
                {
                    exists = false;
                }
            }

            return exists;
        }

        public static string AddIdentityColumnIdIfNotExists(DatabaseType databaseType, string connectionString, string tableName, List<TableColumn> columns)
        {
            var identityColumnName = string.Empty;
            foreach (var column in columns)
            {
                if (column.IsIdentity || ConvertUtils.EqualsIgnoreCase(column.AttributeName, "id"))
                {
                    identityColumnName = column.AttributeName;
                    break;
                }
            }

            if (string.IsNullOrEmpty(identityColumnName))
            {
                identityColumnName = nameof(Entity.Id);
                var sqlString =
                    SqlUtils.GetAddColumnsSqlString(databaseType, tableName, $"{identityColumnName} {SqlUtils.GetAutoIncrementDataType(databaseType, true)}");

                using (var conn = GetConnection(databaseType, connectionString))
                {
                    conn.Execute(sqlString);
                }

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

        public static void AlterTable(DatabaseType databaseType, string connectionString, string tableName, IList<TableColumn> tableColumns, IList<string> dropColumnNames)
        {
            var list = new List<string>();

            var columnNameList = GetColumnNames(databaseType, connectionString, tableName);
            foreach (var tableColumn in tableColumns)
            {
                if (!ConvertUtils.ContainsIgnoreCase(columnNameList, tableColumn.AttributeName))
                {
                    list.Add(SqlUtils.GetAddColumnsSqlString(databaseType, tableName, SqlUtils.GetColumnSqlString(databaseType, tableColumn)));
                }
            }

            if (dropColumnNames != null)
            {
                foreach (var columnName in columnNameList)
                {
                    if (ConvertUtils.ContainsIgnoreCase(dropColumnNames, columnName))
                    {
                        list.Add(SqlUtils.GetDropColumnsSqlString(databaseType, tableName, columnName));
                    }
                }
            }

            if (list.Count <= 0) return;

            using (var conn = GetConnection(databaseType, connectionString))
            {
                foreach (var sqlString in list)
                {
                    conn.Execute(sqlString);
                }
            }
        }

        public static void CreateTable(DatabaseType databaseType, string connectionString, string tableName, List<TableColumn> tableColumns)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@"CREATE TABLE {tableName} (").AppendLine();

            var primaryKeyColumns = new List<TableColumn>();
            TableColumn identityColumn = null;

            foreach (var tableColumn in tableColumns)
            {
                if (ConvertUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.Id)))
                {
                    tableColumn.DataType = DataType.Integer;
                    tableColumn.IsIdentity = true;
                    tableColumn.IsPrimaryKey = true;
                }
                else if (ConvertUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.Guid)))
                {
                    tableColumn.DataType = DataType.VarChar;
                    tableColumn.DataLength = 50;
                }
                else if (ConvertUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.LastModifiedDate)))
                {
                    tableColumn.DataType = DataType.DateTime;
                }
            }

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

                if (tableColumn.DataType == DataType.VarChar && tableColumn.DataLength == 0)
                {
                    tableColumn.DataLength = VarCharDefaultLength;
                }

                var columnSql = SqlUtils.GetColumnSqlString(databaseType, tableColumn);
                if (!string.IsNullOrEmpty(columnSql))
                {
                    sqlBuilder.Append(columnSql).Append(",");
                }
            }

            if (identityColumn != null)
            {
                var primaryKeySql = SqlUtils.GetPrimaryKeySqlString(databaseType, tableName, identityColumn.AttributeName);
                if (!string.IsNullOrEmpty(primaryKeySql))
                {
                    sqlBuilder.Append(primaryKeySql).Append(",");
                }
            }
            else if (primaryKeyColumns.Count > 0)
            {
                foreach (var tableColumn in primaryKeyColumns)
                {
                    var primaryKeySql = SqlUtils.GetPrimaryKeySqlString(databaseType, tableName, tableColumn.AttributeName);
                    if (!string.IsNullOrEmpty(primaryKeySql))
                    {
                        sqlBuilder.Append(primaryKeySql).Append(",");
                    }
                }
            }

            sqlBuilder.Length--;

            sqlBuilder.AppendLine().Append(databaseType == DatabaseType.MySql
                ? ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4"
                : ")");
            
            using (var conn = GetConnection(databaseType, connectionString))
            {
                conn.Execute(sqlBuilder.ToString());
            }
        }

        public static void CreateIndex(DatabaseType databaseType, string connectionString, string tableName, string indexName, params string[] columns)
        {
            var sqlString = new StringBuilder($@"CREATE INDEX {SqlUtils.GetQuotedIdentifier(databaseType, indexName)} ON {SqlUtils.GetQuotedIdentifier(databaseType, tableName)}(");

            foreach (var column in columns)
            {
                var columnName = column;
                var columnOrder = "ASC";
                var i = column.IndexOf(" ", StringComparison.Ordinal);
                if (i != -1)
                {
                    columnName = column.Substring(0, i);
                    columnOrder = column.Substring(i + 1);
                }
                sqlString.Append($"{SqlUtils.GetQuotedIdentifier(databaseType, columnName)} {columnOrder}, ");
            }

            sqlString.Length--;
            sqlString.Append(")");

            using (var conn = GetConnection(databaseType, connectionString))
            {
                conn.Execute(sqlString.ToString());
            }
        }

        public static List<string> GetColumnNames(DatabaseType databaseType, string connectionString, string tableName)
        {
            var allTableColumnInfoList = GetTableColumns(databaseType, connectionString, tableName);
            return allTableColumnInfoList.Select(tableColumnInfo => tableColumnInfo.AttributeName).ToList();
        }

        public static List<TableColumn> GetTableColumns<T>() where T : Entity
        {
            return ReflectionUtils.GetTableColumns(typeof(T));
        }

        public static List<TableColumn> GetTableColumns(DatabaseType databaseType, string connectionString, string tableName)
        {
            var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(databaseType, connectionString);

            List<TableColumn> list = null;

            if (databaseType == DatabaseType.MySql)
            {
                list = SqlUtils.GetMySqlColumns(databaseType, connectionString, databaseName, tableName);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                list = SqlUtils.GetSqlServerColumns(databaseType, connectionString, databaseName, tableName);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                list = SqlUtils.GetPostgreSqlColumns(databaseType, connectionString, databaseName, tableName);
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                list = SqlUtils.GetOracleColumns(databaseType, connectionString, tableName);
            }

            return list;
        }

        public static List<string> GetTableNames(DatabaseType databaseType, string connectionString)
        {
            var sqlString = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(databaseType, connectionString);
                sqlString = $"SELECT table_name FROM information_schema.tables WHERE table_schema='{databaseName}' ORDER BY table_name";
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(databaseType, connectionString);
                sqlString =
                    $"SELECT name FROM [{databaseName}]..sysobjects WHERE type = 'U' AND category<>2 ORDER BY Name";
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(databaseType, connectionString);
                sqlString =
                    $"SELECT table_name FROM information_schema.tables WHERE table_catalog = '{databaseName}' AND table_type = 'BASE TABLE' AND table_schema NOT IN ('pg_catalog', 'information_schema')";
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                sqlString = "select TABLE_NAME from user_tables";
            }

            if (string.IsNullOrEmpty(sqlString)) return new List<string>();

            IEnumerable<string> tableNames;
            using (var conn = GetConnection(databaseType, connectionString))
            {
                tableNames = conn.Query<string>(sqlString);
            }
            return tableNames.Where(tableName => !string.IsNullOrEmpty(tableName)).ToList();
        }

        public static void DropTable(DatabaseType databaseType, string connectionString, string tableName)
        {
            using (var conn = GetConnection(databaseType, connectionString))
            {
                var sql = $"DROP TABLE {tableName}";

                conn.Execute(sql);
            }
        }

        //public static bool DropTable(DatabaseType databaseType, string connectionString, string tableName, out Exception ex)
        //{
        //    ex = null;
        //    var isAltered = false;

        //    try
        //    {
        //        using (var conn = GetConnection(databaseType, connectionString))
        //        {
        //            conn.Execute($"DROP TABLE {tableName}");
        //        }

        //        isAltered = true;
        //    }
        //    catch (Exception e)
        //    {
        //        ex = e;
        //    }

        //    return isAltered;
        //}
    }
}
