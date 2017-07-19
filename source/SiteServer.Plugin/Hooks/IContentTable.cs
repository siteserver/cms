using System.Collections.Generic;

namespace SiteServer.Plugin.Hooks
{
    public interface IContentTable : IHooks
    {
        List<PluginTableColumn> ContentTableColumns { get; }
    }
}
