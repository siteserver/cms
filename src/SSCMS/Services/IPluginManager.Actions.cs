using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IPluginManager
    {
        Task InstallAsync(string userName, string name, string version);

        void UnInstall(string pluginId);
    }
}
