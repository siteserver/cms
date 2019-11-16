using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.CMS.Plugin
{
    public static class PluginStlParserContentManager
    {
        public static async Task<Dictionary<string, Func<ParseContextImpl, string>>> GetParsesAsync()
        {
            var elementsToParse = new Dictionary<string, Func<ParseContextImpl, string>>();

            foreach (var service in await PluginManager.GetServicesAsync())
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
