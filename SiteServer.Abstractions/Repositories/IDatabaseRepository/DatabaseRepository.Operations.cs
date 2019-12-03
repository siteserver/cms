using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public partial interface IDatabaseRepository
    {
        Task<(bool IsSuccess, string ErrorMessage)> InstallDatabaseAsync(string adminName, string adminPassword, IList<IRepository> repositories);

        Task<(bool IsSuccess, Exception Ex)> CreateTableAsync(string tableName, IList<TableColumn> tableColumns, string pluginId, bool isContentTable);

        Task AlterTableAsync(string tableName, IList<TableColumn> tableColumns, string pluginId, IList<string> dropColumnNames = null);

        Task AlterSystemTableAsync(string tableName, IList<TableColumn> tableColumns, IList<string> dropColumnNames = null);

        Task SyncSystemTablesAsync(IList<IRepository> repositories);

        Task UpdateConfigVersionAsync();

        Task SyncDatabaseAsync(IList<IRepository> repositories);

        Task CreateContentTableAsync(string tableName, IList<TableColumn> tableColumns);

        void SyncContentTables();
    }
}
