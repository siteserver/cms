using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlLayout
    {
        private StlLayout() { }
        public const string ElementName = "stl:layout";                     //布局

        public const string Attribute_Cols = "cols";                        //各列宽度
        public const string Attribute_Margin_Top = "margintop";             //上边距
        public const string Attribute_Margin_Bottom = "marginbottom";       //下边距
        public const string Attribute_Context = "context";                  //所处上下文

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary();
                attributes.Add(Attribute_Cols, "各列宽度");
                attributes.Add(Attribute_Margin_Top, "上边距");
                attributes.Add(Attribute_Margin_Bottom, "下边距");
                attributes.Add(Attribute_Context, "所处上下文");
                return attributes;
            }
        }

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfoRef)
        {
            var parsedContent = string.Empty;
            var cols = string.Empty;
            var marginTop = 5;
            var marginBottom = 5;
            var contextInfo = contextInfoRef.Clone();

            try
            {
                var ie = node.Attributes.GetEnumerator();

                while (ie.MoveNext())
                {
                    var attr = (XmlAttribute)ie.Current;
                    var attributeName = attr.Name.ToLower();
                    if (attributeName.Equals(Attribute_Cols))
                    {
                        cols = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_Margin_Top))
                    {
                        marginTop = TranslateUtils.ToInt(attr.Value, 5);
                    }
                    else if (attributeName.Equals(Attribute_Margin_Bottom))
                    {
                        marginBottom = TranslateUtils.ToInt(attr.Value, 5);
                    }
                    else if (attributeName.Equals(Attribute_Context))
                    {
                        contextInfo.ContextType = EContextTypeUtils.GetEnumType(attr.Value);
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
