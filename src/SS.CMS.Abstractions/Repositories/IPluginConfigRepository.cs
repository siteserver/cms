using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IPluginConfigRepository : IRepository
    {
        void Insert(PluginConfigInfo configInfo);

        Task DeleteAsync(string pluginId, int siteId, string configName);

        void Update(PluginConfigInfo configInfo);

        string GetValue(string pluginId, int siteId, string configName);

        bool IsExists(string pluginId, int siteId, string configName);
    }
}
