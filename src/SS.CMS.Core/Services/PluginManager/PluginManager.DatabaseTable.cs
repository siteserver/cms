using System.Collections.Generic;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class PluginManager
    {
        public void SyncTable(IService service)
        {
            if (service.DatabaseTables == null || service.DatabaseTables.Count <= 0) return;

            var db = new Db(_settingsManager.DatabaseType, _settingsManager.DatabaseConnectionString);

            foreach (var tableName in service.DatabaseTables.Keys)
            {
                var tableColumns = service.DatabaseTables[tableName];
                if (tableColumns == null || tableColumns.Count == 0) continue;

                if (!db.IsTableExists(tableName))
                {
                    db.CreateTable(tableName, tableColumns);
                }
                else
                {
                    db.AlterTable(tableName, tableColumns, null);
                }
            }
        }
    }
}
