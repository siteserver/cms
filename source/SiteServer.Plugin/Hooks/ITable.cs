using System.Collections.Generic;

namespace SiteServer.Plugin.Hooks
{
    public interface ITable : IPlugin
    {
        Dictionary<string, List<PluginTableColumn>> Tables { get; }
    }
}
