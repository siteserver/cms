using System;
using System.Collections.Generic;
using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Features
{
    public interface IChannel: IPlugin
    {
        List<PluginContentLink> ContentLinks { get; }

        Action<int, int, int> OnContentAdded { get; }

        Action<int, int, int, int, int, int> OnContentTranslated { get; }

        Action<int, int, int> OnContentDeleted { get; }
    }
}
