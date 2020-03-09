using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IPluginManager
    {
        List<IPlugin> GetPlugins();

        IPlugin GetPlugin(string pluginId);

        bool IsExists(string pluginId);

        void Delete(string pluginId);

        List<string> PackagesIdAndVersionList { get; }

        Task UpdateDisabledAsync(string pluginId, bool isDisabled);

        Task UpdateTaxisAsync(string pluginId, int taxis);

        string GetPluginIconUrl(IPlugin pluginService);
    }
}
