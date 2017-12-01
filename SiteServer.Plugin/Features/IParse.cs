using System;
using System.Collections.Generic;
using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Features
{
    public interface IParse: IPlugin
    {
        Dictionary<string, Func<PluginParseContext, string>> ElementsToParse { get; }
    }
}
