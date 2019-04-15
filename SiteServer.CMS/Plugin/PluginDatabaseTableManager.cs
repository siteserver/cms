using Datory;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.CMS.Plugin
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

                if (!DatoryUtils.IsTableExists(DataProvider.Database, tableName))
                {
                    DatoryUtils.CreateTable(DataProvider.Database, tableName, tableColumns);
                }
                else
                {
                    DatoryUtils.AlterTable(DataProvider.Database, tableName, tableColumns, null);
                }
            }
        }
    }
}
