using System.Threading.Tasks;
using Datory;

namespace SSCMS
{
    public interface IPluginConfigRepository : IRepository
    {
        Task InsertAsync(PluginConfig config);

        Task DeleteAsync(string pluginId, int siteId, string configName);

        Task UpdateAsync(PluginConfig configInfo);

        Task<string> GetValueAsync(string pluginId, int siteId, string configName);

        Task<bool> IsExistsAsync(string pluginId, int siteId, string configName);
    }
}