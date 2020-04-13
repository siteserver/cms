using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Plugins;

namespace SSCMS.Services
{
    public partial interface IOldPluginManager
    {
        void SyncTable(IOldPlugin pluginService);

        bool IsContentTable(IOldPlugin pluginService);

        string GetTableName(string pluginId);

        Task SyncContentTableAsync(IOldPlugin pluginService);

        List<IPackageMetadata> GetContentModelPlugins();

        List<string> GetContentTableNameList();

        List<IPackageMetadata> GetAllContentRelatedPlugins(bool includeContentTable);

        List<IOldPlugin> GetContentPlugins(Channel channel, bool includeContentTable);

        List<string> GetContentPluginIds(Channel channel);

        Dictionary<string, Dictionary<string, Func<IContentContext, string>>> GetContentColumns(List<string> pluginIds);
    }
}
