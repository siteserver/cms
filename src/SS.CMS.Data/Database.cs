using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SS.CMS.Data.DatabaseImpl;
using SS.CMS.Data.Utils;
using System.IO;

namespace SS.CMS.Data
{
    public class Database : IDatabase
    {
        public DatabaseType DatabaseType { get; }

        public string ConnectionString { get; }

        public Database(DatabaseType databaseType, string connectionString)
        {
            if (databaseType == null || connectionString == null) return;

            if (databaseType == DatabaseType.MySql)
            {
                connectionString = connectionString.TrimEnd(';');
                if (!Utilities.ContainsIgnoreCase(connectionString, "SslMode="))
                {
                    connectionString += ";SslMode=Preferred;";
                }
                if (!Utilities.ContainsIgnoreCase(connectionString, "CharSet="))
                {
                    connectionString += ";CharSet=utf8;";
                }
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                connectionString = connectionString.TrimEnd(';');
                if (!Utilities.ContainsIgnoreCase(connectionString, "pooling="))
                {
                    connectionString += ";pooling=false;";
                }
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                if (connectionString.Contains("=~/"))
                {
                    var projDirectoryPath = Directory.GetCurrentDirectory();
                    connectionString = connectionString.Replace("=~/", $"={projDirectoryPath}/");
                    connectionString = connectionString.Replace('/', Path.DirectorySeparatorChar);
                }
            }

            DatabaseType = databaseType;
            ConnectionString = connectionString;
        }

