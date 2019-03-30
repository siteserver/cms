using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using SqlKata.Compilers;

namespace Datory
{
    public static class DatorySql
    {
        private const int DefaultVarcharLength = 500;

        private static readonly ConcurrentDictionary<DatabaseType, bool> UseLegacyPagination = new ConcurrentDictionary<DatabaseType, bool>();

        private static bool IsUseLegacyPagination(DatabaseType databaseType, string connectionString)
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

                        useLegacyPagination = DatoryUtils.ToDecimal(version) < 11;
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

        public static Compiler GetCompiler(DatabaseType databaseType, string connectionString)
        {
            Compiler compiler = null;

            if (databaseType == DatabaseType.MySql)
            {
                compiler = new MySqlCompiler();
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                compiler = new SqlServerCompiler
                {
                    UseLegacyPagination = IsUseLegacyPagination(databaseType, connectionString)
                };
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                compiler = new PostgresCompiler();
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                compiler = new OracleCompiler
                {
                    UseLegacyPagination = IsUseLegacyPagination(databaseType, connectionString)
                };
            }

            return compiler;
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

        public static string ColumnIncrement(DatabaseType databaseType, string columnName, int plusNum = 1)
        {
            var retVal = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                retVal = $"IFNULL({columnName}, 0) + {plusNum}";
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                retVal = $"ISNULL({columnName}, 0) + {plusNum}";
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                retVal = $"COALESCE({columnName}, 0) + {plusNum}";
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                retVal = $"COALESCE({columnName}, 0) + {plusNum}";
            }

            return retVal;
        }

        public static string ColumnDecrement(DatabaseType databaseType, string columnName, int minusNum = 1)
        {
            var retVal = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                retVal = $"IFNULL({columnName}, 0) - {minusNum}";
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                retVal = $"ISNULL({columnName}, 0) - {minusNum}";
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                retVal = $"COALESCE({columnName}, 0) - {minusNum}";
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                retVal = $"COALESCE({columnName}, 0) - {minusNum}";
            }

            return retVal;
        }

        public static string GetSqlString(DatabaseType databaseType, string connectionString, string tableName, IList<string> columnNames = null, string whereSqlString = null, string orderSqlString = null, int offset = 0, int limit = 0, bool distinct = false)
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
                var useLegacyPagination = IsUseLegacyPagination(databaseType, connectionString);

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

        public static string GetConnectionStringUserId(string connectionString)
        {
            var userId = string.Empty;

            foreach (var pair in DatoryUtils.StringCollectionToStringList(connectionString, ';'))
            {
                if (!string.IsNullOrEmpty(pair) && pair.IndexOf("=", StringComparison.Ordinal) != -1)
                {
                    var key = pair.Substring(0, pair.IndexOf("=", StringComparison.Ordinal));
                    var value = pair.Substring(pair.IndexOf("=", StringComparison.Ordinal) + 1);
                    if (DatoryUtils.EqualsIgnoreCase(key, "Uid") ||
                        DatoryUtils.EqualsIgnoreCase(key, "Username") ||
                        DatoryUtils.EqualsIgnoreCase(key, "User ID"))
                    {
                        return value;
                    }
                }
            }

            return userId;
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
                        var sql = $"SELECT COUNT(*) FROM ALL_OBJECTS WHERE OBJECT_TYPE = 'TABLE' AND OWNER = '{GetConnectionStringUserId(connectionString).ToUpper()}' and OBJECT_NAME = '{tableName}'";

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

        public static bool DropTable(DatabaseType databaseType, string connectionString, string tableName)
        {
            using (var conn = GetConnection(databaseType, connectionString))
            {
                var sql = $"DROP TABLE {tableName}";

                conn.Execute(sql);
            }

            return true;
        }

        public static string GetAutoIncrementDataType(DatabaseType databaseType, bool alterTable = false)
        {
            var retVal = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                retVal = alterTable ? "INT AUTO_INCREMENT UNIQUE KEY" : "INT AUTO_INCREMENT";
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                retVal = "int IDENTITY (1, 1)";
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                retVal = "SERIAL";
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                retVal = "NUMBER GENERATED BY DEFAULT ON NULL AS IDENTITY";
            }

            return retVal;
        }

