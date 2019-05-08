using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using Datory;
using MySql.Data.MySqlClient;
using Npgsql;
using NpgsqlTypes;
using Oracle.ManagedDataAccess.Client;
using SiteServer.CMS.Apis;
using SiteServer.CMS.Fx;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Core
{
    public static class SqlUtils
    {


        //public static IDbDataParameter GetIDbDataParameter(string parameterName, DataType dataType, int size, object value)
        //{
        //    IDbDataParameter parameter = null;

        //    if (size == 0)
        //    {
        //        if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
        //        {
        //            parameter = new MySqlParameter(parameterName, ToMySqlDbType(dataType))
        //            {
        //                Value = value
        //            };
        //        }
        //        else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
        //        {
        //            parameter = new SqlParameter(parameterName, ToSqlServerDbType(dataType))
        //            {
        //                Value = value
        //            };
        //        }
        //        else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
        //        {
        //            parameter = new NpgsqlParameter(parameterName, ToNpgsqlDbType(dataType))
        //            {
        //                Value = value
        //            };
        //        }
        //        else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
        //        {
        //            parameter = new OracleParameter(parameterName, ToOracleDbType(dataType))
        //            {
        //                Value = ToOracleDbValue(dataType, value)
        //            };
        //        }
        //    }
        //    else
        //    {
        //        if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
        //        {
        //            parameter = new MySqlParameter(parameterName, ToMySqlDbType(dataType), size)
        //            {
        //                Value = value
        //            };
        //        }
        //        else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
        //        {
        //            parameter = new SqlParameter(parameterName, ToSqlServerDbType(dataType), size)
        //            {
        //                Value = value
        //            };
        //        }
        //        else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
        //        {
        //            parameter = new NpgsqlParameter(parameterName, ToNpgsqlDbType(dataType), size)
        //            {
        //                Value = value
        //            };
        //        }
        //        else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
        //        {
        //            parameter = new OracleParameter(parameterName, ToOracleDbType(dataType), size)
        //            {
        //                Value = ToOracleDbValue(dataType, value)
        //            };
        //        }
        //    }

        //    return parameter;
        //}

        //public static string GetInStrReverse(string inStr, string columnName)
        //{
        //    var retVal = string.Empty;

        //    if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
        //    {
        //        retVal = $"INSTR('{inStr}', {columnName}) > 0";
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
        //    {
        //        retVal = $"CHARINDEX({columnName}, '{inStr}') > 0";
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
        //    {
        //        retVal = $"POSITION({columnName} IN '{inStr}') > 0";
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
        //    {
        //        retVal = $"INSTR('{inStr}', {columnName}) > 0";
        //    }

        //    return retVal;
        //}

        //public static string GetDistinctTopSqlString(string tableName, string columns, string whereString, string orderString, int topN)
        //{
        //    var retVal = $"SELECT DISTINCT {columns} FROM {tableName} {whereString} {orderString}";
        //    if (topN <= 0) return retVal;

        //    if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
        //    {
        //        retVal = $"SELECT DISTINCT {columns} FROM {tableName} {whereString} {orderString} LIMIT {topN}";
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
        //    {
        //        retVal = $"SELECT DISTINCT TOP {topN} {columns} FROM {tableName} {whereString} {orderString}";
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
        //    {
        //        retVal = $"SELECT DISTINCT {columns} FROM {tableName} {whereString} {orderString} LIMIT {topN}";
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
        //    {
        //        retVal = $"SELECT DISTINCT {columns} FROM {tableName} {whereString} {orderString} FETCH FIRST {topN} ROWS ONLY";
        //    }

        //    return retVal;
        //}

        //public static string ToInTopSqlString(string tableName, IList<string> columns, string whereString, string orderString, int topN)
        //{
        //    var builder = new StringBuilder();
        //    if (WebConfigUtils.DatabaseType != DatabaseType.Oracle)
        //    {
        //        foreach (var column in columns)
        //        {
        //            builder.Append($"T.{column}, ");
        //        }
        //        builder.Length = builder.Length - 2;
        //        return
        //            $"SELECT {builder} FROM ({DatorySql.GetSqlString(WebConfigUtils.DatabaseType, DatabaseApi.Instance.UseLegacyPagination, tableName, columns, whereString, orderString, topN)}) AS T";
        //    }

        //    foreach (var column in columns)
        //    {
        //        builder.Append($"{column}, ");
        //    }
        //    builder.Length = builder.Length - 2;
        //    return
        //        $"SELECT {builder} FROM ({DatorySql.GetSqlString(WebConfigUtils.DatabaseType, DatabaseApi.Instance.UseLegacyPagination, tableName, columns, whereString, orderString, topN)})";
        //}


        //private static SqlDbType ToSqlServerDbType(DataType type)
        //{
        //    if (type == DataType.Boolean)
        //    {
        //        return SqlDbType.Bit;
        //    }
        //    if (type == DataType.DateTime)
        //    {
        //        return SqlDbType.DateTime;
        //    }
        //    if (type == DataType.Decimal)
        //    {
        //        return SqlDbType.Decimal;
        //    }
        //    if (type == DataType.Integer)
        //    {
        //        return SqlDbType.Int;
        //    }
        //    if (type == DataType.Text)
        //    {
        //        return SqlDbType.NText;
        //    }
        //    if (type == DataType.VarChar)
        //    {
        //        return SqlDbType.NVarChar;
        //    }
        //    return SqlDbType.VarChar;
        //}

        //private static MySqlDbType ToMySqlDbType(DataType type)
        //{
        //    if (type == DataType.Boolean)
        //    {
        //        return MySqlDbType.Bit;
        //    }
        //    if (type == DataType.DateTime)
        //    {
        //        return MySqlDbType.DateTime;
        //    }
        //    if (type == DataType.Decimal)
        //    {
        //        return MySqlDbType.Decimal;
        //    }
        //    if (type == DataType.Integer)
        //    {
        //        return MySqlDbType.Int32;
        //    }
        //    if (type == DataType.Text)
        //    {
        //        return MySqlDbType.LongText;
        //    }
        //    if (type == DataType.VarChar)
        //    {
        //        return MySqlDbType.VarString;
        //    }

        //    return MySqlDbType.VarString;
        //}

        //private static NpgsqlDbType ToNpgsqlDbType(DataType type)
        //{
        //    if (type == DataType.Boolean)
        //    {
        //        return NpgsqlDbType.Boolean;
        //    }
        //    if (type == DataType.DateTime)
        //    {
        //        return NpgsqlDbType.TimestampTz;
        //    }
        //    if (type == DataType.Decimal)
        //    {
        //        return NpgsqlDbType.Numeric;
        //    }
        //    if (type == DataType.Integer)
        //    {
        //        return NpgsqlDbType.Integer;
        //    }
        //    if (type == DataType.Text)
        //    {
        //        return NpgsqlDbType.Text;
        //    }
        //    return NpgsqlDbType.Varchar;
        //}

        //public static string ReadNextSqlString(StreamReader reader)
        //{
        //    try
        //    {
        //        var sb = new StringBuilder();

        //        while (true)
        //        {
        //            var lineOfText = reader.ReadLine();

        //            if (lineOfText == null)
        //            {
        //                return sb.Length > 0 ? sb.ToString() : null;
        //            }

        //            if (lineOfText.StartsWith("--")) continue;
        //            lineOfText = lineOfText.Replace(")ENGINE=INNODB", ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

        //            if (lineOfText.TrimEnd().ToUpper() == "GO")
        //            {
        //                break;
        //            }

        //            sb.Append(lineOfText + Environment.NewLine);
        //        }

        //        return sb.ToString();
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        //public static string ReadNextStatementFromStream(StringReader reader)
        //{
        //    try
        //    {
        //        var sb = new StringBuilder();

        //        while (true)
        //        {
        //            var lineOfText = reader.ReadLine();
        //            if (lineOfText == null)
        //            {
        //                return sb.Length > 0 ? sb.ToString() : null;
        //            }

        //            if (lineOfText.TrimEnd().ToUpper() == "GO")
        //            {
        //                break;
        //            }

        //            sb.Append(lineOfText + Environment.NewLine);
        //        }

        //        return sb.ToString();
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        //public static string ToPlusSqlString(string fieldName, int plusNum = 1)
        //{
        //    var retVal = string.Empty;

        //    if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
        //    {
        //        retVal = $"{fieldName} = IFNULL({fieldName}, 0) + {plusNum}";
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
        //    {
        //        retVal = $"{fieldName} = ISNULL({fieldName}, 0) + {plusNum}";
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
        //    {
        //        retVal = $"{fieldName} = COALESCE({fieldName}, 0) + {plusNum}";
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
        //    {
        //        retVal = $"{fieldName} = COALESCE({fieldName}, 0) + {plusNum}";
        //    }

        //    return retVal;
        //}

        //public static string ToMinusSqlString(string fieldName, int minusNum = 1)
        //{
        //    var retVal = string.Empty;

        //    if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
        //    {
        //        retVal = $"{fieldName} = IFNULL({fieldName}, 0) - {minusNum}";
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
        //    {
        //        retVal = $"{fieldName} = ISNULL({fieldName}, 0) - {minusNum}";
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
        //    {
        //        retVal = $"{fieldName} = COALESCE({fieldName}, 0) - {minusNum}";
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
        //    {
        //        retVal = $"{fieldName} = COALESCE({fieldName}, 0) - {minusNum}";
        //    }

        //    return retVal;
        //}

        //private static OracleDbType ToOracleDbType(DataType type)
        //{
        //    if (type == DataType.Boolean)
        //    {
        //        return OracleDbType.Int32;
        //    }
        //    if (type == DataType.DateTime)
        //    {
        //        return OracleDbType.TimeStampTZ;
        //    }
        //    if (type == DataType.Decimal)
        //    {
        //        return OracleDbType.Decimal;
        //    }
        //    if (type == DataType.Integer)
        //    {
        //        return OracleDbType.Int32;
        //    }
        //    if (type == DataType.Text)
        //    {
        //        return OracleDbType.NClob;
        //    }
        //    return OracleDbType.NVarchar2;
        //}

        //public static string GetDateDiffGreatThanDays(string fieldName, string days)
        //{
        //    return GetDateDiffGreatThan(fieldName, days, "DAY");
        //}

        //private static string GetDateDiffGreatThan(string fieldName, string fieldValue, string unit)
        //{
        //    var retVal = string.Empty;

        //    if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
        //    {
        //        retVal = $"TIMESTAMPDIFF({unit}, {fieldName}, now()) > {fieldValue}";
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
        //    {
        //        retVal = $"DATEDIFF({unit}, {fieldName}, getdate()) > {fieldValue}";
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
        //    {
        //        retVal = $"EXTRACT(EPOCH FROM current_timestamp - {fieldName})/{GetSecondsByUnit(unit)} > {fieldValue}";
        //    }
        //    else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
        //    {
        //        retVal = $"EXTRACT({unit} FROM CURRENT_TIMESTAMP - {fieldName}) > {fieldValue}";
        //    }

        //    return retVal;
        //}

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

        public static string ToTopSqlString(string sqlString, string orderString, int topN)
        {
            string retVal = $"SELECT * FROM ({sqlString}) temp {orderString}";
            if (topN <= 0) return retVal;

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"SELECT * FROM ({sqlString}) {orderString} LIMIT {topN}";
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"SELECT TOP {topN} * FROM ({sqlString}) temp {orderString}";
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

        

        

        public static object ToOracleDbValue(DataType dataType, object value)
        {
            // Oracle internally changes empty string to NULL values. Oracle simply won't let insert an empty string. So we replace string.Empty value to placeholder _EMPTY_
            if ((dataType == DataType.Text || dataType == DataType.VarChar) && value != null && value.ToString() == string.Empty)
            {
                return Constants.OracleEmptyValue;
            }
            return value;
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
            if (WebConfigUtils.DatabaseType == DatabaseType.Oracle && value == Constants.OracleEmptyValue)
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
        
    }
}
