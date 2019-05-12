using System;
using System.Collections.Generic;
using System.Text;
using SiteServer.Utils;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
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

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var type = string.Empty;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];
                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
            }

            return ParseImpl(pageInfo, contextInfo, type);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string type)
        {
            var attributes = new Dictionary<string, string>();

            foreach (var attributeName in contextInfo.Attributes.AllKeys)
            {
                attributes[attributeName] = contextInfo.Attributes[attributeName];
            }

            string htmlId;
            attributes.TryGetValue("id", out htmlId);
            var url = PageUtils.UnclickedUrl;
            var onclick = string.Empty;

            var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
            var innerHtml = innerBuilder.ToString();

            //计算动作开始
            if (!string.IsNullOrEmpty(type))
            {
                if (StringUtils.EqualsIgnoreCase(type, TypeTranslate))
                {
                    pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.JsAhTranslate);

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

                    pageInfo.FootCodes[TypeTranslate] = $@"
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
            return contextInfo.IsStlEntity
                ? url
                : $@"<a {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</a>";
        }
    }
}
