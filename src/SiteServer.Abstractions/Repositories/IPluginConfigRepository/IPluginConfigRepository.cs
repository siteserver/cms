using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public interface IPluginConfigRepository : IRepository
    {
        Task<int> InsertAsync(PluginConfig configInfo);

        Task DeleteAsync(string pluginId, int siteId, string configName);

        Task UpdateAsync(PluginConfig configInfo);

        Task<string> GetValueAsync(string pluginId, int siteId, string configName);

        Task<bool> IsExistsAsync(string pluginId, int siteId, string configName);
    }
}
