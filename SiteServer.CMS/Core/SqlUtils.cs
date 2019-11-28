using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using Datory;
using MySql.Data.MySqlClient;
using Npgsql;
using NpgsqlTypes;
using Oracle.ManagedDataAccess.Client;

namespace SiteServer.Utils
{
    public static class SqlUtils
    {
        public const string Asterisk = "*";
        public const string OracleEmptyValue = "_EMPTY_";

        public static IDbConnection GetIDbConnection(DatabaseType databaseType, string connectionString)
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

        public static IDbCommand GetIDbCommand()
        {
            IDbCommand command = null;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                command = new MySqlCommand();
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                command = new SqlCommand();
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                command = new NpgsqlCommand();
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                command = new OracleCommand();
            }

            return command;
        }

        public static IDbDataAdapter GetIDbDataAdapter()
        {
            IDbDataAdapter adapter = null;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                adapter = new MySqlDataAdapter();
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                adapter = new SqlDataAdapter();
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                adapter = new NpgsqlDataAdapter();
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                adapter = new OracleDataAdapter();
            }

            return adapter;
        }

        public static void FillDataAdapterWithDataTable(IDbDataAdapter adapter, DataTable table)
        {
            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                ((MySqlDataAdapter)adapter).Fill(table);
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                ((SqlDataAdapter)adapter).Fill(table);
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                ((NpgsqlDataAdapter)adapter).Fill(table);
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                ((OracleDataAdapter)adapter).Fill(table);
            }
        }

        public static IDbDataParameter GetIDbDataParameter(string parameterName, DataType dataType, int size, object value)
        {
            IDbDataParameter parameter = null;

            if (size == 0)
            {
                if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
                {
                    parameter = new MySqlParameter(parameterName, ToMySqlDbType(dataType))
                    {
                        Value = value
                    };
                }
                else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
                {
                    parameter = new SqlParameter(parameterName, ToSqlServerDbType(dataType))
                    {
                        Value = value
                    };
                }
                else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
                {
                    parameter = new NpgsqlParameter(parameterName, ToNpgsqlDbType(dataType))
                    {
                        Value = value
                    };
                }
                else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
                {
                    parameter = new OracleParameter(parameterName, ToOracleDbType(dataType))
                    {
                        Value = ToOracleDbValue(dataType, value)
                    };
                }
            }
            else
            {
                if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
                {
                    parameter = new MySqlParameter(parameterName, ToMySqlDbType(dataType), size)
                    {
                        Value = value
                    };
                }
                else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
                {
                    parameter = new SqlParameter(parameterName, ToSqlServerDbType(dataType), size)
                    {
                        Value = value
                    };
                }
                else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
                {
                    parameter = new NpgsqlParameter(parameterName, ToNpgsqlDbType(dataType), size)
                    {
                        Value = value
                    };
                }
                else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
                {
                    parameter = new OracleParameter(parameterName, ToOracleDbType(dataType), size)
                    {
                        Value = ToOracleDbValue(dataType, value)
                    };
                }
            }

            return parameter;
        }

        public static string GetSqlColumnInList(string columnName, List<int> idList)
        {
            if (idList == null || idList.Count == 0) return string.Empty;

            if (idList.Count < 1000)
            {
                return $"{columnName} IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";
            }

            var sql = new StringBuilder();
            sql.Append(" ").Append(columnName).Append(" IN ( ");
            for (var i = 0; i < idList.Count; i++)
            {
                sql.Append(idList[i] + ",");
                if ((i + 1) % 1000 == 0 && i + 1 < idList.Count)
                {
                    sql.Length -= 1;
                    sql.Append(" ) OR ").Append(columnName).Append(" IN (");
                }
            }
            sql.Length -= 1;
            sql.Append(" )");

            return $"({sql})";
        }

        public static string GetInStr(string columnName, string inStr)
        {
            var retVal = string.Empty;
            inStr = AttackUtils.FilterSql(inStr);

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"INSTR({columnName}, '{inStr}') > 0";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"CHARINDEX('{inStr}', {columnName}) > 0";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"POSITION('{inStr}' IN {columnName}) > 0";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $"INSTR({columnName}, '{inStr}') > 0";
            }

            return retVal;
        }

        public static KeyValuePair<string, IDataParameter> GetInStrWithParameter(string columnName, string inStr)
        {
            
            var parameterName = $"P{StringUtils.GetShortGuid()}";
            var sqlString = string.Empty;
            var parameter = GetIDbDataParameter(parameterName, DataType.VarChar, 0, inStr);

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                sqlString = $"INSTR({columnName}, @{parameterName}) > 0";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                sqlString = $"CHARINDEX(@{parameterName}, {columnName}) > 0";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                sqlString = $"POSITION(@{parameterName} IN {columnName}) > 0";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                sqlString = $"INSTR({columnName}, @{parameterName}) > 0";
            }

            return new KeyValuePair<string, IDataParameter>(sqlString, parameter);
        }

        public static string GetInStrReverse(string inStr, string columnName)
        {
            var retVal = string.Empty;
            inStr = AttackUtils.FilterSql(inStr);

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"INSTR('{inStr}', {columnName}) > 0";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"CHARINDEX({columnName}, '{inStr}') > 0";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"POSITION({columnName} IN '{inStr}') > 0";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $"INSTR('{inStr}', {columnName}) > 0";
            }

            return retVal;
        }

        public static string GetNotInStr(string columnName, string inStr)
        {
            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"INSTR({columnName}, '{inStr}') = 0";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"CHARINDEX('{inStr}', {columnName}) = 0";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"POSITION('{inStr}' IN {columnName}) = 0";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $"INSTR({columnName}, '{inStr}') = 0";
            }

            return retVal;
        }

        public static string ToTopSqlString(string tableName, string columns, string whereString, string orderString, int topN)
        {
            string retVal = $"SELECT {columns} FROM {tableName} {whereString} {orderString}";
            if (topN <= 0) return retVal;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"SELECT {columns} FROM {tableName} {whereString} {orderString} LIMIT {topN}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"SELECT TOP {topN} {columns} FROM {tableName} {whereString} {orderString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"SELECT {columns} FROM {tableName} {whereString} {orderString} LIMIT {topN}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $@"SELECT {columns} FROM {tableName} {whereString} {orderString} FETCH FIRST {topN} ROWS ONLY";
            }

            return retVal;
        }

        public static string ToTopSqlString(string sqlString, string orderString, int topN)
        {
            string retVal = $"SELECT * FROM ({sqlString}) {orderString}";
            if (topN <= 0) return retVal;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"SELECT * FROM ({sqlString}) {orderString} LIMIT {topN}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"SELECT TOP {topN} * FROM ({sqlString}) {orderString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"SELECT * FROM ({sqlString}) {orderString} LIMIT {topN}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $@"SELECT * FROM ({sqlString}) {orderString} FETCH FIRST {topN} ROWS ONLY";
            }

            return retVal;
        }

        public static string GetPageSqlString(string sqlString, string orderString, int itemsPerPage, int currentPageIndex, int pageCount, int recordsInLastPage)
        {
            var retVal = string.Empty;

            var recsToRetrieve = itemsPerPage;
            if (currentPageIndex == pageCount - 1)
            {
                recsToRetrieve = recordsInLastPage;
            }

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
    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
) AS t2 {orderString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $@"
SELECT * FROM (
    SELECT TOP {recsToRetrieve} * FROM (
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
    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
) AS t2 {orderString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) {orderString} FETCH FIRST {itemsPerPage * (currentPageIndex + 1)} ROWS ONLY
    ) {orderStringReverse} FETCH FIRST {recsToRetrieve} ROWS ONLY
) {orderString}";
            }

            return retVal;
        }

        public static string GetDistinctTopSqlString(string tableName, string columns, string whereString, string orderString, int topN)
        {
            var retVal = $"SELECT DISTINCT {columns} FROM {tableName} {whereString} {orderString}";
            if (topN <= 0) return retVal;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"SELECT DISTINCT {columns} FROM {tableName} {whereString} {orderString} LIMIT {topN}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"SELECT DISTINCT TOP {topN} {columns} FROM {tableName} {whereString} {orderString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"SELECT DISTINCT {columns} FROM {tableName} {whereString} {orderString} LIMIT {topN}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $"SELECT DISTINCT {columns} FROM {tableName} {whereString} {orderString} FETCH FIRST {topN} ROWS ONLY";
            }

            return retVal;
        }

        public static string ToInTopSqlString(string tableName, string columns, string whereString, string orderString, int topN)
        {
            var builder = new StringBuilder();
            if (WebConfigUtils.DatabaseType != DatabaseType.Oracle)
            {
                foreach (var column in TranslateUtils.StringCollectionToStringList(columns))
                {
                    builder.Append($"T.{column}, ");
                }
                builder.Length = builder.Length - 2;
                return
                    $"SELECT {builder} FROM ({ToTopSqlString(tableName, columns, whereString, orderString, topN)}) AS T";
            }

            foreach (var column in TranslateUtils.StringCollectionToStringList(columns))
            {
                builder.Append($"{column}, ");
            }
            builder.Length = builder.Length - 2;
            return
                $"SELECT {builder} FROM ({ToTopSqlString(tableName, columns, whereString, orderString, topN)})";
        }

        public static string GetQuotedIdentifier(string identifier)
        {
            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                return $"`{identifier}`";
            }

            return WebConfigUtils.DatabaseType == DatabaseType.SqlServer ? $"[{identifier}]" : identifier;
        }

        public static string GetColumnSqlString(TableColumn tableColumn)
        {
            return $@"{GetQuotedIdentifier(tableColumn.AttributeName)} {GetColumnTypeString(tableColumn)}";
        }

        public static string GetColumnTypeString(TableColumn tableColumn)
        {
            if (tableColumn.IsIdentity)
            {
                return GetAutoIncrementDataType();
            }

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                return ToMySqlColumnTypeString(tableColumn.DataType, tableColumn.DataLength);
            }

            if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                return ToSqlServerColumnTypeString(tableColumn.DataType, tableColumn.DataLength);
            }

            if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                return ToPostgreColumnTypeString(tableColumn.DataType, tableColumn.DataLength);
            }

            if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                return ToOracleColumnTypeString(tableColumn.DataType, tableColumn.DataLength);
            }

            return string.Empty;
        }

        public static string GetPrimaryKeySqlString(string tableName, string attributeName)
        {
            return WebConfigUtils.DatabaseType == DatabaseType.MySql
                ? $@"PRIMARY KEY ({attributeName})"
                : $@"CONSTRAINT PK_{tableName}_{attributeName} PRIMARY KEY ({attributeName})";
        }

        //public static string GetColumnSqlString(DataType dataType, string attributeName, int length)
        //{
        //    var retVal = string.Empty;

        //    if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
        //    {
        //        retVal = ToMySqlColumnString(dataType, attributeName, length);
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
        //    {
        //        retVal = ToSqlServerColumnString(dataType, attributeName, length);
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
        //    {
        //        retVal = ToPostgreColumnString(dataType, attributeName, length);
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
        //    {
        //        retVal = ToOracleColumnString(dataType, attributeName, length);
        //    }

        //    return retVal;
        //}

        public static string GetAddColumnsSqlString(string tableName, string columnsSqlString)
        {
            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"ALTER TABLE `{tableName}` ADD ({columnsSqlString})";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"ALTER TABLE [{tableName}] ADD {columnsSqlString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"ALTER TABLE {tableName} ADD {columnsSqlString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $"ALTER TABLE {tableName} ADD {columnsSqlString}";
            }

            return retVal;
        }

        public static string GetModifyColumnsSqlString(string tableName, string columnName, string columnTypeString)
        {
            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"ALTER TABLE `{tableName}` MODIFY {columnName} {columnTypeString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"ALTER TABLE [{tableName}] ALTER COLUMN {columnName} {columnTypeString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"ALTER TABLE {tableName} ALTER COLUMN {columnName} TYPE {columnTypeString}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $"ALTER TABLE {tableName} MODIFY {columnName} {columnTypeString}";
            }

            return retVal;
        }

        public static string GetDropColumnsSqlString(string tableName, string columnName)
        {
            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"ALTER TABLE `{tableName}` DROP COLUMN `{columnName}`";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"ALTER TABLE [{tableName}] DROP COLUMN [{columnName}]";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"ALTER TABLE {tableName} DROP COLUMN {columnName}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $"ALTER TABLE {tableName} DROP COLUMN {columnName}";
            }

            return retVal;
        }

        public static string GetAutoIncrementDataType(bool alterTable = false)
        {
            var retVal = string.Empty;

            //if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            //{
            //    retVal = "INT AUTO_INCREMENT PRIMARY KEY";
            //}
            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = alterTable ? "INT AUTO_INCREMENT UNIQUE KEY" : "INT AUTO_INCREMENT";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = "int IDENTITY (1, 1)";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = "SERIAL";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = "NUMBER GENERATED BY DEFAULT ON NULL AS IDENTITY";
            }

            return retVal;
        }

        public static DataType ToDataType(string dataTypeStr)
        {
            if (string.IsNullOrEmpty(dataTypeStr)) return DataType.VarChar;

            var dataType = DataType.VarChar;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                dataTypeStr = dataTypeStr.ToLower().Trim();
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
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                dataTypeStr = dataTypeStr.ToLower().Trim();
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
                        dataType = DataType.VarChar;
                        break;
                    case "text":
                        dataType = DataType.Text;
                        break;
                    case "varchar":
                        dataType = DataType.VarChar;
                        break;
                }
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                dataTypeStr = dataTypeStr.ToLower().Trim();
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
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                dataTypeStr = dataTypeStr.ToUpper().Trim();
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
            }

            return dataType;
        }

        private static SqlDbType ToSqlServerDbType(DataType type)
        {
            if (type == DataType.Boolean)
            {
                return SqlDbType.Bit;
            }
            if (type == DataType.DateTime)
            {
                return SqlDbType.DateTime;
            }
            if (type == DataType.Decimal)
            {
                return SqlDbType.Decimal;
            }
            if (type == DataType.Integer)
            {
                return SqlDbType.Int;
            }
            if (type == DataType.Text)
            {
                return SqlDbType.NText;
            }
            if (type == DataType.VarChar)
            {
                return SqlDbType.NVarChar;
            }
            return SqlDbType.VarChar;
        }

        private static MySqlDbType ToMySqlDbType(DataType type)
        {
            if (type == DataType.Boolean)
            {
                return MySqlDbType.Bit;
            }
            if (type == DataType.DateTime)
            {
                return MySqlDbType.DateTime;
            }
            if (type == DataType.Decimal)
            {
                return MySqlDbType.Decimal;
            }
            if (type == DataType.Integer)
            {
                return MySqlDbType.Int32;
            }
            if (type == DataType.Text)
            {
                return MySqlDbType.LongText;
            }
            if (type == DataType.VarChar)
            {
                return MySqlDbType.VarString;
            }

            return MySqlDbType.VarString;
        }

        private static NpgsqlDbType ToNpgsqlDbType(DataType type)
        {
            if (type == DataType.Boolean)
            {
                return NpgsqlDbType.Boolean;
            }
            if (type == DataType.DateTime)
            {
                return NpgsqlDbType.TimestampTz;
            }
            if (type == DataType.Decimal)
            {
                return NpgsqlDbType.Numeric;
            }
            if (type == DataType.Integer)
            {
                return NpgsqlDbType.Integer;
            }
            if (type == DataType.Text)
            {
                return NpgsqlDbType.Text;
            }
            return NpgsqlDbType.Varchar;
        }

        public static object ToOracleDbValue(DataType dataType, object value)
        {
            // Oracle internally changes empty string to NULL values. Oracle simply won't let insert an empty string. So we replace string.Empty value to placeholder _EMPTY_
            if ((dataType == DataType.Text || dataType == DataType.VarChar) && value != null && value.ToString() == string.Empty)
            {
                return OracleEmptyValue;
            }
            return value;
        }

        private static OracleDbType ToOracleDbType(DataType type)
        {
            if (type == DataType.Boolean)
            {
                return OracleDbType.Int32;
            }
            if (type == DataType.DateTime)
            {
                return OracleDbType.TimeStampTZ;
            }
            if (type == DataType.Decimal)
            {
                return OracleDbType.Decimal;
            }
            if (type == DataType.Integer)
            {
                return OracleDbType.Int32;
            }
            if (type == DataType.Text)
            {
                return OracleDbType.NClob;
            }
            return OracleDbType.NVarchar2;
        }

        private static string ToMySqlColumnTypeString(DataType type, int length)
        {
            if (type == DataType.Boolean)
            {
                return "tinyint(1)";
            }
            if (type == DataType.DateTime)
            {
                return "datetime";
            }
            if (type == DataType.Decimal)
            {
                return "decimal(18, 2)";
            }
            if (type == DataType.Integer)
            {
                return "int";
            }
            if (type == DataType.Text)
            {
                return "longtext";
            }
            return $"varchar({length})";
        }

        private static string ToSqlServerColumnTypeString(DataType type, int length)
        {
            if (type == DataType.Boolean)
            {
                return "[bit]";
            }
            if (type == DataType.DateTime)
            {
                return "[datetime]";
            }
            if (type == DataType.Decimal)
            {
                return "[decimal] (18, 2)";
            }
            if (type == DataType.Integer)
            {
                return "[int]";
            }
            if (type == DataType.Text)
            {
                return "[ntext]";
            }
            return $"[nvarchar] ({length})";
        }

        private static string ToPostgreColumnTypeString(DataType type, int length)
        {
            if (type == DataType.Boolean)
            {
                return "bool";
            }
            if (type == DataType.DateTime)
            {
                return "timestamptz";
            }
            if (type == DataType.Decimal)
            {
                return "numeric(18, 2)";
            }
            if (type == DataType.Integer)
            {
                return "int4";
            }
            if (type == DataType.Text)
            {
                return "text";
            }
            return $"varchar({length})";
        }

        private static string ToOracleColumnTypeString(DataType type, int length)
        {
            if (type == DataType.Boolean)
            {
                return "number(1)";
            }
            if (type == DataType.DateTime)
            {
                return "timestamp(6) with time zone";
            }
            if (type == DataType.Decimal)
            {
                return "number(38, 2)";
            }
            if (type == DataType.Integer)
            {
                return "number";
            }
            if (type == DataType.Text)
            {
                return "nclob";
            }
            return $"nvarchar2({length})";
        }

        public static string GetDateDiffLessThanYears(string fieldName, string years)
        {
            return GetDateDiffLessThan(fieldName, years, "YEAR");
        }

        public static string GetDateDiffLessThanMonths(string fieldName, string months)
        {
            return GetDateDiffLessThan(fieldName, months, "MONTH");
        }

        public static string GetDateDiffLessThanDays(string fieldName, string days)
        {
            return GetDateDiffLessThan(fieldName, days, "DAY");
        }

        private static int GetSecondsByUnit(string unit)
        {
            var seconds = 1;
            if (unit == "MINUTE")
            {
                seconds = 60;
            }
            else if (unit == "HOUR")
            {
                seconds = 3600;
            }
            else if (unit == "DAY")
            {
                seconds = 86400;
            }
            else if (unit == "MONTH")
            {
                seconds = 2592000;
            }
            else if (unit == "YEAR")
            {
                seconds = 31536000;
            }
            return seconds;
        }

        private static string GetDateDiffLessThan(string fieldName, string fieldValue, string unit)
        {
            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"TIMESTAMPDIFF({unit}, {fieldName}, now()) < {fieldValue}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"DATEDIFF({unit}, {fieldName}, getdate()) < {fieldValue}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"EXTRACT(EPOCH FROM current_timestamp - {fieldName})/{GetSecondsByUnit(unit)} < {fieldValue}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $"EXTRACT({unit} FROM CURRENT_TIMESTAMP - {fieldName}) < {fieldValue}";
            }
            
            return retVal;
        }

        public static string GetDateDiffGreatThanDays(string fieldName, string days)
        {
            return GetDateDiffGreatThan(fieldName, days, "DAY");
        }

        private static string GetDateDiffGreatThan(string fieldName, string fieldValue, string unit)
        {
            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"TIMESTAMPDIFF({unit}, {fieldName}, now()) > {fieldValue}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"DATEDIFF({unit}, {fieldName}, getdate()) > {fieldValue}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"EXTRACT(EPOCH FROM current_timestamp - {fieldName})/{GetSecondsByUnit(unit)} > {fieldValue}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $"EXTRACT({unit} FROM CURRENT_TIMESTAMP - {fieldName}) > {fieldValue}";
            }

            return retVal;
        }

        public static string GetDatePartYear(string fieldName)
        {
            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"DATE_FORMAT({fieldName}, '%Y')";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"DATEPART([YEAR], {fieldName})";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"date_part('year', {fieldName})";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $"EXTRACT(year from {fieldName})";
            }

            return retVal;
        }

        public static string GetDatePartMonth(string fieldName)
        {
            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"DATE_FORMAT({fieldName}, '%c')";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"DATEPART([MONTH], {fieldName})";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"date_part('month', {fieldName})";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $"EXTRACT(month from {fieldName})";
            }            

            return retVal;
        }

        public static string GetDatePartDay(string fieldName)
        {
            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"DATE_FORMAT({fieldName}, '%e')";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"DATEPART([DAY], {fieldName})";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"date_part('day', {fieldName})";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $"EXTRACT(day from {fieldName})";
            }

            return retVal;
        }

        public static string GetComparableNow()
        {
            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = "now()";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = "getdate()";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = "current_timestamp";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = "sysdate";
            }

            return retVal;
        }

        public static string GetComparableDate(DateTime dateTime)
        {
            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"'{dateTime:yyyy-MM-dd}'";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"'{dateTime:yyyy-MM-dd}'";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"'{dateTime:yyyy-MM-dd}'";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $"to_date('{dateTime:yyyy-MM-dd}', 'yyyy-mm-dd')";
            }

            return retVal;
        }

        public static string GetComparableDateTime(DateTime dateTime)
        {
            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"'{dateTime:yyyy-MM-dd HH:mm:ss}'";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"'{dateTime:yyyy-MM-dd HH:mm:ss}'";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"'{dateTime:yyyy-MM-dd HH:mm:ss}'";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $"to_date('{dateTime:yyyy-MM-dd HH:mm:ss}', 'yyyy-mm-dd hh24:mi:ss')";
            }

            return retVal;
        }

        public static string ToPlusSqlString(string fieldName, int plusNum = 1)
        {
            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"{fieldName} = IFNULL({fieldName}, 0) + {plusNum}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"{fieldName} = ISNULL({fieldName}, 0) + {plusNum}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"{fieldName} = COALESCE({fieldName}, 0) + {plusNum}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $"{fieldName} = COALESCE({fieldName}, 0) + {plusNum}";
            }

            return retVal;
        }

        public static string ToMinusSqlString(string fieldName, int minusNum = 1)
        {
            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"{fieldName} = IFNULL({fieldName}, 0) - {minusNum}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"{fieldName} = ISNULL({fieldName}, 0) - {minusNum}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"{fieldName} = COALESCE({fieldName}, 0) - {minusNum}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = $"{fieldName} = COALESCE({fieldName}, 0) - {minusNum}";
            }

            return retVal;
        }

        public static string GetOrderByRandom()
        {
            var retVal = string.Empty;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = "ORDER BY RAND()";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = "ORDER BY NEWID() DESC";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = "ORDER BY random()";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                retVal = "ORDER BY dbms_random.value()";
            }

            return retVal;
        }

        public static string ToSqlString(string inputString)
        {
            return !string.IsNullOrEmpty(inputString) ? inputString.Replace("'", "''") : string.Empty;
        }

        public static string ReadNextSqlString(StreamReader reader)
        {
            try
            {
                var sb = new StringBuilder();

                while (true)
                {
                    var lineOfText = reader.ReadLine();

                    if (lineOfText == null)
                    {
                        return sb.Length > 0 ? sb.ToString() : null;
                    }

                    if (lineOfText.StartsWith("--")) continue;
                    lineOfText = lineOfText.Replace(")ENGINE=INNODB", ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                    if (lineOfText.TrimEnd().ToUpper() == "GO")
                    {
                        break;
                    }

                    sb.Append(lineOfText + Environment.NewLine);
                }

                return sb.ToString();
            }
            catch
            {
                return null;
            }
        }

        public static string ReadNextStatementFromStream(StringReader reader)
        {
            try
            {
                var sb = new StringBuilder();

                while (true)
                {
                    var lineOfText = reader.ReadLine();
                    if (lineOfText == null)
                    {
                        return sb.Length > 0 ? sb.ToString() : null;
                    }

                    if (lineOfText.TrimEnd().ToUpper() == "GO")
                    {
                        break;
                    }

                    sb.Append(lineOfText + Environment.NewLine);
                }

                return sb.ToString();
            }
            catch
            {
                return null;
            }
        }

        private static object Eval(object dataItem, string name)
        {
            object o = null;
            try
            {
                o = DataBinder.Eval(dataItem, name);
            }
            catch
            {
                // ignored
            }
            if (o == DBNull.Value)
            {
                o = null;
            }
            return o;
        }

        public static int EvalInt(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            return o == null ? 0 : Convert.ToInt32(o);
        }

        public static string EvalString(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            var value =  o?.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                value = AttackUtils.UnFilterSql(value);
            }
            if (WebConfigUtils.DatabaseType == DatabaseType.Oracle && value == OracleEmptyValue)
            {
                value = string.Empty;
            }
            return value;
        }

        public static DateTime EvalDateTime(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            if (o == null)
            {
                return DateUtils.SqlMinValue;
            }
            return (DateTime)o;
        }

        public static bool EvalBool(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            return o != null && TranslateUtils.ToBool(o.ToString());
        }

        public static string GetDatabaseNameFormConnectionString(DatabaseType databaseType, string connectionString)
        {
            if (databaseType == DatabaseType.Oracle)
            {
                var index1 = connectionString.IndexOf("SERVICE_NAME=", StringComparison.Ordinal);
                var index2 = connectionString.IndexOf(")));", StringComparison.Ordinal);
                return connectionString.Substring(index1 + 13, index2 - index1 - 13);
            }
            var name = GetValueFromConnectionString(connectionString, "Database");
            if (string.IsNullOrEmpty(name))
            {
                name = GetValueFromConnectionString(connectionString, "Initial Catalog");
            }
            return name;
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
                        if (StringUtils.EqualsIgnoreCase(attribute, pair.Trim().Split('=')[0]))
                        {
                            retVal = pair.Trim().Split('=')[1];
                            break;
                        }
                    }
                }
            }
            return retVal;
        }

        
    }
}