        private static string ToMySqlColumnString(DataType type, string attributeName, int length)
        {
            if (type == DataType.Boolean)
            {
                return $"`{attributeName}` tinyint(1)";
            }
            if (type == DataType.DateTime)
            {
                return $"`{attributeName}` datetime";
            }
            if (type == DataType.Decimal)
            {
                return $"`{attributeName}` decimal(18, 2)";
            }
            if (type == DataType.Integer)
            {
                return $"`{attributeName}` int";
            }
            if (type == DataType.Text)
            {
                return $"`{attributeName}` longtext";
            }
            return $"`{attributeName}` varchar({length})";
        }

        private static string ToSqlServerColumnString(DataType type, string attributeName, int length)
        {
            if (type == DataType.Boolean)
            {
                return $"[{attributeName}] [bit]";
            }
            if (type == DataType.DateTime)
            {
                return $"[{attributeName}] [datetime]";
            }
            if (type == DataType.Decimal)
            {
                return $"[{attributeName}] [decimal] (18, 2)";
            }
            if (type == DataType.Integer)
            {
                return $"[{attributeName}] [int]";
            }
            if (type == DataType.Text)
            {
                return $"[{attributeName}] [nvarchar] (max)";
            }
            return $"[{attributeName}] [nvarchar] ({length})";
        }

        private static string ToPostgreColumnString(DataType type, string attributeName, int length)
        {
            if (type == DataType.Boolean)
            {
                return $"{attributeName} bool";
            }
            if (type == DataType.DateTime)
            {
                return $"{attributeName} timestamptz";
            }
            if (type == DataType.Decimal)
            {
                return $"{attributeName} numeric(18, 2)";
            }
            if (type == DataType.Integer)
            {
                return $"{attributeName} int4";
            }
            if (type == DataType.Text)
            {
                return $"{attributeName} text";
            }
            return $"{attributeName} varchar({length})";
        }

        private static string ToOracleColumnString(DataType type, string attributeName, int length)
        {
            if (type == DataType.Boolean)
            {
                return $"{attributeName} number(1)";
            }
            if (type == DataType.DateTime)
            {
                return $"{attributeName} timestamp(6) with time zone";
            }
            if (type == DataType.Decimal)
            {
                return $"{attributeName} number(38, 2)";
            }
            if (type == DataType.Integer)
            {
                return $"{attributeName} number";
            }
            if (type == DataType.Text)
            {
                return $"{attributeName} nclob";
            }
            return $"{attributeName} nvarchar2({length})";
        }

        public static string GetColumnSqlString(DatabaseType databaseType, DatoryColumn tableColumn)
        {
            if (tableColumn.IsIdentity)
            {
                return $@"{tableColumn.AttributeName} {GetAutoIncrementDataType(databaseType)}";
            }

            if (databaseType == DatabaseType.MySql)
            {
                return ToMySqlColumnString(tableColumn.DataType, tableColumn.AttributeName, tableColumn.DataLength);
            }

            if (databaseType == DatabaseType.SqlServer)
            {
                return ToSqlServerColumnString(tableColumn.DataType, tableColumn.AttributeName, tableColumn.DataLength);
            }

            if (databaseType == DatabaseType.PostgreSql)
            {
                return ToPostgreColumnString(tableColumn.DataType, tableColumn.AttributeName, tableColumn.DataLength);
            }

            if (databaseType == DatabaseType.Oracle)
            {
                return ToOracleColumnString(tableColumn.DataType, tableColumn.AttributeName, tableColumn.DataLength);
            }

            return string.Empty;
        }

        public static string GetPrimaryKeySqlString(DatabaseType databaseType, string tableName, string attributeName)
        {
            return databaseType == DatabaseType.MySql
                ? $@"PRIMARY KEY ({attributeName})"
                : $@"CONSTRAINT PK_{tableName}_{attributeName} PRIMARY KEY ({attributeName})";
        }

        public static string GetQuotedIdentifier(DatabaseType databaseType, string identifier)
        {
            if (databaseType == DatabaseType.MySql)
            {
                return $"`{identifier}`";
            }

            return databaseType == DatabaseType.SqlServer ? $"[{identifier}]" : identifier;
        }

        public static bool CreateTable(DatabaseType databaseType, string connectionString, string tableName, List<DatoryColumn> tableColumns, out Exception ex, out string sqlString)
        {
            ex = null;
            sqlString = string.Empty;

            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@"CREATE TABLE {tableName} (").AppendLine();

            var primaryKeyColumns = new List<DatoryColumn>();
            DatoryColumn identityColumn = null;

