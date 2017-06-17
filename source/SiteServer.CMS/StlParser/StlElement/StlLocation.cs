using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "当前位置", Description = "通过 stl:location 标签在模板中插入页面的当前位置")]
    public class StlLocation
	{
		private StlLocation(){}
		public const string ElementName = "stl:location";

		public const string AttributeSeparator = "separator";
		public const string AttributeTarget = "target";
		public const string AttributeLinkClass = "linkClass";
        public const string AttributeWordNum = "wordNum";
        public const string AttributeIsDynamic = "isDynamic";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
	    {
	        {AttributeSeparator, "当前位置分隔符"},
	        {AttributeTarget, "打开窗口的目标"},
	        {AttributeLinkClass, "链接CSS样式"},
	        {AttributeWordNum, "链接字数"},
	        {AttributeIsDynamic, "是否动态显示"}
	    };


        //对“当前位置”（stl:location）元素进行解析
        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			string parsedContent;
			try
			{
				var separator = " - ";
				var target = string.Empty;
				var linkClass = string.Empty;
                var wordNum = 0;
                var isDynamic = false;
				var attributes = new StringDictionary();

                var ie = node.Attributes?.GetEnumerator();
			    if (ie != null)
			    {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeSeparator))
                        {
                            separator = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTarget))
                        {
                            target = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeLinkClass))
                        {
                            linkClass = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeWordNum))
                        {
                            wordNum = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                        else
                        {
                            attributes.Add(attr.Name, attr.Value);
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(node, pageInfo, contextInfo, separator, target, linkClass, wordNum, attributes);
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }
			
			return parsedContent;
		}

        private static string ParseImpl(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, string separator, string target, string linkClass, int wordNum, StringDictionary attributes)
        {
            if (!string.IsNullOrEmpty(node.InnerXml))
            {
                separator = node.InnerXml;
            }

            var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId);

            var builder = new StringBuilder();

            var parentsPath = nodeInfo.ParentsPath;
            var parentsCount = nodeInfo.ParentsCount;
            if (parentsPath.Length != 0)
            {
                var nodePath = parentsPath + "," + contextInfo.ChannelId;
                var nodeIdArrayList = TranslateUtils.StringCollectionToStringList(nodePath);
                foreach (var nodeIdStr in nodeIdArrayList)
                {
                    var currentId = int.Parse(nodeIdStr);
                    var currentNodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, currentId);
                    if (currentId == pageInfo.PublishmentSystemId)
                    {
                        var stlAnchor = new HtmlAnchor();
                        if (!string.IsNullOrEmpty(target))
                        {
                            stlAnchor.Target = target;
                        }
                        if (!string.IsNullOrEmpty(linkClass))
                        {
                            stlAnchor.Attributes.Add("class", linkClass);
                        }
                        var url = PageUtility.GetIndexPageUrl(pageInfo.PublishmentSystemInfo);
                        if (url.Equals(PageUtils.UnclickedUrl))
                        {
                            stlAnchor.Target = string.Empty;
                        }
                        stlAnchor.HRef = url;
                        stlAnchor.InnerHtml = StringUtils.MaxLengthText(currentNodeInfo.NodeName, wordNum);

                        ControlUtils.AddAttributesIfNotExists(stlAnchor, attributes);

                        builder.Append(ControlUtils.GetControlRenderHtml(stlAnchor));

                        if (parentsCount > 0)
                        {
                            builder.Append(separator);
                        }
                    }
                    else if (currentId == contextInfo.ChannelId)
                    {
                        var stlAnchor = new HtmlAnchor();
                        if (!string.IsNullOrEmpty(target))
                        {
                            stlAnchor.Target = target;
                        }
                        if (!string.IsNullOrEmpty(linkClass))
                        {
                            stlAnchor.Attributes.Add("class", linkClass);
                        }
                        var url = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, currentNodeInfo);
                        if (url.Equals(PageUtils.UnclickedUrl))
                        {
                            stlAnchor.Target = string.Empty;
                        }
                        stlAnchor.HRef = url;
                        stlAnchor.InnerHtml = StringUtils.MaxLengthText(currentNodeInfo.NodeName, wordNum);

                        ControlUtils.AddAttributesIfNotExists(stlAnchor, attributes);

                        builder.Append(ControlUtils.GetControlRenderHtml(stlAnchor));
                    }
                    else
                    {
                        var stlAnchor = new HtmlAnchor();
                        if (!string.IsNullOrEmpty(target))
                        {
                            stlAnchor.Target = target;
                        }
                        if (!string.IsNullOrEmpty(linkClass))
                        {
                            stlAnchor.Attributes.Add("class", linkClass);
                        }
                        var url = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, currentNodeInfo);
                        if (url.Equals(PageUtils.UnclickedUrl))
                        {
                            stlAnchor.Target = string.Empty;
                        }
                        stlAnchor.HRef = url;
                        stlAnchor.InnerHtml = StringUtils.MaxLengthText(currentNodeInfo.NodeName, wordNum);

                        ControlUtils.AddAttributesIfNotExists(stlAnchor, attributes);

                        builder.Append(ControlUtils.GetControlRenderHtml(stlAnchor));

                        if (parentsCount > 0)
                        {
                            builder.Append(separator);
                        }
                    }
                }
            }

            return builder.ToString();
        }
	}
}
