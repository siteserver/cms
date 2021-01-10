using System.Collections.Specialized;
using System.Text;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.Mocks
{
    public abstract class HtmlBase
    {
        private readonly StringBuilder _sb;

        protected HtmlBase(StringBuilder sb)
        {
            _sb = sb;
        }

        public StringBuilder GetBuilder()
        {
            return _sb;
        }

        protected void Append(string toAppend)
        {
            _sb.Append(toAppend);
        }

        protected void AddOptionalAttributes(NameValueCollection attributes)
        {
            if (attributes != null && attributes.Count > 0)
            {
                _sb.Append(" ");
                _sb.Append(TranslateUtils.ToAttributesString(attributes));
            }
            _sb.Append(">");
        }
    }
}