using System;
using System.Collections.Specialized;
using System.Text;

namespace SSCMS.Core.StlParser.Mocks
{
    public class HtmlTable : HtmlBase, IDisposable
    {
        public HtmlTable(StringBuilder sb, NameValueCollection attributes) : base(sb)
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

        public HtmlRow AddRow(NameValueCollection attributes = null)
        {
            return new HtmlRow(GetBuilder(), attributes);
        }
    }
}