using SqlKata.Compilers;
using Datory.DatabaseImpl;

namespace Datory.Utils
{
    public static class DbUtils
    {
        public const int VarCharDefaultLength = 500;

        internal static Compiler GetCompiler(DatabaseType databaseType, string connectionString)
        {
            Compiler compiler = null;

            if (databaseType == DatabaseType.MySql)
            {
                compiler = MySqlImpl.Instance.GetCompiler(connectionString);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                compiler = SqlServerImpl.Instance.GetCompiler(connectionString);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                compiler = PostgreSqlImpl.Instance.GetCompiler(connectionString);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                compiler = SQLiteImpl.Instance.GetCompiler(connectionString);
            }

            return compiler;
        }

        internal static string ColumnIncrement(DatabaseType databaseType, string columnName, int plusNum = 1)
        {
            var retVal = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                retVal = MySqlImpl.Instance.ColumnIncrement(columnName, plusNum);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                retVal = SqlServerImpl.Instance.ColumnIncrement(columnName, plusNum);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                retVal = PostgreSqlImpl.Instance.ColumnIncrement(columnName, plusNum);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                retVal = SQLiteImpl.Instance.ColumnIncrement(columnName, plusNum);
            }

            return retVal;
        }

        internal static string ColumnDecrement(DatabaseType databaseType, string columnName, int minusNum = 1)
        {
            var retVal = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                retVal = MySqlImpl.Instance.ColumnDecrement(columnName, minusNum);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                retVal = SqlServerImpl.Instance.ColumnDecrement(columnName, minusNum);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                retVal = PostgreSqlImpl.Instance.ColumnDecrement(columnName, minusNum);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                retVal = SQLiteImpl.Instance.ColumnDecrement(columnName, minusNum);
            }

            return retVal;
        }

        internal static string GetAutoIncrementDataType(DatabaseType databaseType, bool alterTable = false)
        {
            var retVal = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                retVal = MySqlImpl.Instance.GetAutoIncrementDataType(alterTable);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                retVal = SqlServerImpl.Instance.GetAutoIncrementDataType(alterTable);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                retVal = PostgreSqlImpl.Instance.GetAutoIncrementDataType(alterTable);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                retVal = SQLiteImpl.Instance.GetAutoIncrementDataType(alterTable);
            }

            return retVal;
        }

        internal static string GetColumnSqlString(DatabaseType databaseType, TableColumn tableColumn)
        {
            var retVal = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                retVal = MySqlImpl.Instance.GetColumnSqlString(tableColumn);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                retVal = SqlServerImpl.Instance.GetColumnSqlString(tableColumn);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                retVal = PostgreSqlImpl.Instance.GetColumnSqlString(tableColumn);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                retVal = SQLiteImpl.Instance.GetColumnSqlString(tableColumn);
            }

            return retVal;
        }

        internal static string GetPrimaryKeySqlString(DatabaseType databaseType, string tableName, string attributeName)
        {
            var retVal = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                retVal = MySqlImpl.Instance.GetPrimaryKeySqlString(tableName, attributeName);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                retVal = SqlServerImpl.Instance.GetPrimaryKeySqlString(tableName, attributeName);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                retVal = PostgreSqlImpl.Instance.GetPrimaryKeySqlString(tableName, attributeName);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                retVal = SQLiteImpl.Instance.GetPrimaryKeySqlString(tableName, attributeName);
            }

            return retVal;
        }

        internal static string GetQuotedIdentifier(DatabaseType databaseType, string identifier)
        {
            var retVal = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                retVal = MySqlImpl.Instance.GetQuotedIdentifier(identifier);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                retVal = SqlServerImpl.Instance.GetQuotedIdentifier(identifier);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                retVal = PostgreSqlImpl.Instance.GetQuotedIdentifier(identifier);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                retVal = SQLiteImpl.Instance.GetQuotedIdentifier(identifier);
            }

            return retVal;
        }

        internal static string GetAddColumnsSqlString(DatabaseType databaseType, string tableName, string columnsSqlString)
        {
            var retVal = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                retVal = MySqlImpl.Instance.GetAddColumnsSqlString(tableName, columnsSqlString);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                retVal = SqlServerImpl.Instance.GetAddColumnsSqlString(tableName, columnsSqlString);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                retVal = PostgreSqlImpl.Instance.GetAddColumnsSqlString(tableName, columnsSqlString);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                retVal = SQLiteImpl.Instance.GetAddColumnsSqlString(tableName, columnsSqlString);
            }

            return retVal;
        }

        internal static string GetDropColumnsSqlString(DatabaseType databaseType, string tableName, string columnName)
        {
            return databaseType == DatabaseType.SQLite
                ? string.Empty
                : $"ALTER TABLE {GetQuotedIdentifier(databaseType, tableName)} DROP COLUMN {GetQuotedIdentifier(databaseType, columnName)}";
        }
    }
}