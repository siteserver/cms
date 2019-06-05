using System.Collections.Generic;
using System.Data;
using SqlKata.Compilers;

namespace SS.CMS.Data.Database
{
    internal interface IDatabase
    {
        IDbConnection GetConnection(string connectionString);

        Compiler GetCompiler(string connectionString);

        List<TableColumn> GetTableColumns(string connectionString, string tableName);

        List<string> GetTableNames(string connectionString);

        string ColumnIncrement(string columnName, int plusNum = 1);

        string ColumnDecrement(string columnName, int minusNum = 1);

        string GetAutoIncrementDataType(bool alterTable = false);

        string GetColumnSqlString(TableColumn tableColumn);

        string GetPrimaryKeySqlString(string tableName, string attributeName);

        string GetQuotedIdentifier(string identifier);

        string GetAddColumnsSqlString(string tableName, string columnsSqlString);
    }
}
