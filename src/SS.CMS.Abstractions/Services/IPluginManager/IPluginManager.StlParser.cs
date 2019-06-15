using System;
using System.Collections.Generic;

namespace SS.CMS.Abstractions.Services
{
    public partial interface IPluginManager
    {
        Dictionary<string, Func<IParseContext, string>> GetParses();
    }
}
