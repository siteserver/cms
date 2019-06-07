using SS.CMS.Core.Plugin.Impl;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Plugin
{
    public static class PluginDatabaseTableManager
    {
        public static void SyncTable(ServiceImpl service)
        {
            if (service.DatabaseTables == null || service.DatabaseTables.Count <= 0) return;

            foreach (var tableName in service.DatabaseTables.Keys)
            {
                var tableColumns = service.DatabaseTables[tableName];
                if (tableColumns == null || tableColumns.Count == 0) continue;

                if (!AppSettings.DbContext.IsTableExists(tableName))
                {
                    AppSettings.DbContext.CreateTable(tableName, tableColumns);
                }
                else
                {
                    AppSettings.DbContext.AlterTable(tableName, tableColumns, null);
                }
            }
        }
    }
}
