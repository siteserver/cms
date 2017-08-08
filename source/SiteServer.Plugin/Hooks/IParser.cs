using System.Collections.Generic;

namespace SiteServer.Plugin.Hooks
{
    public interface IParser: IPlugin
    {
        List<string> ElementNames { get; }

        string Parse(PluginParserContext context);
    }
}
