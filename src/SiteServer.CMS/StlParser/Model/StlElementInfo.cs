using System.Collections.Specialized;
using SiteServer.Utils;

namespace SiteServer.CMS.StlParser.Model
{
    public class StlElementInfo
    {
        public StlElementInfo(string name, NameValueCollection attributes, string outerHtml, string innerHtml)
        {
            Name = name;
            Attributes = attributes;
            OuterHtml = outerHtml;
            InnerHtml = innerHtml;

            IsDynamic = TranslateUtils.ToBool(Attributes["isDynamic"]);
        }

        public bool IsDynamic { get; }

        // name is always lowerCase
        public string Name { get; }

        // attributesIgnoreCase is always not null
        public NameValueCollection Attributes { get; }

        public string OuterHtml { get; }

        public string InnerHtml { get; }
    }
}
