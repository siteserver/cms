using System;
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
	public class StlLocation
	{
		private StlLocation(){}
		public const string ElementName = "stl:location";//显示位置

		public const string Attribute_Separator = "separator";				//当前位置分隔符
		public const string Attribute_Target = "target";					//打开窗口的目标
		public const string Attribute_LinkClass = "linkclass";				//链接CSS样式
        public const string Attribute_WordNum = "wordnum";                  //链接字数
        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

		public static ListDictionary AttributeList
		{
			get
			{
				var attributes = new ListDictionary();
				attributes.Add(Attribute_Separator, "当前位置分隔符");
				attributes.Add(Attribute_Target, "打开窗口的目标");
				attributes.Add(Attribute_LinkClass, "链接CSS样式");
                attributes.Add(Attribute_WordNum, "链接字数");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
				return attributes;
			}
		}


		//对“当前位置”（stl:location）元素进行解析
        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			var parsedContent = string.Empty;
			try
			{
				var ie = node.Attributes.GetEnumerator();
				var separator = " - ";
				var target = string.Empty;
				var linkClass = string.Empty;
                var wordNum = 0;
                var isDynamic = false;

				var attributes = new StringDictionary();

				while (ie.MoveNext())
				{
					var attr = (XmlAttribute)ie.Current;
					var attributeName = attr.Name.ToLower();
					if (attributeName.Equals(Attribute_Separator))
					{
						separator = attr.Value;
					}
					else if (attributeName.Equals(Attribute_Target))
					{
						target = attr.Value;
					}
					else if (attributeName.Equals(Attribute_LinkClass))
					{
						linkClass = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_WordNum))
                    {
                        wordNum = TranslateUtils.ToInt(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value);
                    }
					else
					{
						attributes.Add(attributeName, attr.Value);
					}
				}

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(node, pageInfo, contextInfo, separator, target, linkClass, wordNum, attributes);
                }
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

            var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelID);

            var builder = new StringBuilder();

            var parentsPath = nodeInfo.ParentsPath;
            var parentsCount = nodeInfo.ParentsCount;
            if (parentsPath.Length != 0)
            {
                var nodePath = parentsPath + "," + contextInfo.ChannelID;
                var nodeIDArrayList = TranslateUtils.StringCollectionToStringList(nodePath);
                foreach (string nodeIDStr in nodeIDArrayList)
                {
                    var currentID = int.Parse(nodeIDStr);
                    var currentNodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, currentID);
                    if (currentID == pageInfo.PublishmentSystemId)
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
                    else if (currentID == contextInfo.ChannelID)
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
