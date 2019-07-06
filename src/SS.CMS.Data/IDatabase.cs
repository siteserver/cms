using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace SS.CMS.Data
{
    public interface IDatabase
    {
        DatabaseType DatabaseType { get; }

        string ConnectionString { get; }

        string DatabaseName { get; }

        DbConnection GetConnection();

        Task<bool> IsTableExistsAsync(string tableName);

        Task<string> AddIdentityColumnIdIfNotExistsAsync(string tableName, IList<TableColumn> columns);

        Task AlterTableAsync(string tableName, IList<TableColumn> tableColumns, IList<string> dropColumnNames = null);

        Task CreateTableAsync(string tableName, IList<TableColumn> tableColumns);

        Task CreateIndexAsync(string tableName, string indexName, params string[] columns);

        Task<List<string>> GetColumnNamesAsync(string tableName);

        string GetTableName<T>() where T : Entity;

        List<TableColumn> GetTableColumns<T>() where T : Entity;

        Task DropTableAsync(string tableName);

        Task<IList<TableColumn>> GetTableColumnsAsync(string tableName);

        Task<IList<string>> GetDatabaseNamesAsync();

        Task<IList<string>> GetTableNamesAsync();

        (bool IsConnectionWorks, string ErrorMessage) IsConnectionWorks();
    }
}
