using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using HtmlAgilityPack;
using SiteServer.Utils;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "页签切换", Description = "通过 stl:tabs 标签在模板中显示页签切换")]
    public class StlTabs
    {
        private StlTabs() { }
        public const string ElementName = "stl:tabs";

        [StlAttribute(Title = "页签名称")]
        private const string TabName = nameof(TabName);

        [StlAttribute(Title = "页签类型（head,body）")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "页签切换方式")]
        private const string Action = nameof(Action);

        [StlAttribute(Title = "当前显示页签头部的CSS类")]
        private const string ClassActive = nameof(ClassActive);

        [StlAttribute(Title = "当前隐藏页签头部的CSS类")]
        private const string ClassNormal = nameof(ClassNormal);

        [StlAttribute(Title = "当前页签")]
        private const string Current = nameof(Current);

        public const string TypeHead = "Head";
        public const string TypeBody = "Body";

        public const string ActionClick = "Click";
        public const string ActionMouseOver = "MouseOver";

        public static SortedList<string, string> ActionList => new SortedList<string, string>
        {
            {ActionClick, "点击"},
            {ActionMouseOver, "鼠标移动"}
        };

        internal static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var tabName = string.Empty;
            var type = string.Empty;
            var action = ActionMouseOver;
            var classActive = "active";
            var classNormal = string.Empty;
            var current = 0;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, TabName))
                {
                    tabName = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Action))
                {
                    action = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ClassActive))
                {
                    classActive = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ClassNormal))
                {
                    classNormal = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Current))
                {
                    current = TranslateUtils.ToInt(value, 1);
                }
            }

            return ParseImpl(pageInfo, contextInfo, tabName, type, action, classActive, classNormal, current);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string tabName, string type, string action, string classActive, string classNormal, int current)
        {
            pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.Jquery);

            var builder = new StringBuilder();
            var uniqueId = pageInfo.UniqueId;
            var isHeader = StringUtils.EqualsIgnoreCase(type, TypeHead);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(contextInfo.InnerHtml);

            var htmlNodes = htmlDoc.DocumentNode.ChildNodes;
            if (htmlNodes != null && htmlNodes.Count > 0)
            {
                if (isHeader)
                {
                    builder.Append($@"
<script language=javascript>
function stl_tab_{uniqueId}(tabName, no){{
	for ( i = 1; i <= {htmlNodes.Count}; i++){{
		var el = jQuery('#{tabName}_tabContent_' + i);
		var li = $('#{tabName}_tabHeader_' + i);
		if (i == no){{
            try{{
			    el.show();
            }}catch(e){{}}
            li.removeClass('{classNormal}');
            li.addClass('{classActive}');
		}}else{{
            try{{
			    el.hide();
            }}catch(e){{}}
            li.removeClass('{classActive}');
            li.addClass('{classNormal}');
		}}
	}}
}}
</script>
");
                }

                var count = 0;
                foreach (var htmlNode in htmlNodes)
                {
                    if (htmlNode.NodeType != HtmlNodeType.Element) continue;

                    var attributes = new NameValueCollection();
                    if (htmlNode.Attributes != null)
                    {
                        foreach (var attr in htmlNode.Attributes)
                        {
                            if (attr == null) continue;

                            var attributeName = attr.Name.ToLower();
                            if (!StringUtils.EqualsIgnoreCase(attr.Name, "id") && !StringUtils.EqualsIgnoreCase(attr.Name, "onmouseover") && !StringUtils.EqualsIgnoreCase(attr.Name, "onclick"))
                            {
                                attributes[attributeName] = attr.Value;
                            }
                        }
                    }

                    count++;
                    if (isHeader)
                    {
                        attributes["id"] = $"{tabName}_tabHeader_{count}";
                        if (StringUtils.EqualsIgnoreCase(action, ActionMouseOver))
                        {
                            attributes["onmouseover"] = $"stl_tab_{uniqueId}('{tabName}', {count});return false;";
                        }
                        else
                        {
                            attributes["onclick"] = $"stl_tab_{uniqueId}('{tabName}', {count});return false;";
                        }
                        if (current != 0)
                        {
                            if (count == current)
                            {
                                attributes["class"] = classActive;
                            }
                            else
                            {
                                attributes["class"] = classNormal;
                            }
                        }
                    }
                    else
                    {
                        attributes["id"] = $"{tabName}_tabContent_{count}";
                        if (current != 0)
                        {
                            if (count != current)
                            {
                                attributes["style"] = $"display:none;{attributes["style"]}";
                            }
                        }
                    }

                    var innerHtml = string.Empty;
                    if (!string.IsNullOrEmpty(htmlNode.InnerHtml))
                    {
                        var innerBuilder = new StringBuilder(htmlNode.InnerHtml);
                        StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                        innerHtml = innerBuilder.ToString();
                    }

                    builder.Append(
                        $"<{htmlNode.Name} {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</{htmlNode.Name}>");
                }
            }

            return builder.ToString();
        }
    }
}