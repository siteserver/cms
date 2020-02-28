using Datory;
using SS.CMS.Core;
using SS.CMS.Plugins.Impl;

namespace SS.CMS.Plugins
{
    public static class PluginDatabaseTableManager
    {
        public static async void SyncTable(ServiceImpl service)
        {
            if (service.DatabaseTables == null || service.DatabaseTables.Count <= 0) return;

            foreach (var tableName in service.DatabaseTables.Keys)
            {
                var tableColumns = service.DatabaseTables[tableName];
                if (tableColumns == null || tableColumns.Count == 0) continue;

                if (!await GlobalSettings.Database.IsTableExistsAsync(tableName))
                {
                    await GlobalSettings.Database.CreateTableAsync(tableName, tableColumns);
                }
                else
                {
                    await GlobalSettings.Database.AlterTableAsync(tableName, tableColumns);
                }
            }
        }
    }
}
