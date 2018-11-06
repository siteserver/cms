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

                if (!DataProvider.DatabaseDao.IsTableExists(tableName))
                {
                    DataProvider.DatabaseDao.CreatePluginTable(service.PluginId, tableName, tableColumns);
                }
                else
                {
                    DataProvider.DatabaseDao.AlterPluginTable(service.PluginId, tableName, tableColumns);
                }
            }
        }
    }
}
