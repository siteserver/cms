using System;
using System.Collections.Generic;
using SSCMS.Context;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class OldPluginManager
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
                        elementsToParse[StringUtils.ToLower(elementName)] = plugin.StlElementsToParse[elementName];
                    }
                }
            }

            return elementsToParse;
        }
    }
}
