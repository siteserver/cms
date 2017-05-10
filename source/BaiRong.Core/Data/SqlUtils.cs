using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Web.UI;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using MySql.Data.MySqlClient;

namespace BaiRong.Core.Data
{
    public static class SqlUtils
    {
        public const string Asterisk = "*";

        public static IDbConnection GetIDbConnection()
        {
            return GetIDbConnection(WebConfigUtils.IsMySql, WebConfigUtils.ConnectionString);
        }

        public static IDbConnection GetIDbConnection(bool isMySql, string connectionString)
        {
            IDbConnection conn;

            if (isMySql)
            {
                conn = new MySqlConnection(connectionString);
            }
            else
            {
                conn = new SqlConnection(connectionString);
            }

            return conn;
        }

        public static IDbCommand GetIDbCommand()
        {
            IDbCommand command;

            if (WebConfigUtils.IsMySql)
            {
                command = new MySqlCommand();
            }
            else
            {
                command = new SqlCommand();
            }

            return command;
        }

        public static IDbDataAdapter GetIDbDataAdapter(string text, string connectionString)
        {
            IDbDataAdapter adapter;

            if (WebConfigUtils.IsMySql)
            {
                adapter = new MySqlDataAdapter(text, connectionString);
            }
            else
            {
                adapter = new SqlDataAdapter(text, connectionString);
            }

            return adapter;
        }

        public static IDbDataAdapter GetIDbDataAdapter()
        {
            IDbDataAdapter adapter;

            if (WebConfigUtils.IsMySql)
            {
                adapter = new MySqlDataAdapter();
            }
            else
            {
                adapter = new SqlDataAdapter();
            }

            return adapter;
        }

        public static void FillDataAdapterWithDataTable(IDbDataAdapter adapter, DataTable table)
        {
            if (WebConfigUtils.IsMySql)
            {
                ((MySqlDataAdapter)adapter).Fill(table);
            }
            else
            {
                ((SqlDataAdapter)adapter).Fill(table);
            }
        }

        public static IDbDataParameter GetIDbDataParameter(string parameterName, EDataType dataType, int size)
        {
            IDbDataParameter parameter;

            if (WebConfigUtils.IsMySql)
            {
                parameter = new MySqlParameter(parameterName, EDataTypeUtils.ToMySqlDbType(dataType), size);
            }
            else
            {
                parameter = new SqlParameter(parameterName, EDataTypeUtils.ToSqlDbType(dataType), size);
            }

            return parameter;
        }

        public static IDbDataParameter GetIDbDataParameter(string parameterName, EDataType dataType)
        {
            IDbDataParameter parameter;

            if (WebConfigUtils.IsMySql)
            {
                parameter = new MySqlParameter(parameterName, EDataTypeUtils.ToMySqlDbType(dataType));
            }
            else
            {
                parameter = new SqlParameter(parameterName, EDataTypeUtils.ToSqlDbType(dataType));
            }

            return parameter;
        }

        public static string GetDatabaseNameFormConnectionString(string connectionString)
        {
            var databaseName = GetValueFromConnectionString(connectionString, "database");
            if (string.IsNullOrEmpty(databaseName))
            {
                databaseName = GetValueFromConnectionString(connectionString, "Initial Catalog");
            }

            return databaseName;
        }

