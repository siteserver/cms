using System;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlTabs
    {
        private StlTabs() { }
        public const string ElementName = "stl:tabs";           //页签切换

        public const string Attribute_TabName = "tabname";			            //页签名称
        public const string Attribute_IsHeader = "isheader";			        //是否为页签头部
        public const string Attribute_Action = "action";			            //页签切换方式
        public const string Attribute_ClassActive = "classactive";				//当前显示页签头部的CSS类
        public const string Attribute_ClassNormal = "classnormal";              //当前隐藏页签头部的CSS类
        public const string Attribute_Current = "current";                      //当前页签
        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

        public const string Action_Click = "Click";			                    //点击
        public const string Action_MouseOver = "MouseOver";			            //鼠标移动

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary();

                attributes.Add(Attribute_TabName, "页签名称");
                attributes.Add(Attribute_IsHeader, "是否为页签头部");
                attributes.Add(Attribute_Action, "页签切换方式");
                attributes.Add(Attribute_ClassActive, "当前显示页签头部的CSS类");
                attributes.Add(Attribute_ClassNormal, "当前隐藏页签头部的CSS类");
                attributes.Add(Attribute_Current, "当前页签");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");

                return attributes;
            }
        }

        internal static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;
            try
            {
                var ie = node.Attributes.GetEnumerator();

                var tabName = string.Empty;
                var isHeader = true;
                var action = Action_MouseOver;
                var classActive = "active";
                var classNormal = string.Empty;
                var current = 0;
                var isDynamic = false;

                while (ie.MoveNext())
                {
                    var attr = (XmlAttribute)ie.Current;
                    var attributeName = attr.Name.ToLower();
                    if (attributeName.Equals(Attribute_TabName))
                    {
                        tabName = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_IsHeader))
                    {
                        isHeader = TranslateUtils.ToBool(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_Action))
                    {
                        action = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_ClassActive))
                    {
                        classActive = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_ClassNormal))
                    {
                        classNormal = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_Current))
                    {
                        current = TranslateUtils.ToInt(attr.Value, 1);
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value);
                    }
                }

                pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(node, pageInfo, contextInfo, tabName, isHeader, action, classActive, classNormal, current);
                }
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, string tabName, bool isHeader, string action, string classActive, string classNormal, int current)
        {
            var builder = new StringBuilder();
            var uniqueID = pageInfo.UniqueId;

            if (node.ChildNodes != null && node.ChildNodes.Count > 0)
            {
                if (isHeader)
                {
                    builder.Append($@"
<SCRIPT language=javascript>
function stl_tab_{uniqueID}(tabName, no){{
	for ( i = 1; i <= {node.ChildNodes.Count}; i++){{
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
</SCRIPT>
");
                }

                var count = 0;
                foreach (XmlNode xmlNode in node.ChildNodes)
                {
                    if (xmlNode.NodeType != XmlNodeType.Element) continue;
                    var attributes = new NameValueCollection();
                    var xmlIE = xmlNode.Attributes.GetEnumerator();
                    while (xmlIE.MoveNext())
                    {
                        var attr = (XmlAttribute)xmlIE.Current;
                        var attributeName = attr.Name.ToLower();
                        if (!attributeName.Equals("id") && !attributeName.Equals("onmouseover") && !attributeName.Equals("onclick"))
                        {
                            attributes[attributeName] = attr.Value;
                        }
                    }

                    count++;
                    if (isHeader)
                    {
                        attributes["id"] = $"{tabName}_tabHeader_{count}";
                        if (StringUtils.EqualsIgnoreCase(action, Action_MouseOver))
                        {
                            attributes["onmouseover"] = $"stl_tab_{uniqueID}('{tabName}', {count});return false;";
                        }
                        else
                        {
                            attributes["onclick"] = $"stl_tab_{uniqueID}('{tabName}', {count});return false;";
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

                    var innerXml = string.Empty;
                    if (!string.IsNullOrEmpty(xmlNode.InnerXml))
                    {
                        var innerBuilder = new StringBuilder(xmlNode.InnerXml);
                        StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                        StlParserUtility.XmlToHtml(innerBuilder);
                        innerXml = innerBuilder.ToString();
                    }

                    builder.Append(
                        $"<{xmlNode.Name} {TranslateUtils.ToAttributesString(attributes)}>{innerXml}</{xmlNode.Name}>");
                }
            }

            return builder.ToString();
        }
    }
}
