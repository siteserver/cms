using System.Collections.Generic;
using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Features
{
    public interface ITable : IPlugin
    {
        Dictionary<string, List<PluginTableColumn>> Tables { get; }
    }
}