            foreach (var tableColumn in tableColumns)
            {
                if (DatoryUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(DynamicEntity.Id)))
                {
                    tableColumn.DataType = DataType.Integer;
                    tableColumn.IsIdentity = true;
                    tableColumn.IsPrimaryKey = true;
                }
                else if (DatoryUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(DynamicEntity.Guid)))
                {
                    tableColumn.DataType = DataType.VarChar;
                    tableColumn.DataLength = 50;
                }
                else if (DatoryUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(DynamicEntity.LastModifiedDate)))
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
                    tableColumn.DataLength = DefaultVarcharLength;
                }

                var columnSql = GetColumnSqlString(databaseType, tableColumn);
                if (!string.IsNullOrEmpty(columnSql))
                {
                    sqlBuilder.Append(columnSql).Append(",");
                }
            }

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

            sqlBuilder.Length--;

            sqlBuilder.AppendLine().Append(databaseType == DatabaseType.MySql
                ? ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4"
                : ")");


            try
            {
                using (var conn = GetConnection(databaseType, connectionString))
                {
                    conn.Execute(sqlBuilder.ToString());
                }
            }
            catch (Exception e)
            {
                ex = e;
                return false;
            }

            return true;
        }

        private static string GetValueFromConnectionString(string connectionString, string attribute)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(attribute))
            {
                var pairs = connectionString.Split(';');
                foreach (var pair in pairs)
                {
                    if (pair.IndexOf("=", StringComparison.Ordinal) != -1)
                    {
                        if (DatoryUtils.EqualsIgnoreCase(attribute, pair.Trim().Split('=')[0]))
                        {
                            retVal = pair.Trim().Split('=')[1];
                            break;
                        }
                    }
                }
            }
            return retVal;
        }

        public static string GetDatabaseNameFormConnectionString(DatabaseType databaseType, string connectionString)
        {
            if (databaseType == DatabaseType.Oracle)
            {
                var index1 = connectionString.IndexOf("SERVICE_NAME=", StringComparison.Ordinal);
                var index2 = connectionString.IndexOf(")));", StringComparison.Ordinal);
                return connectionString.Substring(index1 + 13, index2 - index1 - 13);
            }
            return GetValueFromConnectionString(connectionString, "Database");
        }

        public static DataType ToMySqlDataType(string dataTypeStr)
        {
            if (string.IsNullOrEmpty(dataTypeStr)) return DataType.VarChar;

            var dataType = DataType.VarChar;

            dataTypeStr = DatoryUtils.TrimAndToLower(dataTypeStr);
            switch (dataTypeStr)
            {
                case "bit":
                    dataType = DataType.Boolean;
                    break;
                case "datetime":
                    dataType = DataType.DateTime;
                    break;
                case "decimal":
                    dataType = DataType.Decimal;
                    break;
                case "int":
                    dataType = DataType.Integer;
                    break;
                case "longtext":
                    dataType = DataType.Text;
                    break;
                case "nvarchar":
                    dataType = DataType.VarChar;
                    break;
                case "text":
                    dataType = DataType.Text;
                    break;
                case "varchar":
                    dataType = DataType.VarChar;
                    break;
            }

            return dataType;
        }

        public static DataType ToSqlServerDataType(string dataTypeStr, string dataLength)
        {
            if (string.IsNullOrEmpty(dataTypeStr)) return DataType.VarChar;

            var dataType = DataType.VarChar;

            dataTypeStr = DatoryUtils.TrimAndToLower(dataTypeStr);
            dataLength = DatoryUtils.TrimAndToLower(dataLength);
            switch (dataTypeStr)
            {
                case "bit":
                    dataType = DataType.Boolean;
                    break;
                case "datetime":
                    dataType = DataType.DateTime;
                    break;
                case "datetime2":
                    dataType = DataType.DateTime;
                    break;
                case "datetimeoffset":
                    dataType = DataType.DateTime;
                    break;
                case "decimal":
                    dataType = DataType.Decimal;
                    break;
                case "int":
                    dataType = DataType.Integer;
                    break;
                case "ntext":
                    dataType = DataType.Text;
                    break;
                case "nvarchar":
                    dataType = dataLength == "max" ? DataType.Text : DataType.VarChar;
                    break;
                case "text":
                    dataType = DataType.Text;
                    break;
                case "varchar":
                    dataType = DataType.VarChar;
                    break;
            }

            return dataType;
        }

        public static DataType ToPostgreSqlDataType(string dataTypeStr)
        {
            if (string.IsNullOrEmpty(dataTypeStr)) return DataType.VarChar;

            var dataType = DataType.VarChar;

            dataTypeStr = DatoryUtils.TrimAndToLower(dataTypeStr);
            switch (dataTypeStr)
            {
                case "varchar":
                    dataType = DataType.VarChar;
                    break;
                case "bool":
                    dataType = DataType.Boolean;
                    break;
                case "timestamptz":
                    dataType = DataType.DateTime;
                    break;
                case "numeric":
                    dataType = DataType.Decimal;
                    break;
                case "int4":
                    dataType = DataType.Integer;
                    break;
                case "text":
                    dataType = DataType.Text;
                    break;
            }

            return dataType;
        }

        public static DataType ToOracleDataType(string dataTypeStr)
        {
            if (string.IsNullOrEmpty(dataTypeStr)) return DataType.VarChar;

            var dataType = DataType.VarChar;

            dataTypeStr = DatoryUtils.TrimAndToUpper(dataTypeStr);
            if (dataTypeStr.StartsWith("TIMESTAMP("))
            {
                dataType = DataType.DateTime;
            }
            else if (dataTypeStr == "NUMBER")
            {
                dataType = DataType.Integer;
            }
            else if (dataTypeStr == "NCLOB")
            {
                dataType = DataType.Text;
            }
            else if (dataTypeStr == "NVARCHAR2")
            {
                dataType = DataType.VarChar;
            }
            else if (dataTypeStr == "CLOB")
            {
                dataType = DataType.Text;
            }
            else if (dataTypeStr == "VARCHAR2")
            {
                dataType = DataType.VarChar;
            }

            return dataType;
        }

        private static List<DatoryColumn> GetOracleColumns(DatabaseType databaseType, string connectionString, string tableName)
        {
            var owner = GetConnectionStringUserId(connectionString).ToUpper();
            tableName = tableName.ToUpper();

            var list = new List<DatoryColumn>();
            var sqlString =
                $"SELECT COLUMN_NAME AS columnName, DATA_TYPE AS DataType, DATA_PRECISION AS DataPrecision, DATA_SCALE AS DataScale, CHAR_LENGTH AS CharLength, DATA_DEFAULT AS DataDefault FROM all_tab_cols WHERE OWNER = '{owner}' and table_name = '{tableName}' and user_generated = 'YES' ORDER BY COLUMN_ID";

            IEnumerable<dynamic> columns;
            using (var conn = GetConnection(databaseType, connectionString))
            {
                columns = conn.Query<dynamic>(sqlString);
            }

            foreach (var column in columns)
            {
                var columnName = column.columnName;
                var dataType = ToOracleDataType(column.DataType);
                var percision = column.DataPrecision;
                var scale = column.DataScale;
                var charLength = column.CharLength;
                var dataDefault = column.DataDefault;

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

                var info = new DatoryColumn
                {
                    AttributeName = columnName,
                    DataType = dataType,
                    DataLength = charLength,
                    IsPrimaryKey = false,
                    IsIdentity = isIdentity
                };
                list.Add(info);
            }

            sqlString =
                $@"select distinct cu.column_name from all_cons_columns cu inner join all_constraints au 
on cu.constraint_name = au.constraint_name
and au.constraint_type = 'P' and cu.OWNER = '{owner}' and cu.table_name = '{tableName}'";

            IEnumerable<string> columnNames;
            using (var conn = GetConnection(databaseType, connectionString))
            {
                columnNames = conn.Query<string>(sqlString);
            }

            foreach (var columnName in columnNames)
            {
                foreach (var tableColumnInfo in list)
                {
                    if (columnName != tableColumnInfo.AttributeName) continue;
                    tableColumnInfo.IsPrimaryKey = true;
                    break;
                }
            }

            return list;
        }

        private static List<DatoryColumn> GetPostgreSqlColumns(DatabaseType databaseType, string connectionString, string databaseName, string tableName)
        {
            var list = new List<DatoryColumn>();
            var sqlString =
                $"SELECT COLUMN_NAME AS ColumnName, UDT_NAME AS UdtName, CHARACTER_MAXIMUM_LENGTH AS CharacterMaximumLength, COLUMN_DEFAULT AS ColumnDefault FROM information_schema.columns WHERE table_catalog = '{databaseName}' AND table_name = '{tableName.ToLower()}' ORDER BY ordinal_position";

            IEnumerable<dynamic> columns;
            using (var conn = GetConnection(databaseType, connectionString))
            {
                columns = conn.Query<dynamic>(sqlString);
            }

            foreach (var column in columns)
            {
                var columnName = column.ColumnName;
                var udtName = column.UdtName;
                var characterMaximumLength = column.CharacterMaximumLength;
                var columnDefault = column.ColumnDefault;

                var dataType = ToPostgreSqlDataType(udtName);
                var length = characterMaximumLength;

                var isIdentity = columnDefault.StartsWith("nextval(");

                var info = new DatoryColumn
                {
                    AttributeName = columnName,
                    DataType = dataType,
                    DataLength = length,
                    IsPrimaryKey = false,
                    IsIdentity = isIdentity
                };
                list.Add(info);
            }

            sqlString =
                $"select column_name AS ColumnName, constraint_name AS ConstraintName from information_schema.key_column_usage where table_catalog = '{databaseName}' and table_name = '{tableName.ToLower()}';";

            IEnumerable<dynamic> rows;
            using (var conn = GetConnection(databaseType, connectionString))
            {
                rows = conn.Query<dynamic>(sqlString);
            }

            foreach (var row in rows)
            {
                var columnName = row.ColumnName;
                var constraintName = row.ConstraintName;

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

            return list;
        }

        private static List<DatoryColumn> GetSqlServerColumns(DatabaseType databaseType, string connectionString, string databaseName, string tableName)
        {
            string tableId;

            var sqlString =
                $"select id from [{databaseName}]..sysobjects where type = 'U' and category<>2 and name='{tableName}'";

            using (var conn = GetConnection(databaseType, connectionString))
            {
                tableId = conn.QueryFirstOrDefault<string>(sqlString);
            }

            if (string.IsNullOrEmpty(tableId)) return new List<DatoryColumn>();

            var list = new List<DatoryColumn>();
            var isIdentityExist = false;

            sqlString =
                $"select C.name AS ColumnName, T.name AS DataTypeName, C.length AS Length, C.colstat AS IsPrimaryKeyInt, case when C.autoval is null then 0 else 1 end AS IsIdentityInt from [{databaseName}]..systypes T, [{databaseName}]..syscolumns C where C.id={tableId} and C.xtype=T.xusertype order by C.colid";

            IEnumerable<dynamic> columns;
            using (var conn = GetConnection(databaseType, connectionString))
            {
                columns = conn.Query<dynamic>(sqlString);
            }

            foreach (var column in columns)
            {
                var columnName = column.ColumnName;
                if (columnName == "msrepl_tran_version")
                {
                    continue;
                }

                var dataTypeName = column.DataTypeName;
                var length = column.Length;
                var dataType = ToSqlServerDataType(dataTypeName, Convert.ToString(length));
                length = dataType == DataType.VarChar ? length : 0;
                var isPrimaryKeyInt = column.IsPrimaryKeyInt;
                var isIdentityInt = column.IsIdentityInt;

                var isPrimaryKey = isPrimaryKeyInt == 1;
                //var isIdentity = isIdentityInt == 1 || StringUtils.EqualsIgnoreCase(columnName, "Id");
                var isIdentity = isIdentityInt == 1;
                if (isIdentity)
                {
                    isIdentityExist = true;
                }

                var info = new DatoryColumn
                {
                    AttributeName = columnName,
                    DataType = dataType,
                    DataLength = length,
                    IsPrimaryKey = isPrimaryKey,
                    IsIdentity = isIdentity
                };
                list.Add(info);
            }

            if (!isIdentityExist)
            {
                string clName;

                sqlString = $"select name from syscolumns where id = object_id(N'{tableName}') and COLUMNPROPERTY(id, name,'IsIdentity')= 1";

                using (var conn = GetConnection(databaseType, connectionString))
                {
                    clName = conn.QueryFirstOrDefault<string>(sqlString);
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

        private static List<DatoryColumn> GetMySqlColumns(DatabaseType databaseType, string connectionString, string databaseName, string tableName)
        {
            var list = new List<DatoryColumn>();

            var sqlString =
                $"select COLUMN_NAME AS ColumnName, DATA_TYPE AS DataType, CHARACTER_MAXIMUM_LENGTH AS DataLength, COLUMN_KEY AS ColumnKey, EXTRA AS Extra from information_schema.columns where table_schema = '{databaseName}' and table_name = '{tableName}' order by table_name,ordinal_position; ";

            IEnumerable<dynamic> columns;

            using (var conn = GetConnection(databaseType, connectionString))
            {
                columns = conn.Query<dynamic>(sqlString);
            }

            foreach (var column in columns)
            {
                var columnName = column.ColumnName;
                var dataType = ToMySqlDataType(column.DataType);
                var dataLength = column.DataLength;

                var length = dataLength == null || dataType == DataType.Text ? 0 : dataLength;

                var isPrimaryKey = Convert.ToString(column.ColumnKey) == "PRI";
                var isIdentity = Convert.ToString(column.Extra) == "auto_increment";

                var info = new DatoryColumn
                {
                    AttributeName = columnName,
                    DataType = dataType,
                    DataLength = length,
                    IsPrimaryKey = isPrimaryKey,
                    IsIdentity = isIdentity
                };
                list.Add(info);

            }

            return list;
        }

        public static List<string> GetTableColumnNames(DatabaseType databaseType, string connectionString, string tableName)
        {
            var allTableColumnInfoList = GetTableColumns(databaseType, connectionString, tableName);
            return allTableColumnInfoList.Select(tableColumnInfo => tableColumnInfo.AttributeName).ToList();
        }

        public static List<DatoryColumn> GetTableColumns(DatabaseType databaseType, string connectionString, string tableName)
        {
            var databaseName = GetDatabaseNameFormConnectionString(databaseType, connectionString);

            List<DatoryColumn> list = null;

            if (databaseType == DatabaseType.MySql)
            {
                list = GetMySqlColumns(databaseType, connectionString, databaseName, tableName);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                list = GetSqlServerColumns(databaseType, connectionString, databaseName, tableName);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                list = GetPostgreSqlColumns(databaseType, connectionString, databaseName, tableName);
            }
            else if (databaseType == DatabaseType.Oracle)
            {
                list = GetOracleColumns(databaseType, connectionString, tableName);
            }

            return list;
        }

        public static List<string> GetTableNames(DatabaseType databaseType, string connectionString)
        {
            var sqlString = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                var databaseName = GetDatabaseNameFormConnectionString(databaseType, connectionString);
                sqlString = $"SELECT table_name FROM information_schema.tables WHERE table_schema='{databaseName}' ORDER BY table_name";
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                var databaseName = GetDatabaseNameFormConnectionString(databaseType, connectionString);
                sqlString =
                    $"SELECT name FROM [{databaseName}]..sysobjects WHERE type = 'U' AND category<>2 ORDER BY Name";
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                var databaseName = GetDatabaseNameFormConnectionString(databaseType, connectionString);
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

        public static string GetAddColumnsSqlString(DatabaseType databaseType, string tableName, string columnsSqlString)
        {
            var columns = databaseType == DatabaseType.MySql ? $"({columnsSqlString})" : columnsSqlString;

            return $"ALTER TABLE {GetQuotedIdentifier(databaseType, tableName)} ADD " + columns;
        }

        public static string GetDropColumnsSqlString(DatabaseType databaseType, string tableName, string columnName)
        {
            return $"ALTER TABLE {GetQuotedIdentifier(databaseType, tableName)} DROP COLUMN {GetQuotedIdentifier(databaseType, columnName)}";
        }

        public static bool AlterTable(DatabaseType databaseType, string connectionString, string tableName, IList<DatoryColumn> tableColumns, IList<string> dropColumnNames, out Exception ex)
        {
            ex = null;
            var list = new List<string>();

            var columnNameList = GetTableColumnNames(databaseType, connectionString, tableName);
            foreach (var tableColumn in tableColumns)
            {
                if (!DatoryUtils.ContainsIgnoreCase(columnNameList, tableColumn.AttributeName))
                {
                    list.Add(GetAddColumnsSqlString(databaseType, tableName, GetColumnSqlString(databaseType, tableColumn)));
                }
            }

            if (dropColumnNames != null)
            {
                foreach (var columnName in columnNameList)
                {
                    if (DatoryUtils.ContainsIgnoreCase(dropColumnNames, columnName))
                    {
                        list.Add(GetDropColumnsSqlString(databaseType, tableName, columnName));
                    }
                }
            }

            if (list.Count <= 0) return true;

            var isAltered = false;

            try
            {
                foreach (var sqlString in list)
                {
                    using (var conn = GetConnection(databaseType, connectionString))
                    {
                        conn.Execute(sqlString);
                    }
                }
                isAltered = true;
            }
            catch (Exception e)
            {
                ex = e;
            }

            return isAltered;
        }

        public static bool DropTable(DatabaseType databaseType, string connectionString, string tableName, out Exception ex)
        {
            ex = null;
            var isAltered = false;

            try
            {
                using (var conn = GetConnection(databaseType, connectionString))
                {
                    conn.Execute($"DROP TABLE {tableName}");
                }

                isAltered = true;
            }
            catch (Exception e)
            {
                ex = e;
            }

            return isAltered;
        }
    }
}
