using System;
using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Features
{
    public interface IMenu : IPlugin
    {
        PluginMenu PluginMenu { get; }

        Func<int, PluginMenu> SiteMenu { get; }
    }
}
