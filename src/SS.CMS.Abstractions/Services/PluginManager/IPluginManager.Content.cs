using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IPluginManager
    {
        Task<List<IPackageMetadata>> GetContentModelPluginsAsync();

        Task<List<string>> GetContentTableNameListAsync();

        Task<List<IPackageMetadata>> GetAllContentRelatedPluginsAsync(bool includeContentTable);

        Task<List<IPluginService>> GetContentPluginsAsync(Channel channel, bool includeContentTable);

        List<string> GetContentPluginIds(Channel channel);

        Task<Dictionary<string, Dictionary<string, Func<IContentContext, string>>>> GetContentColumnsAsync(List<string> pluginIds);
    }
}
