using System;
using System.Collections.Generic;

namespace SSCMS.Core.Services
{
    public partial class OldPluginManager
    {
        public Dictionary<string, Func<IStlParseContext, string>> GetParses()
        {
            var elementsToParse = new Dictionary<string, Func<IStlParseContext, string>>();

            foreach (var plugin in GetPlugins())
            {
                if (plugin.StlElementsToParse != null && plugin.StlElementsToParse.Count > 0)
                {
                    foreach (var elementName in plugin.StlElementsToParse.Keys)
                    {
                        elementsToParse[elementName.ToLower()] = plugin.StlElementsToParse[elementName];
                    }
                }
            }

            return elementsToParse;
        }
    }
}
