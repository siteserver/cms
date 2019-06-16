using System;
using System.Collections.Specialized;
using System.Text;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.StlElement
{
    [StlElement(Title = "执行动作", Description = "通过 stl:action 标签在模板中创建链接，点击链接后将执行相应的动作")]
    public class StlAction
    {
        private StlAction() { }
        public const string ElementName = "stl:action";

        //繁体/简体转换
        private const string TypeTranslate = "Translate";
        //关闭页面
        private const string TypeClose = "Close";

        [StlAttribute(Title = "动作类型")]
        private const string Type = nameof(Type);

        public static string Parse(ParseContext parseContext)
        {
            var type = string.Empty;

            foreach (var name in parseContext.Attributes.AllKeys)
            {
                var value = parseContext.Attributes[name];
                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
            }

            return ParseImpl(parseContext, type);
        }

        private static string ParseImpl(ParseContext parseContext, string type)
        {
            var attributes = new NameValueCollection();

            foreach (var attributeName in parseContext.Attributes.AllKeys)
            {
                attributes[attributeName] = parseContext.Attributes[attributeName];
            }

            string htmlId = attributes["id"];
            var url = PageUtils.UnClickableUrl;
            var onclick = string.Empty;

            var innerBuilder = new StringBuilder(parseContext.InnerHtml);
            parseContext.ParseInnerContent(innerBuilder);
            var innerHtml = innerBuilder.ToString();

            //计算动作开始
            if (!string.IsNullOrEmpty(type))
            {
                if (StringUtils.EqualsIgnoreCase(type, TypeTranslate))
                {
                    parseContext.PageInfo.AddPageBodyCodeIfNotExists(parseContext.UrlManager, PageInfo.Const.JsAhTranslate);

                    var msgToTraditionalChinese = "繁體";
                    var msgToSimplifiedChinese = "简体";
                    if (!string.IsNullOrEmpty(innerHtml))
                    {
                        if (innerHtml.IndexOf(",", StringComparison.Ordinal) != -1)
                        {
                            msgToTraditionalChinese = innerHtml.Substring(0, innerHtml.IndexOf(",", StringComparison.Ordinal));
                            msgToSimplifiedChinese = innerHtml.Substring(innerHtml.IndexOf(",", StringComparison.Ordinal) + 1);
                        }
                        else
                        {
                            msgToTraditionalChinese = innerHtml;
                        }
                    }
                    innerHtml = msgToTraditionalChinese;

                    if (!string.IsNullOrEmpty(htmlId))
                    {
                        htmlId = "translateLink";
                    }

                    parseContext.FootCodes[TypeTranslate] = $@"
<script type=""text/javascript""> 
var defaultEncoding = 0;
var translateDelay = 0;
var cookieDomain = ""/"";
var msgToTraditionalChinese = ""{msgToTraditionalChinese}"";
var msgToSimplifiedChinese = ""{msgToSimplifiedChinese}"";
var translateButtonId = ""{htmlId}"";
translateInitilization();
</script>
";
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypeClose))
                {
                    url = "javascript:window.close()";
                }
            }
            //计算动作结束

            attributes["href"] = url;
            if (!string.IsNullOrEmpty(htmlId))
            {
                attributes["id"] = htmlId;
            }

            if (!string.IsNullOrEmpty(onclick))
            {
                attributes["onclick"] = onclick;
            }

            // 如果是实体标签，则只返回url
            return parseContext.IsStlEntity
                ? url
                : $@"<a {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</a>";
        }
    }
}
