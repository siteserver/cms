using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IPluginManager
    {
        Dictionary<string, Func<IParseContext, string>> GetParses();
    }
}
