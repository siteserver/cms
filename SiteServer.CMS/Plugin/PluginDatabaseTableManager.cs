using SiteServer.CMS.Apis;
using SiteServer.CMS.Database.Core;
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

                if (!DatabaseApi.Instance.IsTableExists(tableName))
                {
                    DatabaseApi.Instance.CreateTable(tableName, tableColumns, service.PluginId, false, out _, out _);
                }
                else
                {
                    DatabaseApi.Instance.AlterTable(tableName, tableColumns, service.PluginId);
                }
            }
        }
    }
}
