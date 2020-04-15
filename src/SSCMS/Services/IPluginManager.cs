using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using SSCMS.Plugins;

namespace SSCMS.Services
{
    public partial interface IPluginManager
    {
        string DirectoryPath { get; }
        IEnumerable<IPlugin> Plugins { get; }
        IPlugin GetPlugin(string pluginId);
        IEnumerable<Assembly> Assemblies { get; }
        public void Reload();

        Task SaveConfigAsync(string pluginId, Dictionary<string, object> config);
    }
}
