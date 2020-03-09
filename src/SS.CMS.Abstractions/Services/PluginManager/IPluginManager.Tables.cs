using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IPluginManager
    {
        void SyncTable(IPlugin pluginService);

        bool IsContentTable(IPlugin pluginService);

        string GetTableName(string pluginId);

        Task SyncContentTableAsync(IPlugin pluginService);

        List<IPackageMetadata> GetContentModelPlugins();

        List<string> GetContentTableNameList();

        List<IPackageMetadata> GetAllContentRelatedPlugins(bool includeContentTable);

        List<IPlugin> GetContentPlugins(Channel channel, bool includeContentTable);

        List<string> GetContentPluginIds(Channel channel);

        Dictionary<string, Dictionary<string, Func<IContentContext, string>>> GetContentColumns(List<string> pluginIds);
    }
}
