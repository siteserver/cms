using System.Collections.Generic;
using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Features
{
    public interface IContentTable: IPlugin
    {
        string ContentTableName { get; }
        List<PluginTableColumn> ContentTableColumns { get; }
    }
}
