using System;
using System.Collections.Generic;
using SS.CMS.Abstractions.Models;

namespace SS.CMS.Abstractions.Services
{
    public partial interface IPluginManager
    {
        List<IPackageMetadata> GetContentModelPlugins();

        List<string> GetContentTableNameList();

        List<IPackageMetadata> GetAllContentRelatedPlugins(bool includeContentTable);

        List<IService> GetContentPlugins(ChannelInfo channelInfo, bool includeContentTable);

        List<string> GetContentPluginIds(ChannelInfo channelInfo);

        Dictionary<string, Dictionary<string, Func<IContentContext, string>>> GetContentColumns(List<string> pluginIds);
    }
}
