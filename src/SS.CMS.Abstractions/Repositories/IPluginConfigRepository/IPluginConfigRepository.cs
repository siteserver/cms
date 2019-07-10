using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
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
