using System;
using System.Collections.Generic;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface IPluginManager
    {
        bool IsContentTable(IService service);

        string GetContentTableName(string pluginId);

        List<IPackageMetadata> GetContentModelPlugins();

        List<string> GetContentTableNameList();

        List<IPackageMetadata> GetAllContentRelatedPlugins(bool includeContentTable);

        List<IService> GetContentPlugins(ChannelInfo channelInfo, bool includeContentTable);

        List<string> GetContentPluginIds(ChannelInfo channelInfo);

        Dictionary<string, Dictionary<string, Func<IContentContext, string>>> GetContentColumns(List<string> pluginIds);
    }
}
