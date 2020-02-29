using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Abstractions;

namespace SS.CMS.Services
{
    public partial class PluginManager
    {
        public async Task<Dictionary<string, Func<IParseContext, string>>> GetParsesAsync()
        {
            var elementsToParse = new Dictionary<string, Func<IParseContext, string>>();

            foreach (var service in await GetServicesAsync())
            {
                if (service.StlElementsToParse != null && service.StlElementsToParse.Count > 0)
                {
                    foreach (var elementName in service.StlElementsToParse.Keys)
                    {
                        elementsToParse[elementName.ToLower()] = service.StlElementsToParse[elementName];
                    }
                }
            }

            return elementsToParse;
        }
    }
}
