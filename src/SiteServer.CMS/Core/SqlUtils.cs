using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Datory;
using MySql.Data.MySqlClient;
using Npgsql;
using NpgsqlTypes;
using Oracle.ManagedDataAccess.Client;
using SiteServer.Utils;
using SqlKata;

namespace SiteServer.CMS.Core
{
    public static class SqlUtils
    {
        public const string Asterisk = "*";
        public const string OracleEmptyValue = "_EMPTY_";

        public static IDbConnection GetIDbConnection(DatabaseType databaseType, string connectionString)
        {
            return new Connection(databaseType, connectionString);
        }

        public static IDbCommand GetIDbCommand()
        {
            IDbCommand command = null;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                command = new MySqlCommand();
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                command = new SqlCommand();
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                command = new NpgsqlCommand();
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                command = new OracleCommand();
            }

            return command;
        }

        public static IDbDataAdapter GetIDbDataAdapter()
        {
            IDbDataAdapter adapter = null;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                adapter = new MySqlDataAdapter();
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                adapter = new SqlDataAdapter();
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                adapter = new NpgsqlDataAdapter();
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                adapter = new OracleDataAdapter();
            }

            return adapter;
        }

        public static void FillDataAdapterWithDataTable(IDbDataAdapter adapter, DataTable table)
        {
            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                ((MySqlDataAdapter)adapter).Fill(table);
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                ((SqlDataAdapter)adapter).Fill(table);
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                ((NpgsqlDataAdapter)adapter).Fill(table);
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                ((OracleDataAdapter)adapter).Fill(table);
            }
        }

        public static IDbDataParameter GetIDbDataParameter(string parameterName, DataType dataType, int size, object value)
        {
            IDbDataParameter parameter = null;

            if (size == 0)
            {
                if (AppSettings.DatabaseType == DatabaseType.MySql)
                {
                    parameter = new MySqlParameter(parameterName, ToMySqlDbType(dataType))
                    {
                        Value = value
                    };
                }
                else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
                {
                    parameter = new SqlParameter(parameterName, ToSqlServerDbType(dataType))
                    {
                        Value = value
                    };
                }
                else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
                {
                    parameter = new NpgsqlParameter(parameterName, ToNpgsqlDbType(dataType))
                    {
                        Value = value
                    };
                }
                else if (AppSettings.DatabaseType == DatabaseType.Oracle)
                {
                    parameter = new OracleParameter(parameterName, ToOracleDbType(dataType))
                    {
                        Value = ToOracleDbValue(dataType, value)
                    };
                }
            }
            else
            {
                if (AppSettings.DatabaseType == DatabaseType.MySql)
                {
                    parameter = new MySqlParameter(parameterName, ToMySqlDbType(dataType), size)
                    {
                        Value = value
                    };
                }
                else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
                {
                    parameter = new SqlParameter(parameterName, ToSqlServerDbType(dataType), size)
                    {
                        Value = value
                    };
                }
                else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
                {
                    parameter = new NpgsqlParameter(parameterName, ToNpgsqlDbType(dataType), size)
                    {
                        Value = value
                    };
                }
                else if (AppSettings.DatabaseType == DatabaseType.Oracle)
                {
                    parameter = new OracleParameter(parameterName, ToOracleDbType(dataType), size)
                    {
                        Value = ToOracleDbValue(dataType, value)
                    };
                }
            }

            return parameter;
        }

        // public static string GetSqlColumnInList(string columnName, List<int> idList)
        // {
        //     if (idList == null || idList.Count == 0) return string.Empty;

        //     if (idList.Count < 1000)
        //     {
        //         return $"{columnName} IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";
        //     }

        //     var sql = new StringBuilder();
        //     sql.Append(" ").Append(columnName).Append(" IN ( ");
        //     for (var i = 0; i < idList.Count; i++)
        //     {
        //         sql.Append(idList[i] + ",");
        //         if ((i + 1) % 1000 == 0 && i + 1 < idList.Count)
        //         {
        //             sql.Length -= 1;
        //             sql.Append(" ) OR ").Append(columnName).Append(" IN (");
        //         }
        //     }
        //     sql.Length -= 1;
        //     sql.Append(" )");

        //     return $"({sql})";
        // }

        public static string GetInStr(string columnName, string inStr)
        {
            var retval = string.Empty;
            inStr = AttackUtils.FilterSql(inStr);

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $"INSTR({columnName}, '{inStr}') > 0";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $"CHARINDEX('{inStr}', {columnName}) > 0";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $"POSITION('{inStr}' IN {columnName}) > 0";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $"INSTR({columnName}, '{inStr}') > 0";
            }

            return retval;
        }



        public static KeyValuePair<string, IDataParameter> GetInStrWithParameter(string columnName, string inStr)
        {
            var parameterName = $"P{StringUtils.GetShortGuid()}";
            var sqlString = string.Empty;
            var parameter = GetIDbDataParameter(parameterName, DataType.VarChar, 0, inStr);

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                sqlString = $"INSTR({columnName}, @{parameterName}) > 0";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                sqlString = $"CHARINDEX(@{parameterName}, {columnName}) > 0";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                sqlString = $"POSITION(@{parameterName} IN {columnName}) > 0";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                sqlString = $"INSTR({columnName}, @{parameterName}) > 0";
            }

            return new KeyValuePair<string, IDataParameter>(sqlString, parameter);
        }

        // public static string GetInStrReverse(string inStr, string columnName)
        // {
        //     var retval = string.Empty;
        //     inStr = AttackUtils.FilterSql(inStr);

        //     if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
        //     {
        //         retval = $"INSTR('{inStr}', {columnName}) > 0";
        //     }
        //     else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
        //     {
        //         retval = $"CHARINDEX({columnName}, '{inStr}') > 0";
        //     }
        //     else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
        //     {
        //         retval = $"POSITION({columnName} IN '{inStr}') > 0";
        //     }
        //     else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
        //     {
        //         retval = $"INSTR('{inStr}', {columnName}) > 0";
        //     }

        //     return retval;
        // }

        public static string GetNotInStr(string columnName, string inStr)
        {
            var retval = string.Empty;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $"INSTR({columnName}, '{inStr}') = 0";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $"CHARINDEX('{inStr}', {columnName}) = 0";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $"POSITION('{inStr}' IN {columnName}) = 0";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $"INSTR({columnName}, '{inStr}') = 0";
            }

            return retval;
        }

        public static string ToTopSqlString(string tableName, string columns, string whereString, string orderString, int topN)
        {
            string retval = $"SELECT {columns} FROM {tableName} {whereString} {orderString}";
            if (topN <= 0) return retval;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $"SELECT {columns} FROM {tableName} {whereString} {orderString} LIMIT {topN}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $"SELECT TOP {topN} {columns} FROM {tableName} {whereString} {orderString}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $"SELECT {columns} FROM {tableName} {whereString} {orderString} LIMIT {topN}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $@"SELECT {columns} FROM {tableName} {whereString} {orderString} FETCH FIRST {topN} ROWS ONLY";
            }

            return retval;
        }

        public static string ToTopSqlString(string sqlString, string orderString, int topN)
        {
            string retval = $"SELECT * FROM ({sqlString}) {orderString}";
            if (topN <= 0) return retval;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $"SELECT * FROM ({sqlString}) {orderString} LIMIT {topN}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $"SELECT TOP {topN} * FROM ({sqlString}) {orderString}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $"SELECT * FROM ({sqlString}) {orderString} LIMIT {topN}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $@"SELECT * FROM ({sqlString}) {orderString} FETCH FIRST {topN} ROWS ONLY";
            }

            return retval;
        }

        public static string GetPageSqlString(string sqlString, string orderString, int itemsPerPage, int currentPageIndex, int pageCount, int recordsInLastPage)
        {
            var retval = string.Empty;

            var recsToRetrieve = itemsPerPage;
            if (currentPageIndex == pageCount - 1)
            {
                recsToRetrieve = recordsInLastPage;
            }

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
        SELECT * FROM ({sqlString}) {orderString} FETCH FIRST {itemsPerPage * (currentPageIndex + 1)} ROWS ONLY
    ) {orderStringReverse} FETCH FIRST {recsToRetrieve} ROWS ONLY
) {orderString}";
            }

            return retval;
        }

        public static string GetDistinctTopSqlString(string tableName, string columns, string whereString, string orderString, int topN)
        {
            var retval = $"SELECT DISTINCT {columns} FROM {tableName} {whereString} {orderString}";
            if (topN <= 0) return retval;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $"SELECT DISTINCT {columns} FROM {tableName} {whereString} {orderString} LIMIT {topN}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $"SELECT DISTINCT TOP {topN} {columns} FROM {tableName} {whereString} {orderString}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $"SELECT DISTINCT {columns} FROM {tableName} {whereString} {orderString} LIMIT {topN}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $"SELECT DISTINCT {columns} FROM {tableName} {whereString} {orderString} FETCH FIRST {topN} ROWS ONLY";
            }

            return retval;
        }

        // public static string ToInTopSqlString(string tableName, string columns, string whereString, string orderString, int topN)
        // {
        //     var builder = new StringBuilder();
        //     if (WebConfigUtils.DatabaseType != DatabaseType.Oracle)
        //     {
        //         foreach (var column in TranslateUtils.StringCollectionToStringList(columns))
        //         {
        //             builder.Append($"T.{column}, ");
        //         }
        //         builder.Length = builder.Length - 2;
        //         return
        //             $"SELECT {builder} FROM ({ToTopSqlString(tableName, columns, whereString, orderString, topN)}) AS T";
        //     }

        //     foreach (var column in TranslateUtils.StringCollectionToStringList(columns))
        //     {
        //         builder.Append($"{column}, ");
        //     }
        //     builder.Length = builder.Length - 2;
        //     return
        //         $"SELECT {builder} FROM ({ToTopSqlString(tableName, columns, whereString, orderString, topN)})";
        // }

        public static string GetQuotedIdentifier(string identifier)
        {
            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                return $"`{identifier}`";
            }

            return AppSettings.DatabaseType == DatabaseType.SqlServer ? $"[{identifier}]" : identifier;
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

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                return ToMySqlColumnTypeString(tableColumn.DataType, tableColumn.DataLength);
            }

            if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                return ToSqlServerColumnTypeString(tableColumn.DataType, tableColumn.DataLength);
            }

            if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                return ToPostgreColumnTypeString(tableColumn.DataType, tableColumn.DataLength);
            }

            if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                return ToOracleColumnTypeString(tableColumn.DataType, tableColumn.DataLength);
            }

            return string.Empty;
        }

        public static string GetPrimaryKeySqlString(string tableName, string attributeName)
        {
            return AppSettings.DatabaseType == DatabaseType.MySql
                ? $@"PRIMARY KEY ({attributeName})"
                : $@"CONSTRAINT PK_{tableName}_{attributeName} PRIMARY KEY ({attributeName})";
        }

        //public static string GetColumnSqlString(DataType dataType, string attributeName, int length)
        //{
        //    var retval = string.Empty;

        //    if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
        //    {
        //        retval = ToMySqlColumnString(dataType, attributeName, length);
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
        //    {
        //        retval = ToSqlServerColumnString(dataType, attributeName, length);
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
        //    {
        //        retval = ToPostgreColumnString(dataType, attributeName, length);
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
        //    {
        //        retval = ToOracleColumnString(dataType, attributeName, length);
        //    }

        //    return retval;
        //}

        public static string GetAddColumnsSqlString(string tableName, string columnsSqlString)
        {
            var retval = string.Empty;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $"ALTER TABLE `{tableName}` ADD ({columnsSqlString})";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $"ALTER TABLE [{tableName}] ADD {columnsSqlString}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $"ALTER TABLE {tableName} ADD {columnsSqlString}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $"ALTER TABLE {tableName} ADD {columnsSqlString}";
            }

            return retval;
        }

        public static string GetModifyColumnsSqlString(string tableName, string columnName, string columnTypeString)
        {
            var retval = string.Empty;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $"ALTER TABLE `{tableName}` MODIFY {columnName} {columnTypeString}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $"ALTER TABLE [{tableName}] ALTER COLUMN {columnName} {columnTypeString}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $"ALTER TABLE {tableName} ALTER COLUMN {columnName} TYPE {columnTypeString}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $"ALTER TABLE {tableName} MODIFY {columnName} {columnTypeString}";
            }

            return retval;
        }

        public static string GetDropColumnsSqlString(string tableName, string columnName)
        {
            var retval = string.Empty;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $"ALTER TABLE `{tableName}` DROP COLUMN `{columnName}`";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $"ALTER TABLE [{tableName}] DROP COLUMN [{columnName}]";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $"ALTER TABLE {tableName} DROP COLUMN {columnName}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $"ALTER TABLE {tableName} DROP COLUMN {columnName}";
            }

            return retval;
        }

        public static string GetAutoIncrementDataType(bool alterTable = false)
        {
            var retval = string.Empty;

            //if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            //{
            //    retval = "INT AUTO_INCREMENT PRIMARY KEY";
            //}
            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = alterTable ? "INT AUTO_INCREMENT UNIQUE KEY" : "INT AUTO_INCREMENT";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = "int IDENTITY (1, 1)";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = "SERIAL";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = "NUMBER GENERATED BY DEFAULT ON NULL AS IDENTITY";
            }

            return retval;
        }

        public static DataType ToDataType(string dataTypeStr)
        {
            if (string.IsNullOrEmpty(dataTypeStr)) return DataType.VarChar;

            var dataType = DataType.VarChar;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
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
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
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
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
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
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
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
            var retval = string.Empty;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $"TIMESTAMPDIFF({unit}, {fieldName}, now()) < {fieldValue}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $"DATEDIFF({unit}, {fieldName}, getdate()) < {fieldValue}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $"EXTRACT(EPOCH FROM current_timestamp - {fieldName})/{GetSecondsByUnit(unit)} < {fieldValue}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $"EXTRACT({unit} FROM CURRENT_TIMESTAMP - {fieldName}) < {fieldValue}";
            }

            return retval;
        }

        // public static string GetDateDiffGreatThanDays(string fieldName, string days)
        // {
        //     return GetDateDiffGreatThan(fieldName, days, "DAY");
        // }

        private static string GetDateDiffGreatThan(string fieldName, string fieldValue, string unit)
        {
            var retval = string.Empty;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $"TIMESTAMPDIFF({unit}, {fieldName}, now()) > {fieldValue}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $"DATEDIFF({unit}, {fieldName}, getdate()) > {fieldValue}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $"EXTRACT(EPOCH FROM current_timestamp - {fieldName})/{GetSecondsByUnit(unit)} > {fieldValue}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $"EXTRACT({unit} FROM CURRENT_TIMESTAMP - {fieldName}) > {fieldValue}";
            }

            return retval;
        }

        public static string GetDatePartYear(string fieldName)
        {
            var retval = string.Empty;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $"DATE_FORMAT({fieldName}, '%Y')";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $"DATEPART([YEAR], {fieldName})";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $"date_part('year', {fieldName})";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $"EXTRACT(year from {fieldName})";
            }

            return retval;
        }

        public static string GetDatePartMonth(string fieldName)
        {
            var retval = string.Empty;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $"DATE_FORMAT({fieldName}, '%c')";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $"DATEPART([MONTH], {fieldName})";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $"date_part('month', {fieldName})";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $"EXTRACT(month from {fieldName})";
            }

            return retval;
        }

        public static string GetDatePartDay(string fieldName)
        {
            var retval = string.Empty;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $"DATE_FORMAT({fieldName}, '%e')";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $"DATEPART([DAY], {fieldName})";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $"date_part('day', {fieldName})";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $"EXTRACT(day from {fieldName})";
            }

            return retval;
        }

        public static string GetComparableNow()
        {
            var retval = string.Empty;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = "now()";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = "getdate()";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = "current_timestamp";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = "sysdate";
            }

            return retval;
        }

        public static string GetComparableDate(DateTime dateTime)
        {
            var retval = string.Empty;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $"'{dateTime:yyyy-MM-dd}'";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $"'{dateTime:yyyy-MM-dd}'";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $"'{dateTime:yyyy-MM-dd}'";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $"to_date('{dateTime:yyyy-MM-dd}', 'yyyy-mm-dd')";
            }

            return retval;
        }

        public static string GetComparableDateTime(DateTime dateTime)
        {
            var retval = string.Empty;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $"'{dateTime:yyyy-MM-dd HH:mm:ss}'";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $"'{dateTime:yyyy-MM-dd HH:mm:ss}'";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $"'{dateTime:yyyy-MM-dd HH:mm:ss}'";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $"to_date('{dateTime:yyyy-MM-dd HH:mm:ss}', 'yyyy-mm-dd hh24:mi:ss')";
            }

            return retval;
        }

        public static string ToPlusSqlString(string fieldName, int plusNum = 1)
        {
            var retval = string.Empty;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $"{fieldName} = IFNULL({fieldName}, 0) + {plusNum}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $"{fieldName} = ISNULL({fieldName}, 0) + {plusNum}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $"{fieldName} = COALESCE({fieldName}, 0) + {plusNum}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $"{fieldName} = COALESCE({fieldName}, 0) + {plusNum}";
            }

            return retval;
        }

        public static string ToMinusSqlString(string fieldName, int minusNum = 1)
        {
            var retval = string.Empty;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = $"{fieldName} = IFNULL({fieldName}, 0) - {minusNum}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = $"{fieldName} = ISNULL({fieldName}, 0) - {minusNum}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = $"{fieldName} = COALESCE({fieldName}, 0) - {minusNum}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = $"{fieldName} = COALESCE({fieldName}, 0) - {minusNum}";
            }

            return retval;
        }

        public static string GetOrderByRandom()
        {
            var retval = string.Empty;

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                retval = "ORDER BY RAND()";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                retval = "ORDER BY NEWID() DESC";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                retval = "ORDER BY random()";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                retval = "ORDER BY dbms_random.value()";
            }

            return retval;
        }

        // public static string ToSqlString(string inputString)
        // {
        //     return !string.IsNullOrEmpty(inputString) ? inputString.Replace("'", "''") : string.Empty;
        // }

        // public static string ReadNextSqlString(StreamReader reader)
        // {
        //     try
        //     {
        //         var sb = new StringBuilder();

        //         while (true)
        //         {
        //             var lineOfText = reader.ReadLine();

        //             if (lineOfText == null)
        //             {
        //                 return sb.Length > 0 ? sb.ToString() : null;
        //             }

        //             if (lineOfText.StartsWith("--")) continue;
        //             lineOfText = lineOfText.Replace(")ENGINE=INNODB", ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

        //             if (lineOfText.TrimEnd().ToUpper() == "GO")
        //             {
        //                 break;
        //             }

        //             sb.Append(lineOfText + Environment.NewLine);
        //         }

        //         return sb.ToString();
        //     }
        //     catch
        //     {
        //         return null;
        //     }
        // }

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

        private static string GetValueFromConnectionString(string connectionString, string attribute)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(attribute))
            {
                var pairs = connectionString.Split(';');
                foreach (var pair in pairs)
                {
                    if (pair.IndexOf("=", StringComparison.Ordinal) != -1)
                    {
                        if (StringUtils.EqualsIgnoreCase(attribute, pair.Trim().Split('=')[0]))
                        {
                            retval = pair.Trim().Split('=')[1];
                            break;
                        }
                    }
                }
            }
            return retval;
        }


    }
}
