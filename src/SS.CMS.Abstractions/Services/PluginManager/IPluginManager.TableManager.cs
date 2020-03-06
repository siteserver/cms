using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IPluginManager
    {
        void SyncTable(IPluginService pluginService);

        bool IsContentTable(IPluginService pluginService);

        Task<string> GetTableNameAsync(string pluginId);

        Task SyncContentTableAsync(IPluginService pluginService);
    }
}
