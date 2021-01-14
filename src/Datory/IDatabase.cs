using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Datory
{
    public interface IDatabase
    {
        DatabaseType DatabaseType { get; }

        string ConnectionString { get; }

        string DatabaseName { get; }

        DbConnection GetConnection();

        Task<bool> IsTableExistsAsync(string tableName);

        Task<string> AddIdentityColumnIdIfNotExistsAsync(string tableName, List<TableColumn> columns);

        Task AlterTableAsync(string tableName, IEnumerable<TableColumn> tableColumns, IEnumerable<string> dropColumnNames = null);

        Task CreateTableAsync(string tableName, IEnumerable<TableColumn> tableColumns);

        Task CreateIndexAsync(string tableName, string indexName, params string[] columns);

        Task<List<string>> GetColumnNamesAsync(string tableName);

        string GetTableName<T>() where T : Entity;

        List<TableColumn> GetTableColumns<T>() where T : Entity;

        List<TableColumn> GetTableColumns(IEnumerable<TableColumn> tableColumns);

        Task<List<TableColumn>> GetTableColumnsAsync(string tableName);

        Task DropTableAsync(string tableName);

        Task<List<string>> GetDatabaseNamesAsync();

        Task<List<string>> GetTableNamesAsync();

        Task<(bool IsConnectionWorks, string ErrorMessage)> IsConnectionWorksAsync();

        string GetQuotedIdentifier(string identifier);
    }
}
