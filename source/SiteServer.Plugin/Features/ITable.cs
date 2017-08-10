using System.Collections.Generic;

namespace SiteServer.Plugin.Features
{
    public interface ITable : IPlugin
    {
        Dictionary<string, List<PluginTableColumn>> Tables { get; }
    }
}
