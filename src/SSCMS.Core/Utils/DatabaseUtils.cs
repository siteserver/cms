using Datory;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public static class DatabaseUtils
    {
        public static string GetInStr(IDatabase database, string columnName, string inStr)
        {
            var retVal = string.Empty;
            inStr = AttackUtils.FilterSql(inStr);
            columnName = database.GetQuotedIdentifier(columnName);

            if (database.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"INSTR({columnName}, '{inStr}') > 0";
            }
            else if (database.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"CHARINDEX('{inStr}', {columnName}) > 0";
            }
            else if (database.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"POSITION('{inStr}' IN {columnName}) > 0";
            }

            return retVal;
        }

        public static string GetNotInStr(IDatabase database, string columnName, string inStr)
        {
            var retVal = string.Empty;
            columnName = database.GetQuotedIdentifier(columnName);

            if (database.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"INSTR({columnName}, '{inStr}') = 0";
            }
            else if (database.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"CHARINDEX('{inStr}', {columnName}) = 0";
            }
            else if (database.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"POSITION('{inStr}' IN {columnName}) = 0";
            }

            return retVal;
        }

        public static string ToTopSqlString(IDatabase database, string tableName, string columns, string whereString, string orderString, int topN)
        {
            tableName = database.GetQuotedIdentifier(tableName);

            var retVal = $"SELECT {columns} FROM {tableName} {whereString} {orderString}";
            if (topN <= 0) return retVal;

            if (database.DatabaseType == DatabaseType.MySql)
            {
                retVal = $"SELECT {columns} FROM {tableName} {whereString} {orderString} LIMIT {topN}";
            }
            else if (database.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"SELECT TOP {topN} {columns} FROM {tableName} {whereString} {orderString}";
            }
            else if (database.DatabaseType == DatabaseType.PostgreSql)
            {
                retVal = $"SELECT {columns} FROM {tableName} {whereString} {orderString} LIMIT {topN}";
            }

            return retVal;
        }

        public static string ToSqlBool(DatabaseType databaseType, bool val)
        {
            if (databaseType == DatabaseType.SqlServer)
            {
                return val ? "1" : "0";
            }

            return val.ToString().ToLower();
        }
    }
}
