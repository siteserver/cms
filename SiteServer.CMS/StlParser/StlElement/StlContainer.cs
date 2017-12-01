using System.Collections.Generic;
using System.Text;
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

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            // 如果是实体标签则返回空
            if (contextInfo.IsCurlyBrace)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(contextInfo.InnerXml))
            {
                return string.Empty;
            }

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeContext))
                {
                    contextInfo.ContextType = EContextTypeUtils.GetEnumType(value);
                }
            }

            var innerHtml = RegexUtils.GetInnerContent(ElementName, contextInfo.StlElement);

            var builder = new StringBuilder(innerHtml);
            StlParserManager.ParseInnerContent(builder, pageInfo, contextInfo);

            return builder.ToString();
        }
    }
}
