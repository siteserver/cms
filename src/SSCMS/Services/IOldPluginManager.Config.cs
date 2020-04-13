using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IOldPluginManager
    {
        Task<bool> SetConfigAsync(string pluginId, int siteId, object config);

        Task<bool> SetConfigAsync(string pluginId, int siteId, string name, object config);

        Task<T> GetConfigAsync<T>(string pluginId, int siteId, string name = "");

        Task<bool> RemoveConfigAsync(string pluginId, int siteId, string name = "");
    }
}
