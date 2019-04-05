using System.Linq;
using Datory;
using SiteServer.CMS.Apis;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Fx;
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

                var datoryColumns = tableColumns.Select(x => (TableColumn) x).ToList();

                if (!DatoryUtils.IsTableExists(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, tableName))
                {
                    TableColumnManager.CreateTable(tableName, datoryColumns, service.PluginId, false, out _);
                }
                else
                {
                    TableColumnManager.AlterTable(tableName, datoryColumns, service.PluginId);
                }
            }
        }
    }
}
