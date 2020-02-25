using System;
using System.Collections.Specialized;
using System.Text;

namespace SS.CMS.StlParser.Mock
{
    public class HtmlRow : HtmlBase, IDisposable
    {
        public HtmlRow(StringBuilder sb, NameValueCollection attributes) : base(sb)
        {
            Append("<tr");
            AddOptionalAttributes(attributes);
        }

        public void Dispose()
        {
            Append("</tr>");
        }

        public void AddCell(string innerText, NameValueCollection attributes)
        {
            Append("<td");
            AddOptionalAttributes(attributes);
            Append(innerText);
            Append("</td>");
        }
    }
}