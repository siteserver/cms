using System;
using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Features
{
    public interface IMenu : IPlugin
    {
        Func<PluginMenu> GlobalMenu { get; }

        Func<int, PluginMenu> Menu { get; }
    }
}
