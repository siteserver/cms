using System;
using System.Collections.Specialized;
using System.Text;
using SiteServer.Utils;

namespace SiteServer.CMS.StlParser.Model
{
    public static partial class Html
    {
        public class Select : HtmlBase, IDisposable
        {
            public Select(StringBuilder sb, NameValueCollection attributes) : base(sb)
            {
                Append("<select");
                AddOptionalAttributes(attributes);
            }

            public void Dispose()
            {
                Append("</select>");
            }

            public void AddOption(string text, string value, bool selected = false)
            {
                Append("<option");
                var attributes = new NameValueCollection
                {
                    {"value", value}
                };
                if (selected)
                {
                    attributes["selected"] = "selected";
                }
                AddOptionalAttributes(attributes);
                Append(text);
                Append("</option>");
            }
        }

    }
}