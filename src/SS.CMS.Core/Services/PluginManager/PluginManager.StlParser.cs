using System;
using System.Collections.Generic;

namespace SS.CMS.Core.Services
{
    public partial class PluginManager
    {
        public Dictionary<string, Func<IParseContext, string>> GetParses()
        {
            var elementsToParse = new Dictionary<string, Func<IParseContext, string>>();

            foreach (var service in Services)
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
