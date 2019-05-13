using System;
using System.Collections.Specialized;
using System.Text;
using SiteServer.Utils;

namespace SiteServer.CMS.StlParser.Model
{
    public static partial class Html
    {
        public class Table : HtmlBase, IDisposable
        {
            public Table(StringBuilder sb, NameValueCollection attributes) : base(sb)
            {
                Append("<table");
                AddOptionalAttributes(attributes);
            }

            public void StartHead(NameValueCollection attributes = null)
            {
                Append("<thead");
                AddOptionalAttributes(attributes);
            }

            public void EndHead()
            {
                Append("</thead>");
            }

            public void StartFoot(NameValueCollection attributes = null)
            {
                Append("<tfoot");
                AddOptionalAttributes(attributes);
            }

            public void EndFoot()
            {
                Append("</tfoot>");
            }

            public void StartBody(NameValueCollection attributes = null)
            {
                Append("<tbody");
                AddOptionalAttributes(attributes);
            }

            public void EndBody()
            {
                Append("</tbody>");
            }

            public void Dispose()
            {
                Append("</table>");
            }

            public Row AddRow(NameValueCollection attributes = null)
            {
                return new Row(GetBuilder(), attributes);
            }
        }

        public class Row : HtmlBase, IDisposable
        {
            public Row(StringBuilder sb, NameValueCollection attributes) : base(sb)
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
}