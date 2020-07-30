using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IPluginManager
    {
        List<IPlugin> Plugins { get; }
        List<IPlugin> EnabledPlugins { get; }
        List<IPlugin> NetCorePlugins { get; }
        IPlugin Current { get; }
        IPlugin GetPlugin(string pluginId);
        void Load();
        IEnumerable<T> GetExtensions<T>(bool useCaching = true) where T : IPluginExtension;
        Task<Dictionary<string, object>> GetConfigAsync(string pluginId);
        Task SaveConfigAsync(string pluginId, Dictionary<string, object> config);
    }
}
