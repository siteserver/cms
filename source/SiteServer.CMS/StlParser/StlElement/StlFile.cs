using System;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
	public class StlFile
	{
		private StlFile(){}
        public const string ElementName = "stl:file";       //文件下载链接

        public const string Attribute_NO = "no";                            //显示字段的顺序
		public const string Attribute_Src = "src";		                        //需要下载的文件地址
        public const string Attribute_IsFilesize = "isfilesize";                //显示文件大小
        public const string Attribute_IsCount = "iscount";                      //是否记录文件下载次数
        public const string Attribute_Type = "type";							//指定存储附件的字段

        public const string Attribute_LeftText = "lefttext";                //显示在信息前的文字
        public const string Attribute_RightText = "righttext";              //显示在信息后的文字
        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

		public static ListDictionary AttributeList
		{
			get
			{
				var attributes = new ListDictionary();
                attributes.Add(Attribute_NO, "显示字段的顺序");
                attributes.Add(Attribute_Src, "需要下载的文件地址");
                attributes.Add(Attribute_IsFilesize, "显示文件大小");
                attributes.Add(Attribute_IsCount, "是否记录文件下载次数");
                attributes.Add(Attribute_Type, "指定存储附件的字段");

                attributes.Add(Attribute_LeftText, "显示在信息前的文字");
                attributes.Add(Attribute_RightText, "显示在信息后的文字");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
				return attributes;
			}
		}

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;
            try
            {
                var ie = node.Attributes.GetEnumerator();

                var no = 0;
                var src = string.Empty;
                var isFilesize = false;
                var isCount = true;
                var type = BackgroundContentAttribute.FileUrl;
                var leftText = string.Empty;
                var rightText = string.Empty;
                var isDynamic = false;

                var attributes = new StringDictionary();

                while (ie.MoveNext())
                {
                    var attr = (XmlAttribute)ie.Current;
                    var attributeName = attr.Name.ToLower();
                    if (attributeName.Equals(Attribute_NO))
                    {
                        no = TranslateUtils.ToInt(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_Src))
                    {
                        src = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_IsFilesize))
                    {
                        isFilesize = TranslateUtils.ToBool(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_IsCount))
                    {
                        isCount = TranslateUtils.ToBool(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_Type))
                    {
                        type = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_LeftText))
                    {
                        leftText = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_RightText))
                    {
                        rightText = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value);
                    }
                    else
                    {
                        attributes.Remove(attributeName);
                        attributes.Add(attributeName, attr.Value);
                    }
                }

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(node, pageInfo, contextInfo, type, no, src, isFilesize, isCount, leftText, rightText, attributes);
                }
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, string type, int no, string src, bool isFilesize, bool isCount, string leftText, string rightText, StringDictionary attributes)
        {
            var parsedContent = string.Empty;

            if (!string.IsNullOrEmpty(node.InnerXml))
            {
                var innerBuilder = new StringBuilder(node.InnerXml);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                node.InnerXml = innerBuilder.ToString();
            }

            var fileUrl = string.Empty;
            if (!string.IsNullOrEmpty(src))
            {
                fileUrl = src;
            }
            else
            {
                if (contextInfo.ContextType == EContextType.Undefined)
                    contextInfo.ContextType = EContextType.Content;
                if (contextInfo.ContextType == EContextType.Content)
                {
                    if (contextInfo.ContentID != 0)
                    {
                        var contentInfo = contextInfo.ContentInfo;

                        if (contentInfo != null && !string.IsNullOrEmpty(contentInfo.GetExtendedAttribute(type)))
                        {
                            if (no <= 1)
                            {
                                if (StringUtils.EqualsIgnoreCase(type, BackgroundContentAttribute.FileUrl))
                                {
                                    fileUrl = contentInfo.GetExtendedAttribute(BackgroundContentAttribute.FileUrl);
                                }
                                else
                                {
                                    fileUrl = contentInfo.GetExtendedAttribute(type);
                                }
                            }
                            else
                            {
                                var extendAttributeName = ContentAttribute.GetExtendAttributeName(type);
                                var extendValues = contentInfo.GetExtendedAttribute(extendAttributeName);
                                if (!string.IsNullOrEmpty(extendValues))
                                {
                                    var index = 2;
                                    foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                                    {
                                        if (index == no)
                                        {
                                            fileUrl = extendValue;

                                            break;
                                        }
                                        index++;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (contextInfo.ContextType == EContextType.Each)
                {
                    fileUrl = contextInfo.ItemContainer.EachItem.DataItem as string;
                }
            }

            parsedContent = InputParserUtility.GetFileHtmlWithoutCount(pageInfo.PublishmentSystemInfo, fileUrl, attributes, node.InnerXml, false);

            if (isFilesize)
            {
                var filePath = PathUtility.MapPath(pageInfo.PublishmentSystemInfo, fileUrl);
                parsedContent += " (" + FileUtils.GetFileSizeByFilePath(filePath) + ")";
            }
            else
            {
                if (isCount && contextInfo.ContentInfo != null)
                {
                    parsedContent = InputParserUtility.GetFileHtmlWithCount(pageInfo.PublishmentSystemInfo, contextInfo.ContentInfo.NodeId, contextInfo.ContentInfo.Id, fileUrl, attributes, node.InnerXml, false);
                }
                else
                {
                    parsedContent = InputParserUtility.GetFileHtmlWithoutCount(pageInfo.PublishmentSystemInfo, fileUrl, attributes, node.InnerXml, false);
                }                
            }

            if (!string.IsNullOrEmpty(parsedContent))
            {
                parsedContent = leftText + parsedContent + rightText;
            }

            return parsedContent;
        }
	}
}
