using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiteServer.Utils;

namespace SiteServer.CMS.StlParser.Model
{
    public class StlElementInfo
    {
        public StlElementInfo(string name, Dictionary<string, string> attributesIgnoreCase, string outerHtml, string innerHtml)
        {
            Name = name;
            AttributesIgnoreCase = attributesIgnoreCase;
            OuterHtml = outerHtml;
            InnerHtml = innerHtml;

            IsDynamic = AttributesIgnoreCase.ContainsKey("isDynamic") &&
                        TranslateUtils.ToBool(AttributesIgnoreCase["isDynamic"]);
        }

        public bool IsDynamic { get; }

        // name is always lowerCase
        public string Name { get; }

        // attributesIgnoreCase is always not null
        public Dictionary<string, string> AttributesIgnoreCase { get; }

        public string OuterHtml { get; }

        public string InnerHtml { get; }
    }
}
