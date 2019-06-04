using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Dapper;
using SqlKata.Compilers;
using SS.CMS.Plugin.Data.Database;
using SS.CMS.Plugin.Data.Utils;

[assembly: InternalsVisibleTo("SS.CMS.Plugin.Tests")]

namespace SS.CMS.Plugin.Data
{
    public static class DatoryUtils
    {
        public const int VarCharDefaultLength = 500;

        private static readonly ConcurrentDictionary<string, bool> UseLegacyPagination = new ConcurrentDictionary<string, bool>();

        public static bool IsUseLegacyPagination(DatabaseType databaseType, string connectionString)
        {
            var database = $"{databaseType.Value}:{connectionString}";

            if (UseLegacyPagination.TryGetValue(database, out var useLegacyPagination)) return useLegacyPagination;
            useLegacyPagination = false;

            if (databaseType == DatabaseType.SqlServer)
            {
                const string sqlString = "select left(cast(serverproperty('productversion') as varchar), 4)";

                try
                {
                    using (var connection = new Connection(databaseType, connectionString))
                    {
                        var version = connection.ExecuteScalar<string>(sqlString);

                        useLegacyPagination = Utilities.ToDecimal(version) < 11;
                    }
                }
                catch
                {
                    // ignored
                }
            }

            UseLegacyPagination[database] = useLegacyPagination;

            return useLegacyPagination;
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
                if (databaseType == DatabaseType.Oracle)
                {
                    var userName = Utilities.GetConnectionStringUserName(connectionString);
                    var sql = $"SELECT COUNT(*) FROM ALL_OBJECTS WHERE OBJECT_TYPE = 'TABLE' AND OWNER = '{userName.ToUpper()}' and OBJECT_NAME = '{tableName}'";

                    using (var connection = new Connection(databaseType, connectionString))
                    {
                        exists = connection.ExecuteScalar<int>(sql) == 1;
                    }
                    
                }
                else if (databaseType == DatabaseType.SQLite)
                {
                    var sql = $"SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{tableName}'";

                    using (var connection = new Connection(databaseType, connectionString))
                    {
                        exists = connection.ExecuteScalar<int>(sql) == 1;
                    }
                }
                else // ANSI SQL way.  Works in PostgreSQL, MSSQL, MySQL.  
                {
                    var sql = $"select case when exists((select * from information_schema.tables where table_name = '{tableName}')) then 1 else 0 end";

                    using (var connection = new Connection(databaseType, connectionString))
                    {
                        exists = connection.ExecuteScalar<int>(sql) == 1;
                    }
                }
            }
            catch
            {
                try
                {
                    var sql = $"select 1 from {tableName} where 1 = 0";

                    using (var connection = new Connection(databaseType, connectionString))
                    {
                        exists = connection.ExecuteScalar<int>(sql) == 1;
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
                if (column.IsIdentity || Utilities.EqualsIgnoreCase(column.AttributeName, "id"))
                {
                    identityColumnName = column.AttributeName;
                    break;
                }
            }

            if (string.IsNullOrEmpty(identityColumnName))
            {
                identityColumnName = nameof(Entity.Id);
                var sqlString =
                    GetAddColumnsSqlString(databaseType, tableName, $"{identityColumnName} {GetAutoIncrementDataType(databaseType, true)}");

                using (var connection = new Connection(databaseType, connectionString))
                {
                    connection.Execute(sqlString);
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

        public static void AlterTable(DatabaseType databaseType, string connectionString, string tableName, IList<TableColumn> tableColumns, IList<string> dropColumnNames = null)
        {
            var list = new List<string>();

            var columnNameList = GetColumnNames(databaseType, connectionString, tableName);
            foreach (var tableColumn in tableColumns)
            {
                if (!Utilities.ContainsIgnoreCase(columnNameList, tableColumn.AttributeName))
                {
                    list.Add(GetAddColumnsSqlString(databaseType, tableName, GetColumnSqlString(databaseType, tableColumn)));
                }
            }

            if (dropColumnNames != null)
            {
                foreach (var columnName in columnNameList)
                {
                    if (Utilities.ContainsIgnoreCase(dropColumnNames, columnName))
                    {
                        list.Add(GetDropColumnsSqlString(databaseType, tableName, columnName));
                    }
                }
            }

            if (list.Count <= 0) return;

            foreach (var sqlString in list)
            {
                using (var connection = new Connection(databaseType, connectionString))
                {
                    connection.Execute(sqlString);
                }
            }
        }

        public static void CreateTable(DatabaseType databaseType, string connectionString, string tableName, List<TableColumn> tableColumns)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@"CREATE TABLE {GetQuotedIdentifier(databaseType, tableName)} (").AppendLine();

            var primaryKeyColumns = new List<TableColumn>();
            TableColumn identityColumn = null;

            foreach (var tableColumn in tableColumns)
            {
                if (Utilities.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.Id)))
                {
                    tableColumn.DataType = DataType.Integer;
                    tableColumn.IsIdentity = true;
                    tableColumn.IsPrimaryKey = true;
                }
                else if (Utilities.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.Guid)))
                {
                    tableColumn.DataType = DataType.VarChar;
                    tableColumn.DataLength = 50;
                }
                else if (Utilities.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.LastModifiedDate)))
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

                var columnSql = GetColumnSqlString(databaseType, tableColumn);
                if (!string.IsNullOrEmpty(columnSql))
                {
                    sqlBuilder.Append(columnSql).Append(",");
                }
            }

            if (databaseType != DatabaseType.SQLite)
            {
                if (identityColumn != null)
                {
                    var primaryKeySql = GetPrimaryKeySqlString(databaseType, tableName, identityColumn.AttributeName);
                    if (!string.IsNullOrEmpty(primaryKeySql))
                    {
                        sqlBuilder.Append(primaryKeySql).Append(",");
                    }
                }
                else if (primaryKeyColumns.Count > 0)
                {
                    foreach (var tableColumn in primaryKeyColumns)
                    {
                        var primaryKeySql = GetPrimaryKeySqlString(databaseType, tableName, tableColumn.AttributeName);
                        if (!string.IsNullOrEmpty(primaryKeySql))
                        {
                            sqlBuilder.Append(primaryKeySql).Append(",");
                        }
                    }
                }
            }

            sqlBuilder.Length--;

            sqlBuilder.AppendLine().Append(databaseType == DatabaseType.MySql
                ? ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4"
                : ")");

            using (var connection = new Connection(databaseType, connectionString))
            {
                connection.Execute(sqlBuilder.ToString());
            }
        }

        public static void CreateIndex(DatabaseType databaseType, string connectionString, string tableName, string indexName, params string[] columns)
        {
            var sqlString = new StringBuilder($@"CREATE INDEX {GetQuotedIdentifier(databaseType, indexName)} ON {GetQuotedIdentifier(databaseType, tableName)}(");

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
                sqlString.Append($"{GetQuotedIdentifier(databaseType, columnName)} {columnOrder}, ");
            }

            sqlString.Length--;
            sqlString.Append(")");

            using (var connection = new Connection(databaseType, connectionString))
            {
                connection.Execute(sqlString.ToString());
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

        public static void DropTable(DatabaseType databaseType, string connectionString, string tableName)
        {
            using (var connection = new Connection(databaseType, connectionString))
            {
                connection.Execute($"DROP TABLE {GetQuotedIdentifier(databaseType, tableName)}");
            }
        }

        public static IDbConnection GetConnection(DatabaseType databaseType, string connectionString)
        {
            IDbConnection conn = null;

            if (databaseType == DatabaseType.MySql)
            {
                conn = Database.MySql.Instance.GetConnection(connectionString);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                conn = Database.SqlServer.Instance.GetConnection(connectionString);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                conn = Database.PostgreSql.Instance.GetConnection(connectionString);
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                conn = Database.Oracle.Instance.GetConnection(connectionString);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                conn = Database.SQLite.Instance.GetConnection(connectionString);
            }

            return conn;
        }

        public static Compiler GetCompiler(DatabaseType databaseType, string connectionString)
        {
            Compiler compiler = null;

            if (databaseType == DatabaseType.MySql)
            {
                compiler = Database.MySql.Instance.GetCompiler(connectionString);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                compiler = Database.SqlServer.Instance.GetCompiler(connectionString);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                compiler = Database.PostgreSql.Instance.GetCompiler(connectionString);
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                compiler = Database.Oracle.Instance.GetCompiler(connectionString);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                compiler = Database.SQLite.Instance.GetCompiler(connectionString);
            }

            return compiler;
        }

        public static List<TableColumn> GetTableColumns(DatabaseType databaseType, string connectionString, string tableName)
        {
            List<TableColumn> list = null;

            if (databaseType == DatabaseType.MySql)
            {
                list = Database.MySql.Instance.GetTableColumns(connectionString, tableName);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                list = Database.SqlServer.Instance.GetTableColumns(connectionString, tableName);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                list = Database.PostgreSql.Instance.GetTableColumns(connectionString, tableName);
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                list = Database.Oracle.Instance.GetTableColumns(connectionString, tableName);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                list = Database.SQLite.Instance.GetTableColumns(connectionString, tableName);
            }

            return list;
        }

        public static List<string> GetTableNames(DatabaseType databaseType, string connectionString)
        {
            List<string> tableNames = null;

            if (databaseType == DatabaseType.MySql)
            {
                tableNames = Database.MySql.Instance.GetTableNames(connectionString);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                tableNames = Database.SqlServer.Instance.GetTableNames(connectionString);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                tableNames = Database.PostgreSql.Instance.GetTableNames(connectionString);
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                tableNames = Database.Oracle.Instance.GetTableNames(connectionString);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                tableNames = Database.SQLite.Instance.GetTableNames(connectionString);
            }

            return tableNames;
        }

        public static string ColumnIncrement(DatabaseType databaseType, string columnName, int plusNum = 1)
        {
            var retVal = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                retVal = Database.MySql.Instance.ColumnIncrement(columnName, plusNum);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                retVal = Database.SqlServer.Instance.ColumnIncrement(columnName, plusNum);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                retVal = Database.PostgreSql.Instance.ColumnIncrement(columnName, plusNum);
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                retVal = Database.Oracle.Instance.ColumnIncrement(columnName, plusNum);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                retVal = Database.SQLite.Instance.ColumnIncrement(columnName, plusNum);
            }

            return retVal;
        }

        public static string ColumnDecrement(DatabaseType databaseType, string columnName, int minusNum = 1)
        {
            var retVal = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                retVal = Database.MySql.Instance.ColumnDecrement(columnName, minusNum);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                retVal = Database.SqlServer.Instance.ColumnDecrement(columnName, minusNum);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                retVal = Database.PostgreSql.Instance.ColumnDecrement(columnName, minusNum);
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                retVal = Database.Oracle.Instance.ColumnDecrement(columnName, minusNum);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                retVal = Database.SQLite.Instance.ColumnDecrement(columnName, minusNum);
            }

            return retVal;
        }

        public static string GetAutoIncrementDataType(DatabaseType databaseType, bool alterTable = false)
        {
            var retVal = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                retVal = Database.MySql.Instance.GetAutoIncrementDataType(alterTable);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                retVal = Database.SqlServer.Instance.GetAutoIncrementDataType(alterTable);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                retVal = Database.PostgreSql.Instance.GetAutoIncrementDataType(alterTable);
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                retVal = Database.Oracle.Instance.GetAutoIncrementDataType(alterTable);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                retVal = Database.SQLite.Instance.GetAutoIncrementDataType(alterTable);
            }

            return retVal;
        }

        public static string GetColumnSqlString(DatabaseType databaseType, TableColumn tableColumn)
        {
            var retVal = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                retVal = Database.MySql.Instance.GetColumnSqlString(tableColumn);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                retVal = Database.SqlServer.Instance.GetColumnSqlString(tableColumn);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                retVal = Database.PostgreSql.Instance.GetColumnSqlString(tableColumn);
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                retVal = Database.Oracle.Instance.GetColumnSqlString(tableColumn);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                retVal = Database.SQLite.Instance.GetColumnSqlString(tableColumn);
            }

            return retVal;
        }

        public static string GetPrimaryKeySqlString(DatabaseType databaseType, string tableName, string attributeName)
        {
            var retVal = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                retVal = Database.MySql.Instance.GetPrimaryKeySqlString(tableName, attributeName);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                retVal = Database.SqlServer.Instance.GetPrimaryKeySqlString(tableName, attributeName);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                retVal = Database.PostgreSql.Instance.GetPrimaryKeySqlString(tableName, attributeName);
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                retVal = Database.Oracle.Instance.GetPrimaryKeySqlString(tableName, attributeName);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                retVal = Database.SQLite.Instance.GetPrimaryKeySqlString(tableName, attributeName);
            }

            return retVal;
        }

        public static string GetQuotedIdentifier(DatabaseType databaseType, string identifier)
        {
            var retVal = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                retVal = Database.MySql.Instance.GetQuotedIdentifier(identifier);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                retVal = Database.SqlServer.Instance.GetQuotedIdentifier(identifier);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                retVal = Database.PostgreSql.Instance.GetQuotedIdentifier(identifier);
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                retVal = Database.Oracle.Instance.GetQuotedIdentifier(identifier);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                retVal = Database.SQLite.Instance.GetQuotedIdentifier(identifier);
            }

            return retVal;
        }

        public static string GetAddColumnsSqlString(DatabaseType databaseType, string tableName, string columnsSqlString)
        {
            var retVal = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                retVal = Database.MySql.Instance.GetAddColumnsSqlString(tableName, columnsSqlString);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                retVal = Database.SqlServer.Instance.GetAddColumnsSqlString(tableName, columnsSqlString);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                retVal = Database.PostgreSql.Instance.GetAddColumnsSqlString(tableName, columnsSqlString);
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                retVal = Database.Oracle.Instance.GetAddColumnsSqlString(tableName, columnsSqlString);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                retVal = SQLite.Instance.GetAddColumnsSqlString(tableName, columnsSqlString);
            }

            return retVal;
        }

        public static string GetDropColumnsSqlString(DatabaseType databaseType, string tableName, string columnName)
        {
            return $"ALTER TABLE {GetQuotedIdentifier(databaseType, tableName)} DROP COLUMN {GetQuotedIdentifier(databaseType, columnName)}";
        }
    }
}