        public static string GetValueFromConnectionString(string connectionString, string attribute)
        {
            //server=(local);uid=sa;pwd=bairong;Trusted_Connection=no;database=V1
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

        public static string GetInStr(string columnName, string inStr)
        {
            return WebConfigUtils.IsMySql ? $"INSTR({columnName}, '{inStr}') > 0" : $"CHARINDEX('{inStr}', {columnName}) > 0";
        }

        public static string GetNotInStr(string columnName, string inStr)
        {
            return WebConfigUtils.IsMySql ? $"INSTR({columnName}, '{inStr}') = 0" : $"CHARINDEX('{inStr}', {columnName}) = 0";
        }

        public static string GetNotNullAndEmpty(string columnName)
        {
            return WebConfigUtils.IsMySql ? $"LENGTH(IFNULL({columnName},'')) > 0" : $"DATALENGTH({columnName}) > 0";
        }

        public static string GetNullOrEmpty(string columnName)
        {
            return WebConfigUtils.IsMySql ? $"LENGTH(IFNULL({columnName},'')) = 0" : $"DATALENGTH({columnName}) = 0";
        }

        public static string GetTopSqlString(string tableName, string columns, string whereAndOrder, int topN)
        {
            if (topN > 0)
            {
                return WebConfigUtils.IsMySql ? $"SELECT {columns} FROM {tableName} {whereAndOrder} LIMIT {topN}" : $"SELECT TOP {topN} {columns} FROM {tableName} {whereAndOrder}";
            }
            return $"SELECT {columns} FROM {tableName} {whereAndOrder}";
        }

        public static string GetDistinctTopSqlString(string tableName, string columns, string whereAndOrder, int topN)
        {
            if (topN > 0)
            {
                return WebConfigUtils.IsMySql ? $"SELECT DISTINCT {columns} FROM {tableName} {whereAndOrder} LIMIT {topN}" : $"SELECT DISTINCT TOP {topN} {columns} FROM {tableName} {whereAndOrder}";
            }
            return $"SELECT DISTINCT {columns} FROM {tableName} {whereAndOrder}";
        }

        public static string GetInTopSqlString(string tableName, string columns, string whereAndOrder, int topN)
        {
            var builder = new StringBuilder();
            foreach (var column in TranslateUtils.StringCollectionToStringList(columns))
            {
                builder.Append($"T.{column}, ");
            }
            builder.Length = builder.Length - 2;
            return
                $"SELECT {builder} FROM ({GetTopSqlString(tableName, columns, whereAndOrder, topN)}) AS T";
        }

        public static string GetColumnSqlString(EDataType dataType, string attributeName, int length)
        {
            return WebConfigUtils.IsMySql ? GetMySqlColumnSqlString(dataType, attributeName, length) : GetSqlServerColumnSqlString(dataType, attributeName, length);
        }

        public static string GetMySqlColumnSqlString(EDataType dataType, string attributeName, int length)
        {
            string retval;
            var sqlDbType = EDataTypeUtils.ToSqlDbType(dataType);
            switch (sqlDbType)
            {
                case SqlDbType.Char:
                    retval = $"{attributeName} VARCHAR({length})";
                    break;
                case SqlDbType.DateTime:
                    retval = $"{attributeName} DATETIME";
                    break;
                case SqlDbType.Decimal:
                    retval = $"{attributeName} DECIMAL(18, 2)";
                    break;
                case SqlDbType.Float:
                    retval = $"{attributeName} FLOAT(18, 2)";
                    break;
                case SqlDbType.Int:
                    retval = $"{attributeName} INT";
                    break;
                case SqlDbType.NChar:
                    retval = $"{attributeName} VARCHAR({length})";
                    break;
                case SqlDbType.NText:
                    retval = $"{attributeName} LONGTEXT";
                    break;
                case SqlDbType.NVarChar:
                    retval = $"{attributeName} VARCHAR({length})";
                    break;
                case SqlDbType.Text:
                    retval = $"{attributeName} LONGTEXT";
                    break;
                case SqlDbType.VarChar:
                    retval = $"{attributeName} VARCHAR({length})";
                    break;
                default:
                    retval = $"{attributeName} VARCHAR({length})";
                    break;
            }
            return retval;
        }

        public static string GetSqlServerColumnSqlString(EDataType dataType, string attributeName, int length)
        {
            var retval = string.Empty;
            var sqlDbType = EDataTypeUtils.ToSqlDbType(dataType);
            switch (sqlDbType)
            {
                case SqlDbType.BigInt:
                    retval = $"[{attributeName}] [bigint]";
                    break;
                case SqlDbType.Binary:
                    retval = $"[{attributeName}] [binary] ({length})";
                    break;
                case SqlDbType.Bit:
                    retval = $"[{attributeName}] [bit]";
                    break;
                case SqlDbType.Char:
                    retval = $"[{attributeName}] [char] ({length})";
                    break;
                case SqlDbType.DateTime:
                    retval = $"[{attributeName}] [datetime]";
                    break;
                case SqlDbType.Decimal:
                    retval = $"[{attributeName}] [decimal] (18, 2)";
                    break;
                case SqlDbType.Float:
                    retval = $"[{attributeName}] [float]";
                    break;
                case SqlDbType.Image:
                    retval = $"[{attributeName}] [image]";
                    break;
                case SqlDbType.Int:
                    retval = $"[{attributeName}] [int]";
                    break;
                case SqlDbType.Money:
                    retval = $"[{attributeName}] [money]";
                    break;
                case SqlDbType.NChar:
                    retval = $"[{attributeName}] [nchar] ({length})";
                    break;
                case SqlDbType.NText:
                    retval = $"[{attributeName}] [ntext]";
                    break;
                case SqlDbType.NVarChar:
                    retval = $"[{attributeName}] [nvarchar] ({length})";
                    break;
                case SqlDbType.Real:
                    retval = $"[{attributeName}] [real]";
                    break;
                case SqlDbType.SmallDateTime:
                    retval = $"[{attributeName}] [smalldatetime]";
                    break;
                case SqlDbType.SmallInt:
                    retval = $"[{attributeName}] [smallint]";
                    break;
                case SqlDbType.SmallMoney:
                    retval = $"[{attributeName}] [smallmoney]";
                    break;
                case SqlDbType.Text:
                    retval = $"[{attributeName}] [text]";
                    break;
                case SqlDbType.Timestamp:
                    retval = $"[{attributeName}] [timestamp]";
                    break;
                case SqlDbType.TinyInt:
                    retval = $"[{attributeName}] [tinyint]";
                    break;
                case SqlDbType.VarBinary:
                    retval = $"[{attributeName}] [varbinary] ({length})";
                    break;
                case SqlDbType.VarChar:
                    retval = $"[{attributeName}] [varchar] ({length})";
                    break;
            }
            retval += " NULL";
            return retval;
        }

        public static string GetDefaultDateString()
        {
            return EDataTypeUtils.GetDefaultString(EDataType.DateTime);
        }

        public static string Parse(EDataType dataType, string valueStr, int length)
        {
            string retval;

            switch (dataType)
            {
                case EDataType.Bit:
                    retval = ParseToIntString(valueStr);
                    break;
                case EDataType.Char:
                    retval = ParseToSqlStringWithQuote(valueStr, length);
                    break;
                case EDataType.DateTime:
                    retval = ParseToDateTimeString(valueStr);
                    break;
                case EDataType.Decimal:
                    retval = ParseToDoubleString(valueStr);
                    break;
                case EDataType.Float:
                    retval = ParseToDoubleString(valueStr);
                    break;
                case EDataType.Integer:
                    retval = ParseToIntString(valueStr);
                    break;
                case EDataType.NChar:
                    retval = ParseToSqlStringWithNAndQuote(valueStr, length);
                    break;
                case EDataType.NText:
                    retval = ParseToSqlStringWithNAndQuote(valueStr);
                    break;
                case EDataType.NVarChar:
                    retval = ParseToSqlStringWithNAndQuote(valueStr, length);
                    break;
                case EDataType.Text:
                    retval = ParseToSqlStringWithQuote(valueStr);
                    break;
                case EDataType.VarChar:
                    retval = ParseToSqlStringWithQuote(valueStr, length);
                    break;
                default:
                    retval = "NULL";
                    break;
            }
            return retval;
        }

        public static string ParseToIntString(string str)
        {
            var valueInt = 0;
            try
            {
                valueInt = int.Parse(str);
            }
            catch
            {
                // ignored
            }
            return valueInt.ToString();
        }


        public static string ParseToDoubleString(string str)
        {
            double valueDouble = 0;
            try
            {
                valueDouble = double.Parse(str);
            }
            catch
            {
                // ignored
            }
            return valueDouble.ToString(CultureInfo.InvariantCulture);
        }


        public static string ParseToSqlStringWithQuote(string str)
        {
            return ParseToSqlStringWithQuote(str, 0);
        }


        public static string ParseToSqlStringWithQuote(string str, int length)
        {
            str = ToSqlString(str);
            if (string.IsNullOrEmpty(str))
            {
                return "''";
            }
            if (length == 0)
            {
                return "'" + str + "'";
            }
            else
            {
                length = Math.Min(str.Length, length);
                return "'" + str.Substring(0, length) + "'";
            }
        }

        public static string ParseToSqlStringWithNAndQuote(string str)
        {
            return ParseToSqlStringWithNAndQuote(str, 0);
        }


        public static string ParseToSqlStringWithNAndQuote(string str, int length)
        {
            string retval;
            str = ToSqlString(str);
            if (length == 0)
            {
                retval = "'" + str + "'";
            }
            else
            {
                length = Math.Min(str.Length, length);
                retval = "'" + str.Substring(0, length) + "'";
            }
            retval = "N" + retval;
            return retval;
        }


        public static string ParseToDateTimeString(string datetimeStr)
        {
            var dateTime = TranslateUtils.ToDateTime(datetimeStr, DateUtils.SqlMinValue);
            if (dateTime == DateTime.MinValue)
            {
                dateTime = DateUtils.SqlMinValue;
            }
            return "'" + dateTime + "'";
        }

        public static string ParseToOracleDateTime(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
            {
                dateTime = DateUtils.SqlMinValue;
            }
            return $"TO_DATE('{dateTime:yyyy-MM-dd hh:mm:ss}', 'yyyy-mm-dd hh24:mi:ss')";
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

        public static string GetDateDiffLessThanHours(string fieldName, string hours)
        {
            return GetDateDiffLessThan(fieldName, hours, "HOUR");
        }

        public static string GetDateDiffLessThanMinutes(string fieldName, string minutes)
        {
            return GetDateDiffLessThan(fieldName, minutes, "MINUTE");
        }

        private static string GetDateDiffLessThan(string fieldName, string fieldValue, string unit)
        {
            if (WebConfigUtils.IsMySql)
            {
                return $"TIMESTAMPDIFF({unit}, {fieldName}, now()) < {fieldValue}";
            }
            return $"DATEDIFF({unit}, {fieldName}, getdate()) < {fieldValue}";
        }

        public static string GetDateDiffGreatThanYears(string fieldName, string years)
        {
            return GetDateDiffGreatThan(fieldName, years, "YEAR");
        }

        public static string GetDateDiffGreatThanMonths(string fieldName, string months)
        {
            return GetDateDiffGreatThan(fieldName, months, "MONTH");
        }

        public static string GetDateDiffGreatThanDays(string fieldName, string days)
        {
            return GetDateDiffGreatThan(fieldName, days, "DAY");
        }

        public static string GetDateDiffGreatThanHours(string fieldName, string hours)
        {
            return GetDateDiffGreatThan(fieldName, hours, "HOUR");
        }

        public static string GetDateDiffGreatThanMinutes(string fieldName, string minutes)
        {
            return GetDateDiffGreatThan(fieldName, minutes, "MINUTE");
        }

        private static string GetDateDiffGreatThan(string fieldName, string fieldValue, string unit)
        {
            if (WebConfigUtils.IsMySql)
            {
                return $"TIMESTAMPDIFF({unit}, {fieldName}, now()) > {fieldValue}";
            }
            return $"DATEDIFF({unit}, {fieldName}, getdate()) > {fieldValue}";
        }

        public static string GetDatePartYear(string fieldName)
        {
            if (WebConfigUtils.IsMySql)
            {
                return $"DATE_FORMAT({fieldName}, '%Y')";
            }
            return $"DATEPART([YEAR], {fieldName})";
        }

        public static string GetDatePartMonth(string fieldName)
        {
            if (WebConfigUtils.IsMySql)
            {
                return $"DATE_FORMAT({fieldName}, '%c')";
            }
            return $"DATEPART([MONTH], {fieldName})";
        }

        public static string GetDatePartDay(string fieldName)
        {
            if (WebConfigUtils.IsMySql)
            {
                return $"DATE_FORMAT({fieldName}, '%e')";
            }
            return $"DATEPART([DAY], {fieldName})";
        }

        public static string GetDatePartHour(string fieldName)
        {
            if (WebConfigUtils.IsMySql)
            {
                return $"DATE_FORMAT({fieldName}, '%k')";
            }
            return $"DATEPART([HOUR], {fieldName})";
        }

        public static string GetDatePartDayOfYear(string fieldName)
        {
            if (WebConfigUtils.IsMySql)
            {
                return $"DATE_FORMAT({fieldName}, '%j')";
            }
            return $"DATEPART([DAYOFYEAR], {fieldName})";
        }

        public static string GetAddOne(string fieldName)
        {
            return GetAddNum(fieldName, 1);
        }

        public static string GetAddNum(string fieldName, int addNum)
        {
            if (WebConfigUtils.IsMySql)
            {
                return $"{fieldName} = IFNULL({fieldName}, 0) + {addNum}";
            }
            return $"{fieldName} = ISNULL({fieldName}, 0) + {addNum}";
        }

        public static string GetMinusNum(string fieldName, int minusNum)
        {
            if (WebConfigUtils.IsMySql)
            {
                return $"{fieldName} = IFNULL({fieldName}, 0) - {minusNum}";
            }
            return $"{fieldName} = ISNULL({fieldName}, 0) - {minusNum}";
        }

        public static int GetMaxLengthForNVarChar()
        {
            const int maxValue = 4000;
            return maxValue;
        }

        public static string ToSqlString(string inputString)
        {
            return !string.IsNullOrEmpty(inputString) ? inputString.Replace("'", "''") : string.Empty;
        }

        public static string ToSqlString(string inputString, int maxLength)
        {
            if (string.IsNullOrEmpty(inputString)) return string.Empty;

            if (maxLength > 0 && inputString.Length > maxLength)
            {
                inputString = inputString.Substring(0, maxLength);
            }
            return inputString.Replace("'", "''");
        }

        /// <summary>
        /// 验证此字符串是否合作作为字段名称
        /// </summary>
        public static bool IsAttributeNameCompliant(string attributeName)
        {
            if (string.IsNullOrEmpty(attributeName) || attributeName.IndexOf(" ", StringComparison.Ordinal) != -1) return false;
            if (-1 != attributeName.IndexOfAny(PathUtils.InvalidPathChars))
            {
                return false;
            }
            foreach (var t in attributeName)
            {
                if (StringUtils.IsTwoBytesChar(t))
                {
                    return false;
                }
            }
            return true;
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
                    lineOfText = lineOfText.Replace(")ENGINE=INNODB", ") ENGINE=InnoDB DEFAULT CHARSET=utf8");

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

        private static string Cache_GetTableStructureCacheString(string connectionString, string databaseName, string tableId)
        {
            return
                $"BaiRong.Core.Data.SqlUtils.GetTableStructureCacheString.{connectionString}.{databaseName}.{tableId}";
        }

        public static void Cache_CacheTableColumnInfoList(string connectionString, string databaseName, string tableId, List<TableColumnInfo> tableColumnInfoList)
        {
            var cacheKey = Cache_GetTableStructureCacheString(connectionString, databaseName, tableId);
            CacheUtils.Max(cacheKey, tableColumnInfoList);
        }

        public static List<TableColumnInfo> Cache_GetTableColumnInfoListCache(string connectionString, string databaseName, string tableId)
        {
            var cacheKey = Cache_GetTableStructureCacheString(connectionString, databaseName, tableId);
            return CacheUtils.Get(cacheKey) as List<TableColumnInfo>;
        }

        private static string Cache_GetTableIDCacheString(string databaseName, string tableName)
        {
            return $"BaiRong.Core.Data.SqlUtils.GetTableStructureCacheString.tableID_{databaseName}_{tableName}";
        }

        public static void Cache_CacheTableID(string databaseName, string tableName, string tableId)
        {
            var cacheKey = Cache_GetTableIDCacheString(databaseName, tableName);
            CacheUtils.Max(cacheKey, tableId);
        }

        public static string Cache_GetTableIDCache(string databaseName, string tableName)
        {
            var cacheKey = Cache_GetTableIDCacheString(databaseName, tableName);
            return CacheUtils.Get(cacheKey) as string;
        }

        public static void Cache_RemoveTableColumnInfoListCache()
        {
            CacheUtils.RemoveByStartString("BaiRong.Core.Data.SqlUtils.GetTableStructureCacheString");
        }

        public static object Eval(object dataItem, string name)
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

        public static decimal EvalDecimal(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            return o == null ? 0 : Convert.ToDecimal(o);
        }

        public static string EvalString(object dataItem, string name)
        {
            var o = Eval(dataItem, name);
            return o?.ToString() ?? string.Empty;
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
    }
}
