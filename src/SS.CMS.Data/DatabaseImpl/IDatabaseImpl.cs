using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using SqlKata.Compilers;

namespace SS.CMS.Data.DatabaseImpl
{
    public interface IDatabaseImpl
    {
        DbConnection GetConnection(string connectionString);

        Compiler GetCompiler(string connectionString);

        bool IsUseLegacyPagination(string connectionString);

        Task<IList<TableColumn>> GetTableColumnsAsync(string connectionString, string tableName);

        Task<IList<string>> GetDatabaseNamesAsync(string connectionString);

        Task<IList<string>> GetTableNamesAsync(string connectionString);

        string ColumnIncrement(string columnName, int plusNum = 1);

        string ColumnDecrement(string columnName, int minusNum = 1);

        string GetAutoIncrementDataType(bool alterTable = false);

        string GetColumnSqlString(TableColumn tableColumn);

        string GetPrimaryKeySqlString(string tableName, string attributeName);

        string GetQuotedIdentifier(string identifier);

        string GetAddColumnsSqlString(string tableName, string columnsSqlString);
    }
}