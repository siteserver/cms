using System;
using System.Collections.Generic;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.CMS.Plugin
{
    public static class PluginStlParserContentManager
    {
        public static Dictionary<string, Func<ParseContextImpl, string>> GetParses()
        {
            var elementsToParse = new Dictionary<string, Func<ParseContextImpl, string>>();

            foreach (var service in PluginManager.Services)
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
