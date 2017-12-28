using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Features
{
    public interface IContentRelated: IPlugin
    {
        List<PluginContentLink> ContentLinks { get; }

        Action<int, int, int> ContentAddCompleted { get; }

        Action<int, int, int, int, int, int> ContentTranslateCompleted { get; }

        Action<int, int, int> ContentDeleteCompleted { get; }

        Dictionary<string, Func<int, int, IAttributes, string>> ContentFormCustomized { get; }

        Action<int, int, IContentInfo, NameValueCollection> ContentFormSubmited { get; }
    }
}
