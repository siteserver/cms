using SS.CMS.Core.Components;
using SS.CMS.Core.Settings;
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

                if (!AppContext.Db.IsTableExists(tableName))
                {
                    AppContext.Db.CreateTable(tableName, tableColumns);
                }
                else
                {
                    AppContext.Db.AlterTable(tableName, tableColumns, null);
                }
            }
        }
    }
}
