using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using SqlKata.Compilers;

namespace Datory.DatabaseImpl
{
    public interface IDatabaseImpl
    {
        DbConnection GetConnection(string connectionString);

        Compiler GetCompiler(string connectionString);

        bool IsUseLegacyPagination(string connectionString);

        Task<List<TableColumn>> GetTableColumnsAsync(string connectionString, string tableName);

        Task<List<string>> GetDatabaseNamesAsync(string connectionString);

        Task<List<string>> GetTableNamesAsync(string connectionString);

        string ColumnIncrement(string columnName, int plusNum = 1);

        string ColumnDecrement(string columnName, int minusNum = 1);

        string GetAutoIncrementDataType(bool alterTable = false);

        string GetColumnSqlString(TableColumn tableColumn);

        string GetPrimaryKeySqlString(string tableName, string attributeName);

        string GetQuotedIdentifier(string identifier);

        string GetAddColumnsSqlString(string tableName, string columnsSqlString);
    }
}