using System;
using System.Collections.Generic;

namespace SSCMS.Services
{
    public partial interface IOldPluginManager
    {
        Dictionary<string, Func<IParseContext, string>> GetParses();
    }
}
