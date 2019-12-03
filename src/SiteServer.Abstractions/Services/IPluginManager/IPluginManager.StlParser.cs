using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IPluginManager
    {
        Task<Dictionary<string, Func<IParseContext, string>>> GetParsesAsync();
    }
}
