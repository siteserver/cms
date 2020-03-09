using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Abstractions;

namespace SS.CMS.Services
{
    public partial class PluginManager
    {
        public Dictionary<string, Func<IParseContext, string>> GetParses()
        {
            var elementsToParse = new Dictionary<string, Func<IParseContext, string>>();

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
