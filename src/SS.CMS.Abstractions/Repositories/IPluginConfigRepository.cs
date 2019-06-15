using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface IPluginConfigRepository : IRepository
    {
        void Insert(PluginConfigInfo configInfo);

        void Delete(string pluginId, int siteId, string configName);

        void Update(PluginConfigInfo configInfo);

        string GetValue(string pluginId, int siteId, string configName);

        bool IsExists(string pluginId, int siteId, string configName);
    }
}
