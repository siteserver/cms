using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Configuration;

namespace SSCMS.Services
{
    public partial interface IPluginManager
    {
        Task DisableAsync(string pluginId, bool disabled);

        void Install(string pluginId, string version);

        void UnInstall(string pluginId);
    }
}
