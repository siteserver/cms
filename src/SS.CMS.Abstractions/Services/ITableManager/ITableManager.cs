using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;

namespace SS.CMS.Services
{
    public partial interface ITableManager
    {
        Task<IEnumerable<TableColumn>> GetTableColumnInfoListAsync(string tableName);

        Task<IEnumerable<TableColumn>> GetTableColumnInfoListAsync(string tableName, List<string> excludeAttributeNameList);

        Task<IEnumerable<TableColumn>> GetTableColumnInfoListAsync(string tableName, DataType excludeDataType);

        Task<TableColumn> GetTableColumnInfoAsync(string tableName, string attributeName);

        Task<bool> IsAttributeNameExistsAsync(string tableName, string attributeName);

        Task<List<string>> GetTableColumnNameListAsync(string tableName);

        Task<List<string>> GetTableColumnNameListAsync(string tableName, List<string> excludeAttributeNameList);

        Task<List<string>> GetTableColumnNameListAsync(string tableName, DataType excludeDataType);

        Task<(bool IsSuccess, Exception Ex)> CreateTableAsync(string tableName, IList<TableColumn> tableColumns, string pluginId, bool isContentTable);

        Task AlterTableAsync(string tableName, IList<TableColumn> tableColumns, string pluginId, IList<string> dropColumnNames = null);

        Task CreateContentTableAsync(string tableName, IList<TableColumn> tableColumns);

        Task AlterSystemTableAsync(string tableName, IList<TableColumn> tableColumns, IList<string> dropColumnNames = null);

        void InstallDatabase(string userName, string password);

        Task SyncSystemTablesAsync();

        void SyncContentTables();

        Task UpdateConfigVersionAsync();

        Task SyncDatabaseAsync();
    }
}
