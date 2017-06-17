using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "容器", Description = "通过 stl:container 标签在模板中定义容器，切换上下文")]
    public class StlContainer
    {
        private StlContainer() { }
        public const string ElementName = "stl:container";

        public const string AttributeContext = "context";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeContext, "所处上下文"}
        };

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

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeContext))
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
