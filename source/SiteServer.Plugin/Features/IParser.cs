using System.Collections.Generic;

namespace SiteServer.Plugin.Features
{
    public interface IParser: IPlugin
    {
        List<string> ElementNames { get; }

        string Parse(PluginParserContext context);
    }
}
