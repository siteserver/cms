using Datory;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

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

                if (!DatoryUtils.IsTableExists(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, tableName))
                {
                    DatoryUtils.CreateTable(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, tableName, tableColumns);
                }
                else
                {
                    DatoryUtils.AlterTable(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, tableName, tableColumns, null);
                }
            }
        }
    }
}
