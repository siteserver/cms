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

        bool IsTableExists(string tableName);

        Task<bool> IsTableExistsAsync(string tableName);

        string AddIdentityColumnIdIfNotExists(string tableName, List<TableColumn> columns);

        void AlterTable(string tableName, IList<TableColumn> tableColumns, IList<string> dropColumnNames = null);

        Task AlterTableAsync(string tableName, IList<TableColumn> tableColumns, IList<string> dropColumnNames = null);

        void CreateTable(string tableName, List<TableColumn> tableColumns);

        Task CreateTableAsync(string tableName, List<TableColumn> tableColumns);

        void CreateIndex(string tableName, string indexName, params string[] columns);

        Task CreateIndexAsync(string tableName, string indexName, params string[] columns);

        List<string> GetColumnNames(string tableName);

        string GetTableName<T>() where T : Entity;

        List<TableColumn> GetTableColumns<T>() where T : Entity;

        void DropTable(string tableName);

        Task DropTableAsync(string tableName);

        DbConnection GetConnection();

        List<TableColumn> GetTableColumns(string tableName);

        List<string> GetTableNames();

        Task<(bool IsConnectionWorks, string ErrorMessage)> IsConnectionWorksAsync();
    }
}
