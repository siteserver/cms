using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "页面布局", Description = "通过 stl:layout 标签在模板中创建页面布局")]
    public class StlLayout
    {
        private StlLayout() { }
        public const string ElementName = "stl:layout";

        public const string AttributeCols = "cols";
        public const string AttributeMarginTop = "marginTop";
        public const string AttributeMarginBottom = "marginBottom";
        public const string AttributeContext = "context";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeCols, "各列宽度"},
            {AttributeMarginTop, "上边距"},
            {AttributeMarginBottom, "下边距"},
            {AttributeContext, "所处上下文"}
        };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfoRef)
        {
            var parsedContent = string.Empty;
            var contextInfo = contextInfoRef.Clone();

            try
            {
                var cols = string.Empty;
                var marginTop = 5;
                var marginBottom = 5;

                var ie = node.Attributes?.GetEnumerator();
                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeCols))
                        {
                            cols = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeMarginTop))
                        {
                            marginTop = TranslateUtils.ToInt(attr.Value, 5);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeMarginBottom))
                        {
                            marginBottom = TranslateUtils.ToInt(attr.Value, 5);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeContext))
                        {
                            contextInfo.ContextType = EContextTypeUtils.GetEnumType(attr.Value);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(node.InnerXml))
                {
                    var innerHtml = RegexUtils.GetInnerContent(ElementName, stlElement);

                    var containerList = RegexUtils.GetTagContents(StlContainer.ElementName, innerHtml);

                    var widthList = new List<string>();
                    for (var i = 0; i < containerList.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(cols) && cols.Split(',').Length == i + 1)
                        {
                            var width = cols.Split(',')[i];
                            if (width == "*")
                            {
                                width = string.Empty;
                            }
                            widthList.Add(width);
                        }
                        else
                        {
                            widthList.Add(string.Empty);
                        }
                    }

                    var builder = new StringBuilder();

                    var style = string.Empty;
                    if (marginTop > 0)
                    {
                        style += $"margin-top:{marginTop}px;";
                    }
                    if (marginBottom > 0)
                    {
                        style += $"margin-bottom:{marginTop}px;";
                    }
                    if (!string.IsNullOrEmpty(style))
                    {
                        style = $@"style=""{style}""";
                    }
                    builder.Append($@"<table width=""100%"" cellspacing=""0"" cellpadding=""0"" {style}><tr>");

                    for (var i = 0; i < widthList.Count; i++)
                    {
                        var container = containerList[i];
                        var width = widthList[i];
                        if (!string.IsNullOrEmpty(width))
                        {
                            width = $@" width=""{width}""";
                        }
                        builder.Append($@"<td{width} valign=""top"">{container}</td>");
                    }

                    builder.Append(@"</tr></table>");

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
