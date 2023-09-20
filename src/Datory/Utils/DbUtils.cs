using SqlKata.Compilers;
using Datory.DatabaseImpl;

namespace Datory.Utils
{
    public static class DbUtils
    {
        public const int VarCharDefaultLength = 500;
        public const string LocalDbHostVirtualPath = "~/database.sqlite";
        public const string LocalDbContainerVirtualPath = "~/wwwroot/sitefiles/database.sqlite";

        public static string GetConnectionString(DatabaseType databaseType, string server, bool isDefaultPort, int port, string userName, string password, string databaseName)
        {
            var connectionString = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                connectionString = MySqlImpl.Instance.GetConnectionString(server, isDefaultPort, port, userName, password, databaseName);
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                connectionString = SqlServerImpl.Instance.GetConnectionString(server, isDefaultPort, port, userName, password, databaseName);
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                connectionString = PostgreSqlImpl.Instance.GetConnectionString(server, isDefaultPort, port, userName, password, databaseName);
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                connectionString = SQLiteImpl.Instance.GetConnectionString(server, isDefaultPort, port, userName, password, databaseName);
            }
            else if (databaseType == DatabaseType.Gauss)
            {
                connectionString = GaussImpl.Instance.GetConnectionString(server, isDefaultPort, port, userName, password, databaseName);
            }
            else if (databaseType == DatabaseType.Dm)
            {
                connectionString = DmImpl.Instance.GetConnectionString(server, isDefaultPort, port, userName, password, databaseName);
            }
            else if (databaseType == DatabaseType.KingbaseES)
            {
                connectionString = KingbaseESImpl.Instance.GetConnectionString(server, isDefaultPort, port, userName, password, databaseName);
            }

