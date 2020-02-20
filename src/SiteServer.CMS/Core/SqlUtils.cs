using System;
using System.Collections.Generic;
using System.Text;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Framework;

namespace SiteServer.CMS.Core
{
    public static class SqlUtils
    {
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

        public static string GetQuotedIdentifier(string identifier)
        {
            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                return $"`{identifier}`";
            }

            return WebConfigUtils.DatabaseType == DatabaseType.SqlServer ? $"[{identifier}]" : identifier;
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
