using System.Collections.Specialized;
using SSCMS.Parse;
using SSCMS.Services;

namespace SSCMS.Core.Context
{
    public class PluginParseStlContext : PluginParseContext, IParseStlContext
    {
        public PluginParseStlContext(IParseManager parseManager, string stlOuterHtml, string stlInnerHtml, NameValueCollection stlAttributes) : base(parseManager)
        {
            StlOuterHtml = stlOuterHtml;
            StlInnerHtml = stlInnerHtml;
            StlAttributes = stlAttributes;
        }

        public string StlOuterHtml { get; }

        public string StlInnerHtml { get; }

        public NameValueCollection StlAttributes { get; }

        public bool IsStlEntity => ParseManager.ContextInfo.IsStlEntity;
    }
}
