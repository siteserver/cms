using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Plugins.Impl;

namespace SS.CMS.Plugins
{
    public static partial class PluginManager
    {
        public static async Task<Dictionary<string, Func<ParseContextImpl, string>>> GetParsesAsync()
        {
            var elementsToParse = new Dictionary<string, Func<ParseContextImpl, string>>();

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
