using System.Threading.Tasks;
using SS.CMS.Data;

namespace SS.CMS.Core.Services
{
    public partial class PluginManager
    {
        public async Task SyncTableAsync(IService service)
        {
            if (service.DatabaseTables == null || service.DatabaseTables.Count <= 0) return;

            var db = new Database(_settingsManager.DatabaseType, _settingsManager.DatabaseConnectionString);

            foreach (var tableName in service.DatabaseTables.Keys)
            {
                var tableColumns = service.DatabaseTables[tableName];
                if (tableColumns == null || tableColumns.Count == 0) continue;

                if (!db.IsTableExists(tableName))
                {
                    await db.CreateTableAsync(tableName, tableColumns);
                }
                else
                {
                    await db.AlterTableAsync(tableName, tableColumns, null);
                }
            }
        }
    }
}