            return connectionString;
        }

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
            else if (databaseType == DatabaseType.Gauss)
            {
                compiler = GaussImpl.Instance.GetCompiler(connectionString);
            }
            else if (databaseType == DatabaseType.Dm)
            {
                compiler = DmImpl.Instance.GetCompiler(connectionString);
            }
            else if (databaseType == DatabaseType.KingbaseES)
            {
                compiler = KingbaseESImpl.Instance.GetCompiler(connectionString);
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
            else if (databaseType == DatabaseType.Gauss)
            {
                retVal = GaussImpl.Instance.ColumnIncrement(columnName, plusNum);
            }
            else if (databaseType == DatabaseType.Dm)
            {
                retVal = DmImpl.Instance.ColumnIncrement(columnName, plusNum);
            }
            else if (databaseType == DatabaseType.KingbaseES)
            {
                retVal = KingbaseESImpl.Instance.ColumnIncrement(columnName, plusNum);
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
            else if (databaseType == DatabaseType.Gauss)
            {
                retVal = GaussImpl.Instance.ColumnDecrement(columnName, minusNum);
            }
            else if (databaseType == DatabaseType.Dm)
            {
                retVal = DmImpl.Instance.ColumnDecrement(columnName, minusNum);
            }
            else if (databaseType == DatabaseType.KingbaseES)
            {
                retVal = KingbaseESImpl.Instance.ColumnDecrement(columnName, minusNum);
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
            else if (databaseType == DatabaseType.Gauss)
            {
                retVal = GaussImpl.Instance.GetAutoIncrementDataType(alterTable);
            }
            else if (databaseType == DatabaseType.Dm)
            {
                retVal = DmImpl.Instance.GetAutoIncrementDataType(alterTable);
            }
            else if (databaseType == DatabaseType.KingbaseES)
            {
                retVal = KingbaseESImpl.Instance.GetAutoIncrementDataType(alterTable);
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
            else if (databaseType == DatabaseType.Gauss)
            {
                retVal = GaussImpl.Instance.GetColumnSqlString(tableColumn);
            }
            else if (databaseType == DatabaseType.Dm)
            {
                retVal = DmImpl.Instance.GetColumnSqlString(tableColumn);
            }
            else if (databaseType == DatabaseType.KingbaseES)
            {
                retVal = KingbaseESImpl.Instance.GetColumnSqlString(tableColumn);
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
            else if (databaseType == DatabaseType.Gauss)
            {
                retVal = GaussImpl.Instance.GetPrimaryKeySqlString(tableName, attributeName);
            }
            else if (databaseType == DatabaseType.Dm)
            {
                retVal = DmImpl.Instance.GetPrimaryKeySqlString(tableName, attributeName);
            }
            else if (databaseType == DatabaseType.KingbaseES)
            {
                retVal = KingbaseESImpl.Instance.GetPrimaryKeySqlString(tableName, attributeName);
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
            else if (databaseType == DatabaseType.Gauss)
            {
                retVal = GaussImpl.Instance.GetQuotedIdentifier(identifier);
            }
            else if (databaseType == DatabaseType.Dm)
            {
                retVal = DmImpl.Instance.GetQuotedIdentifier(identifier);
            }
            else if (databaseType == DatabaseType.KingbaseES)
            {
                retVal = KingbaseESImpl.Instance.GetQuotedIdentifier(identifier);
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
            else if (databaseType == DatabaseType.Gauss)
            {
                retVal = GaussImpl.Instance.GetAddColumnsSqlString(tableName, columnsSqlString);
            }
            else if (databaseType == DatabaseType.Dm)
            {
                retVal = DmImpl.Instance.GetAddColumnsSqlString(tableName, columnsSqlString);
            }
            else if (databaseType == DatabaseType.KingbaseES)
            {
                retVal = KingbaseESImpl.Instance.GetAddColumnsSqlString(tableName, columnsSqlString);
            }

            return retVal;
        }

        internal static string GetDropColumnsSqlString(DatabaseType databaseType, string tableName, string columnName)
        {
            return databaseType == DatabaseType.SQLite
                ? string.Empty
                : $"ALTER TABLE {GetQuotedIdentifier(databaseType, tableName)} DROP COLUMN {GetQuotedIdentifier(databaseType, columnName)}";
        }

        public static string GetOrderByRandomString(DatabaseType databaseType)
        {
            var orderBy = string.Empty;

            if (databaseType == DatabaseType.MySql)
            {
                orderBy = "RAND()";
            }
            else if (databaseType == DatabaseType.SqlServer)
            {
                orderBy = "NEWID()";
            }
            else if (databaseType == DatabaseType.PostgreSql)
            {
                orderBy = "random()";
            }
            else if (databaseType == DatabaseType.SQLite)
            {
                orderBy = "RANDOM()";
            }
            else if (databaseType == DatabaseType.Gauss)
            {
                orderBy = "random()";
            }
            else if (databaseType == DatabaseType.Dm)
            {
                orderBy = "RAND()";
            }
            else if (databaseType == DatabaseType.KingbaseES)
            {
                orderBy = "random()";
            }

            return orderBy;
        }

        public static string GetInStr(IDatabase database, string columnName, string inStr)
        {
            var retVal = string.Empty;
            inStr = Utilities.FilterSql(inStr);
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
            else if (database.DatabaseType == DatabaseType.SQLite)
            {
                retVal = $"INSTR({columnName}, '{inStr}') > 0";
            }
            else if (database.DatabaseType == DatabaseType.Gauss)
            {
                retVal = $"POSITION('{inStr}' IN {columnName}) > 0";
            }
            else if (database.DatabaseType == DatabaseType.Dm)
            {
                retVal = $"INSTR({columnName}, '{inStr}') > 0";
            }
            else if (database.DatabaseType == DatabaseType.KingbaseES)
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
            else if (database.DatabaseType == DatabaseType.SQLite)
            {
                retVal = $"INSTR({columnName}, '{inStr}') = 0";
            }
            else if (database.DatabaseType == DatabaseType.Gauss)
            {
                retVal = $"POSITION('{inStr}' IN {columnName}) = 0";
            }
            else if (database.DatabaseType == DatabaseType.Dm)
            {
                retVal = $"INSTR({columnName}, '{inStr}') = 0";
            }
            else if (database.DatabaseType == DatabaseType.KingbaseES)
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