using System;
using System.Collections.Generic;
using SSCMS.Context;

namespace SSCMS.Services
{
    public partial interface IOldPluginManager
    {
        Dictionary<string, Func<IParseContext, string>> GetParses();
    }
}
