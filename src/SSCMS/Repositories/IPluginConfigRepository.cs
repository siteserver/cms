using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IPluginConfigRepository : IRepository
    {
        Task InsertAsync(PluginConfig config);

        Task DeleteAsync(string pluginId, int siteId, string configName);

        Task UpdateAsync(PluginConfig configInfo);

        Task<string> GetValueAsync(string pluginId, int siteId, string configName);

        Task<bool> IsExistsAsync(string pluginId, int siteId, string configName);

        Task<bool> SetConfigAsync(string pluginId, int siteId, object config);

        Task<bool> SetConfigAsync(string pluginId, int siteId, string name, object config);

        Task<T> GetConfigAsync<T>(string pluginId, int siteId, string name = "");

        Task<bool> RemoveConfigAsync(string pluginId, int siteId, string name = "");
    }
}