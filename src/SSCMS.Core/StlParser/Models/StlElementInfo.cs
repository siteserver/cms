using System.Collections.Specialized;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.Models
{
    public class StlElementInfo
    {
        public StlElementInfo(string name, NameValueCollection attributes, string outerHtml, string innerHtml, int startIndex)
        {
            Name = name;
            Attributes = attributes;
            OuterHtml = outerHtml;
            InnerHtml = innerHtml;
            StartIndex = startIndex;

            IsDynamic = TranslateUtils.ToBool(Attributes["isDynamic"]);
        }

        public bool IsDynamic { get; }

        // name is always lowerCase
        public string Name { get; }

        // attributesIgnoreCase is always not null
        public NameValueCollection Attributes { get; }

        public string OuterHtml { get; }

        public string InnerHtml { get; set; }

        public int StartIndex { get; }

        public override string ToString()
        {
            var attributes = TranslateUtils.ToAttributesString(Attributes);
            if (!string.IsNullOrEmpty(attributes))
            {
                attributes = " " + attributes;
            }
            return $"<{Name}{attributes}>{InnerHtml}</{Name}>";
        }
    }
}
