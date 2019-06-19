using System;
using System.Collections.Generic;

namespace SS.CMS.Services.IPluginManager
{
    public partial interface IPluginManager
    {
        Dictionary<string, Func<IParseContext, string>> GetParses();
    }
}