        public bool IsTableExists(string tableName)
        {
            bool exists;

            if (DatabaseType == DatabaseType.Oracle)
            {
                tableName = tableName.ToUpper();
            }
            else if (DatabaseType == DatabaseType.MySql || DatabaseType == DatabaseType.PostgreSql)
            {
                tableName = tableName.ToLower();
            }

            try
            {
                if (DatabaseType == DatabaseType.Oracle)
                {
                    var userName = Utilities.GetConnectionStringUserName(ConnectionString);
                    var sql = $"SELECT COUNT(*) FROM ALL_OBJECTS WHERE OBJECT_TYPE = 'TABLE' AND OWNER = '{userName.ToUpper()}' and OBJECT_NAME = '{tableName}'";

                    using (var connection = GetConnection())
                    {
                        exists = connection.ExecuteScalar<int>(sql) == 1;
                    }

                }
                else if (DatabaseType == DatabaseType.SQLite)
                {
                    var sql = $"SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{tableName}'";

                    using (var connection = GetConnection())
                    {
                        exists = connection.ExecuteScalar<int>(sql) == 1;
                    }
                }
                else // ANSI SQL way.  Works in PostgreSQL, MSSQL, MySQL.  
                {
                    var sql = $"select case when exists((select * from information_schema.tables where table_name = '{tableName}')) then 1 else 0 end";

                    using (var connection = GetConnection())
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

                    using (var connection = GetConnection())
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

        public async Task<bool> IsTableExistsAsync(string tableName)
        {
            bool exists;

            if (DatabaseType == DatabaseType.Oracle)
            {
                tableName = tableName.ToUpper();
            }
            else if (DatabaseType == DatabaseType.MySql || DatabaseType == DatabaseType.PostgreSql)
            {
                tableName = tableName.ToLower();
            }

            try
            {
                if (DatabaseType == DatabaseType.Oracle)
                {
                    var userName = Utilities.GetConnectionStringUserName(ConnectionString);
                    var sql = $"SELECT COUNT(*) FROM ALL_OBJECTS WHERE OBJECT_TYPE = 'TABLE' AND OWNER = '{userName.ToUpper()}' and OBJECT_NAME = '{tableName}'";

                    using (var connection = GetConnection())
                    {
                        exists = await connection.ExecuteScalarAsync<int>(sql) == 1;
                    }
                }
                else if (DatabaseType == DatabaseType.SQLite)
                {
                    var sql = $"SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{tableName}'";

                    using (var connection = GetConnection())
                    {
                        exists = await connection.ExecuteScalarAsync<int>(sql) == 1;
                    }
                }
                else // ANSI SQL way.  Works in PostgreSQL, MSSQL, MySQL.  
                {
                    var sql = $"select case when exists((select * from information_schema.tables where table_name = '{tableName}')) then 1 else 0 end";

                    using (var connection = GetConnection())
                    {
                        exists = await connection.ExecuteScalarAsync<int>(sql) == 1;
                    }
                }
            }
            catch
            {
                try
                {
                    var sql = $"select 1 from {tableName} where 1 = 0";

                    using (var connection = GetConnection())
                    {
                        exists = await connection.ExecuteScalarAsync<int>(sql) == 1;
                    }
                }
                catch
                {
                    exists = false;
                }
            }

            return exists;
        }

        public bool IsConnectionWorks(out string errorMessage)
        {
            var retval = false;
            errorMessage = string.Empty;
            try
            {
                var connection = GetConnection();
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    retval = true;
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return retval;
        }

        public string AddIdentityColumnIdIfNotExists(string tableName, List<TableColumn> columns)
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
                    DbUtils.GetAddColumnsSqlString(DatabaseType, tableName, $"{identityColumnName} {DbUtils.GetAutoIncrementDataType(DatabaseType, true)}");

                using (var connection = GetConnection())
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

        public void AlterTable(string tableName, IList<TableColumn> tableColumns, IList<string> dropColumnNames = null)
        {
            var list = new List<string>();

            var columnNameList = GetColumnNames(tableName);
            foreach (var tableColumn in tableColumns)
            {
                if (!Utilities.ContainsIgnoreCase(columnNameList, tableColumn.AttributeName))
                {
                    list.Add(DbUtils.GetAddColumnsSqlString(DatabaseType, tableName, DbUtils.GetColumnSqlString(DatabaseType, tableColumn)));
                }
            }

            if (dropColumnNames != null)
            {
                foreach (var columnName in columnNameList)
                {
                    if (Utilities.ContainsIgnoreCase(dropColumnNames, columnName))
                    {
                        list.Add(DbUtils.GetDropColumnsSqlString(DatabaseType, tableName, columnName));
                    }
                }
            }

            if (list.Count <= 0) return;

            foreach (var sqlString in list)
            {
                using (var connection = GetConnection())
                {
                    connection.Execute(sqlString);
                }
            }
        }

        public async Task AlterTableAsync(string tableName, IList<TableColumn> tableColumns, IList<string> dropColumnNames = null)
        {
            var list = new List<string>();

            var columnNameList = GetColumnNames(tableName);
            foreach (var tableColumn in tableColumns)
            {
                if (!Utilities.ContainsIgnoreCase(columnNameList, tableColumn.AttributeName))
                {
                    list.Add(DbUtils.GetAddColumnsSqlString(DatabaseType, tableName, DbUtils.GetColumnSqlString(DatabaseType, tableColumn)));
                }
            }

            if (dropColumnNames != null)
            {
                foreach (var columnName in columnNameList)
                {
                    if (Utilities.ContainsIgnoreCase(dropColumnNames, columnName))
                    {
                        list.Add(DbUtils.GetDropColumnsSqlString(DatabaseType, tableName, columnName));
                    }
                }
            }

            if (list.Count <= 0) return;

            foreach (var sqlString in list)
            {
                using (var connection = GetConnection())
                {
                    await connection.ExecuteAsync(sqlString);
                }
            }
        }

        public void CreateTable(string tableName, List<TableColumn> tableColumns)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@"CREATE TABLE {DbUtils.GetQuotedIdentifier(DatabaseType, tableName)} (").AppendLine();

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
                else if (Utilities.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.CreationDate)))
                {
                    tableColumn.DataType = DataType.DateTime;
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
                    tableColumn.DataLength = DbUtils.VarCharDefaultLength;
                }

                var columnSql = DbUtils.GetColumnSqlString(DatabaseType, tableColumn);
                if (!string.IsNullOrEmpty(columnSql))
                {
                    sqlBuilder.Append(columnSql).Append(",");
                }
            }

