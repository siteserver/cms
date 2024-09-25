using SqlKata.Compilers;
using Datory.DatabaseImpl;

namespace Datory.Utils
{
    public static class DbUtils
    {
        public const int VarCharDefaultLength = 500;
        public const string LocalDbHostVirtualPath = "~/database.sqlite";
        public const string LocalDbContainerVirtualPath = "~/wwwroot/sitefiles/database.sqlite";

        public static IDatabaseImpl GetInstance(DatabaseType databaseType)
        {
            IDatabaseImpl instance = null;

            if (databaseType == DatabaseType.MySql || databaseType == DatabaseType.OceanBase)
            {
                instance = MySqlImpl.Instance;
            }
            else if (databaseType == DatabaseType.MariaDB)
            {
                instance = MariaDBImpl.Instance;
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                instance = SqlServerImpl.Instance;
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                instance = PostgreSqlImpl.Instance;
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                instance = SQLiteImpl.Instance;
            }
            else if (databaseType == DatabaseType.KingbaseES)
            {
                instance = KingbaseESImpl.Instance;
            }
            else if (databaseType == DatabaseType.Dm)
            {
                instance = DmImpl.Instance;
            }

            return instance;
        }

        public static string GetConnectionString(DatabaseType databaseType, string server, bool isDefaultPort, int port, string userName, string password, string databaseName)
        {
            return GetInstance(databaseType).GetConnectionString(server, isDefaultPort, port, userName, password, databaseName);
        }

        internal static Compiler GetCompiler(DatabaseType databaseType, string connectionString)
        {
            return GetInstance(databaseType).GetCompiler(connectionString);
        }

        internal static string ColumnIncrement(DatabaseType databaseType, string columnName, int plusNum = 1)
        {
            return GetInstance(databaseType).ColumnIncrement(columnName, plusNum);
        }

        internal static string ColumnDecrement(DatabaseType databaseType, string columnName, int minusNum = 1)
        {
            return GetInstance(databaseType).ColumnDecrement(columnName, minusNum);
        }

        internal static string GetAutoIncrementDataType(DatabaseType databaseType, bool alterTable = false)
        {
            return GetInstance(databaseType).GetAutoIncrementDataType(alterTable);
        }

        internal static string GetColumnSqlString(DatabaseType databaseType, TableColumn tableColumn)
        {
            return GetInstance(databaseType).GetColumnSqlString(tableColumn);
        }

        internal static string GetPrimaryKeySqlString(DatabaseType databaseType, string tableName, string attributeName)
        {
            return GetInstance(databaseType).GetPrimaryKeySqlString(tableName, attributeName);
        }

        internal static string GetQuotedIdentifier(DatabaseType databaseType, string identifier)
        {
            return GetInstance(databaseType).GetQuotedIdentifier(identifier);
        }

        internal static string GetAddColumnsSqlString(DatabaseType databaseType, string tableName, string columnsSqlString)
        {
            return GetInstance(databaseType).GetAddColumnsSqlString(tableName, columnsSqlString);
        }

        public static string GetOrderByRandomString(DatabaseType databaseType)
        {
            return GetInstance(databaseType).GetOrderByRandomString();
        }

        public static string GetInStr(DatabaseType databaseType, string columnName, string inStr)
        {
            return GetInstance(databaseType).GetInStr(columnName, inStr);
        }

        public static string GetNotInStr(DatabaseType databaseType, string columnName, string inStr)
        {
            return GetInstance(databaseType).GetNotInStr(columnName, inStr);
        }

        internal static string GetDropColumnsSqlString(DatabaseType databaseType, string tableName, string columnName)
        {
            return databaseType == DatabaseType.SQLite
                ? string.Empty
                : $"ALTER TABLE {GetQuotedIdentifier(databaseType, tableName)} DROP COLUMN {GetQuotedIdentifier(databaseType, columnName)}";
        }

        public static string ToTopSqlString(IDatabase database, string tableName, string columns, string whereString, string orderString, int topN)
        {
            tableName = database.GetQuotedIdentifier(tableName);

            var retVal = $"SELECT {columns} FROM {tableName} {whereString} {orderString}";
            if (topN <= 0) return retVal;

            if (database.DatabaseType == DatabaseType.SqlServer)
            {
                retVal = $"SELECT TOP {topN} {columns} FROM {tableName} {whereString} {orderString}";
            }
            else
            {
                retVal = $"SELECT {columns} FROM {tableName} {whereString} {orderString} LIMIT {topN}";
            }

            return retVal;
        }

        public static string ToSqlBool(DatabaseType databaseType, bool val)
        {
            if (databaseType == DatabaseType.SqlServer || databaseType == DatabaseType.Dm)
            {
                return val ? "1" : "0";
            }

            return val.ToString().ToLower();
        }
    }
}