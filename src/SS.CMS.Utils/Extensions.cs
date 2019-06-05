using SqlKata;
using SS.CMS.Data;

namespace SS.CMS.Utils
{
    public static class Extensions
    {
        public static bool IsDefault<T>(this T value) where T : struct
        {
            return value.Equals(default(T));
        }

        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }

        public static Query WhereInStr(this Query query, string columnName, string inStr)
        {
            var where = string.Empty;
            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                where = $"INSTR([{columnName}], ?) > 0";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                where = $"CHARINDEX(?, [{columnName}]) > 0";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                where = $"POSITION(? IN [{columnName}]) > 0";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                where = $"INSTR([{columnName}], ?) > 0";
            }

            query.WhereRaw(where, inStr);
            return query;
        }

        public static Query WhereInStrReverse(this Query query, string columnName, string inStr)
        {
            var where = string.Empty;
            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                where = $"INSTR(?, [{columnName}]) > 0";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                where = $"CHARINDEX([{columnName}], ?) > 0";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                where = $"POSITION([{columnName}] IN ?) > 0";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                where = $"INSTR(?, [{columnName}]) > 0";
            }

            query.WhereRaw(where, inStr);
            return query;
        }

        public static Query WhereNotInStr(this Query query, string columnName, string inStr)
        {
            var where = string.Empty;
            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                where = $"INSTR([{columnName}], ?) = 0";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                where = $"CHARINDEX(?, [{columnName}]) = 0";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                where = $"POSITION(? IN [{columnName}]) = 0";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                where = $"INSTR([{columnName}], ?) = 0";
            }

            query.WhereRaw(where, inStr);
            return query;
        }
    }
}
