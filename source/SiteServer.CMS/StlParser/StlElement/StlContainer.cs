using System;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlContainer
    {
        private StlContainer() { }
        public const string ElementName = "stl:container";                  //容器

        public const string AttributeContext = "context";                  //所处上下文

        public static ListDictionary AttributeList => new ListDictionary
        {
            {AttributeContext, "所处上下文"}
        };

        public static string GetContainer(string content)
        {
            return $@"
<stl:container>
{content}
</stl:container>
";
        }

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfoRef)
        {
            var parsedContent = string.Empty;

            var contextInfo = contextInfoRef.Clone();
            try
            {
                var ie = node.Attributes?.GetEnumerator();
                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;
                        var attributeName = attr.Name.ToLower();
                        if (attributeName.Equals(AttributeContext))
                        {
                            contextInfo.ContextType = EContextTypeUtils.GetEnumType(attr.Value);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(node.InnerXml))
                {
                    var innerHtml = RegexUtils.GetInnerContent(ElementName, stlElement);

                    var builder = new StringBuilder(innerHtml);
                    StlParserManager.ParseInnerContent(builder, pageInfo, contextInfo);

                    parsedContent = builder.ToString();
                }
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }
    }
}
