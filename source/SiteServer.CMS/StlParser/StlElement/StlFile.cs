using System;
using System.Collections.Generic;
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
    [Stl(Usage = "文件下载链接", Description = "通过 stl:file 标签在模板中显示文件下载链接")]
    public class StlFile
	{
	    private StlFile() { }

	    public const string ElementName = "stl:file";

        public const string AttributeNo = "no";
		public const string AttributeSrc = "src";
        public const string AttributeIsFilesize = "isFileSize";
        public const string AttributeIsCount = "isCount";
        public const string AttributeType = "type";
        public const string AttributeLeftText = "leftText";
        public const string AttributeRightText = "rightText";
        public const string AttributeIsDynamic = "isDynamic";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
	        {AttributeNo, "显示字段的顺序"},
	        {AttributeSrc, "需要下载的文件地址"},
	        {AttributeIsFilesize, "显示文件大小"},
	        {AttributeIsCount, "是否记录文件下载次数"},
	        {AttributeType, "指定存储附件的字段"},
	        {AttributeLeftText, "显示在信息前的文字"},
	        {AttributeRightText, "显示在信息后的文字"},
	        {AttributeIsDynamic, "是否动态显示"}
	    };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent;
            try
            {
                var no = 0;
                var src = string.Empty;
                var isFilesize = false;
                var isCount = true;
                var type = BackgroundContentAttribute.FileUrl;
                var leftText = string.Empty;
                var rightText = string.Empty;
                var isDynamic = false;

                var attributes = new StringDictionary();

                var ie = node.Attributes?.GetEnumerator();
                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeNo))
                        {
                            no = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeSrc))
                        {
                            src = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsFilesize))
                        {
                            isFilesize = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsCount))
                        {
                            isCount = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeType))
                        {
                            type = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeLeftText))
                        {
                            leftText = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeRightText))
                        {
                            rightText = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                        else
                        {
                            attributes[attr.Name] = attr.Value;
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(node, pageInfo, contextInfo, type, no, src, isFilesize, isCount, leftText, rightText, attributes);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, string type, int no, string src, bool isFilesize, bool isCount, string leftText, string rightText, StringDictionary attributes)
        {
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
                {
                    contextInfo.ContextType = EContextType.Content;
                }
                if (contextInfo.ContextType == EContextType.Content)
                {
                    if (contextInfo.ContentId != 0)
                    {
                        var contentInfo = contextInfo.ContentInfo;

                        if (!string.IsNullOrEmpty(contentInfo?.GetExtendedAttribute(type)))
                        {
                            if (no <= 1)
                            {
                                fileUrl = contentInfo.GetExtendedAttribute(StringUtils.EqualsIgnoreCase(type, BackgroundContentAttribute.FileUrl) ? BackgroundContentAttribute.FileUrl : type);
                            }
                            else
                            {
                                var extendAttributeName = ContentAttribute.GetExtendAttributeName(type);
                                var extendValues = contentInfo.GetExtendedAttribute(extendAttributeName);
                                if (!string.IsNullOrEmpty(extendValues))
                                {
                                    var index = 2;
                                    foreach (var extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
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

            var parsedContent = InputParserUtility.GetFileHtmlWithoutCount(pageInfo.PublishmentSystemInfo, fileUrl, attributes, node.InnerXml, false);

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