            if (DatabaseType != DatabaseType.SQLite)
            {
                if (identityColumn != null)
                {
                    var primaryKeySql = DbUtils.GetPrimaryKeySqlString(DatabaseType, tableName, identityColumn.AttributeName);
                    if (!string.IsNullOrEmpty(primaryKeySql))
                    {
                        sqlBuilder.Append(primaryKeySql).Append(",");
                    }
                }
                else if (primaryKeyColumns.Count > 0)
                {
                    foreach (var tableColumn in primaryKeyColumns)
                    {
                        var primaryKeySql = DbUtils.GetPrimaryKeySqlString(DatabaseType, tableName, tableColumn.AttributeName);
                        if (!string.IsNullOrEmpty(primaryKeySql))
                        {
                            sqlBuilder.Append(primaryKeySql).Append(",");
                        }
                    }
                }
            }

            sqlBuilder.Length--;

            sqlBuilder.AppendLine().Append(DatabaseType == DatabaseType.MySql
                ? ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4"
                : ")");

            using (var connection = GetConnection())
            {
                connection.Execute(sqlBuilder.ToString());
            }
        }

        public async Task CreateTableAsync(string tableName, List<TableColumn> tableColumns)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@"CREATE TABLE {DbUtils.GetQuotedIdentifier(DatabaseType, tableName)} (").AppendLine();

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
                else if (Utilities.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.CreationDate)))
                {
                    tableColumn.DataType = DataType.DateTime;
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
                    tableColumn.DataLength = DbUtils.VarCharDefaultLength;
                }

                var columnSql = DbUtils.GetColumnSqlString(DatabaseType, tableColumn);
                if (!string.IsNullOrEmpty(columnSql))
                {
                    sqlBuilder.Append(columnSql).Append(",");
                }
            }

            if (DatabaseType != DatabaseType.SQLite)
            {
                if (identityColumn != null)
                {
                    var primaryKeySql = DbUtils.GetPrimaryKeySqlString(DatabaseType, tableName, identityColumn.AttributeName);
                    if (!string.IsNullOrEmpty(primaryKeySql))
                    {
                        sqlBuilder.Append(primaryKeySql).Append(",");
                    }
                }
                else if (primaryKeyColumns.Count > 0)
                {
                    foreach (var tableColumn in primaryKeyColumns)
                    {
                        var primaryKeySql = DbUtils.GetPrimaryKeySqlString(DatabaseType, tableName, tableColumn.AttributeName);
                        if (!string.IsNullOrEmpty(primaryKeySql))
                        {
                            sqlBuilder.Append(primaryKeySql).Append(",");
                        }
                    }
                }
            }

            sqlBuilder.Length--;

            sqlBuilder.AppendLine().Append(DatabaseType == DatabaseType.MySql
                ? ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4"
                : ")");

            using (var connection = GetConnection())
            {
                await connection.ExecuteAsync(sqlBuilder.ToString());
            }
        }

        public void CreateIndex(string tableName, string indexName, params string[] columns)
        {
            var sqlString = new StringBuilder($@"CREATE INDEX {DbUtils.GetQuotedIdentifier(DatabaseType, indexName)} ON {DbUtils.GetQuotedIdentifier(DatabaseType, tableName)}(");

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
                sqlString.Append($"{DbUtils.GetQuotedIdentifier(DatabaseType, columnName)} {columnOrder}, ");
            }

            sqlString.Length--;
            sqlString.Append(")");

            using (var connection = GetConnection())
            {
                connection.Execute(sqlString.ToString());
            }
        }

        public async Task CreateIndexAsync(string tableName, string indexName, params string[] columns)
        {
            var sqlString = new StringBuilder($@"CREATE INDEX {DbUtils.GetQuotedIdentifier(DatabaseType, indexName)} ON {DbUtils.GetQuotedIdentifier(DatabaseType, tableName)}(");

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
                sqlString.Append($"{DbUtils.GetQuotedIdentifier(DatabaseType, columnName)} {columnOrder}, ");
            }

            sqlString.Length--;
            sqlString.Append(")");

            using (var connection = GetConnection())
            {
                await connection.ExecuteAsync(sqlString.ToString());
            }
        }

        public List<string> GetColumnNames(string tableName)
        {
            var allTableColumnInfoList = GetTableColumns(tableName);
            return allTableColumnInfoList.Select(tableColumnInfo => tableColumnInfo.AttributeName).ToList();
        }

        public string GetTableName<T>() where T : Entity
        {
            return ReflectionUtils.GetTableName(typeof(T));
        }

        public List<TableColumn> GetTableColumns<T>() where T : Entity
        {
            return ReflectionUtils.GetTableColumns(typeof(T));
        }

        public void DropTable(string tableName)
        {
            using (var connection = GetConnection())
            {
                connection.Execute($"DROP TABLE {DbUtils.GetQuotedIdentifier(DatabaseType, tableName)}");
            }
        }

        public async Task DropTableAsync(string tableName)
        {
            using (var connection = GetConnection())
            {
                await connection.ExecuteAsync($"DROP TABLE {DbUtils.GetQuotedIdentifier(DatabaseType, tableName)}");
            }
        }

        public DbConnection GetConnection()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString)) return null;

            DbConnection conn = null;
            if (DatabaseType == DatabaseType.MySql)
            {
                conn = MySqlImpl.Instance.GetConnection(ConnectionString);
            }
            else if (DatabaseType == DatabaseType.SqlServer)
            {
                conn = SqlServerImpl.Instance.GetConnection(ConnectionString);
            }
            else if (DatabaseType == DatabaseType.PostgreSql)
            {
                conn = PostgreSqlImpl.Instance.GetConnection(ConnectionString);
            }
            else if (DatabaseType == DatabaseType.Oracle)
            {
                conn = OracleImpl.Instance.GetConnection(ConnectionString);
            }
            else if (DatabaseType == DatabaseType.SQLite)
            {
                conn = SQLiteImpl.Instance.GetConnection(ConnectionString);
            }

            return conn;
        }

        public List<TableColumn> GetTableColumns(string tableName)
        {
            List<TableColumn> list = null;

            if (DatabaseType == DatabaseType.MySql)
            {
                list = MySqlImpl.Instance.GetTableColumns(ConnectionString, tableName);
            }
            else if (DatabaseType == DatabaseType.SqlServer)
            {
                list = SqlServerImpl.Instance.GetTableColumns(ConnectionString, tableName);
            }
            else if (DatabaseType == DatabaseType.PostgreSql)
            {
                list = PostgreSqlImpl.Instance.GetTableColumns(ConnectionString, tableName);
            }
            else if (DatabaseType == DatabaseType.Oracle)
            {
                list = OracleImpl.Instance.GetTableColumns(ConnectionString, tableName);
            }
            else if (DatabaseType == DatabaseType.SQLite)
            {
                list = SQLiteImpl.Instance.GetTableColumns(ConnectionString, tableName);
            }

            return list;
        }

        public List<string> GetTableNames()
        {
            List<string> tableNames = null;

            if (DatabaseType == DatabaseType.MySql)
            {
                tableNames = MySqlImpl.Instance.GetTableNames(ConnectionString);
            }
            else if (DatabaseType == DatabaseType.SqlServer)
            {
                tableNames = SqlServerImpl.Instance.GetTableNames(ConnectionString);
            }
            else if (DatabaseType == DatabaseType.PostgreSql)
            {
                tableNames = PostgreSqlImpl.Instance.GetTableNames(ConnectionString);
            }
            else if (DatabaseType == DatabaseType.Oracle)
            {
                tableNames = OracleImpl.Instance.GetTableNames(ConnectionString);
            }
            else if (DatabaseType == DatabaseType.SQLite)
            {
                tableNames = SQLiteImpl.Instance.GetTableNames(ConnectionString);
            }

            return tableNames;
        }
    }
}
