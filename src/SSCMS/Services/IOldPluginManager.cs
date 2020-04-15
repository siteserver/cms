using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Plugins;

namespace SSCMS.Services
{
    public partial interface IOldPluginManager
    {
        List<IOldPlugin> GetPlugins();

        IOldPlugin GetPlugin(string pluginId);

        bool IsExists(string pluginId);

        void Delete(string pluginId);

        List<string> PackagesIdAndVersionList { get; }

        //Task UpdateDisabledAsync(string pluginId, bool isDisabled);

        //Task UpdateTaxisAsync(string pluginId, int taxis);

        string GetPluginIconUrl(IOldPlugin pluginService);
    }
}
