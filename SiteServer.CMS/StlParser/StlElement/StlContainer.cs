using System.Text;
using SiteServer.Utils;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "容器", Description = "通过 stl:container 标签在模板中定义容器，切换上下文")]
    public class StlContainer
    {
        private StlContainer() { }
        public const string ElementName = "stl:container";

        private static readonly Attr Context = new Attr("context", "所处上下文");

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            // 如果是实体标签则返回空
            if (contextInfo.IsStlEntity)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(contextInfo.InnerHtml))
            {
                return string.Empty;
            }

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Context.Name))
                {
                    contextInfo.ContextType = EContextTypeUtils.GetEnumType(value);
                }
            }

            var innerHtml = RegexUtils.GetInnerContent(ElementName, contextInfo.OuterHtml);

            var builder = new StringBuilder(innerHtml);
            StlParserManager.ParseInnerContent(builder, pageInfo, contextInfo);

            return builder.ToString();
        }
    }
}
