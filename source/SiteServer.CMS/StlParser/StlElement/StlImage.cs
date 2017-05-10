using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "显示图片", Description = "通过 stl:image 标签在模板中显示栏目或内容的图片")]
    public class StlImage
	{
		private StlImage(){}
		public const string ElementName = "stl:image";

		public const string AttributeChannelIndex = "channelIndex";
		public const string AttributeChannelName = "channelName";
        public const string AttributeNo = "no";
		public const string AttributeParent = "parent";
		public const string AttributeUpLevel = "upLevel";
        public const string AttributeTopLevel = "topLevel";
        public const string AttributeType = "type";
        public const string AttributeIsOriginal = "isOriginal";
		public const string AttributeSrc = "src";
        public const string AttributeAltSrc = "altSrc";
        public const string AttributeWidth = "width";
        public const string AttributeHeight = "height";
        public const string AttributeIsDynamic = "isDynamic";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
	        {AttributeChannelIndex, "栏目索引"},
	        {AttributeChannelName, "栏目名称"},
	        {AttributeNo, "显示字段的顺序"},
	        {AttributeParent, "显示父栏目"},
	        {AttributeUpLevel, "上级栏目的级别"},
	        {AttributeTopLevel, "从首页向下的栏目级别"},
	        {AttributeType, "指定存储图片的字段"},
	        {AttributeIsOriginal, "如果是引用内容，是否获取所引用内容的值"},
	        {AttributeSrc, "显示的图片地址"},
	        {AttributeAltSrc, "当指定的图片不存在时显示的图片地址"},
	        {AttributeWidth, "宽度"},
	        {AttributeHeight, "高度"},
	        {AttributeIsDynamic, "是否动态显示"}
	    };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			string parsedContent;
			try
			{
				var isGetPicUrlFromAttribute = false;
				var channelIndex = string.Empty;
				var channelName = string.Empty;
                var no = 0;
				var upLevel = 0;
                var topLevel = -1;
                var type = BackgroundContentAttribute.ImageUrl;
                var isOriginal = false;
                var src = string.Empty;
				var altSrc = string.Empty;
                var isDynamic = false;
                var stlImage = new HtmlImage();

                var ie = node.Attributes?.GetEnumerator();
			    if (ie != null)
			    {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChannelIndex))
                        {
                            channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                            if (!string.IsNullOrEmpty(channelIndex))
                            {
                                isGetPicUrlFromAttribute = true;
                            }
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChannelName))
                        {
                            channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                            if (!string.IsNullOrEmpty(channelName))
                            {
                                isGetPicUrlFromAttribute = true;
                            }
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeNo))
                        {
                            no = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeParent))
                        {
                            if (TranslateUtils.ToBool(attr.Value))
                            {
                                upLevel = 1;
                                isGetPicUrlFromAttribute = true;
                            }
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeUpLevel))
                        {
                            upLevel = TranslateUtils.ToInt(attr.Value);
                            if (upLevel > 0)
                            {
                                isGetPicUrlFromAttribute = true;
                            }
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTopLevel))
                        {
                            topLevel = TranslateUtils.ToInt(attr.Value);
                            if (topLevel >= 0)
                            {
                                isGetPicUrlFromAttribute = true;
                            }
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeType))
                        {
                            type = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsOriginal))
                        {
                            isOriginal = TranslateUtils.ToBool(attr.Value, true);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeSrc))
                        {
                            src = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeAltSrc))
                        {
                            altSrc = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                        else
                        {
                            stlImage.Attributes[attr.Name]  = attr.Value;
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(stlElement, node, pageInfo, contextInfo, stlImage, isGetPicUrlFromAttribute, channelIndex, channelName, no, upLevel, topLevel, type, isOriginal, src, altSrc);
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, HtmlImage stlImage, bool isGetPicUrlFromAttribute, string channelIndex, string channelName, int no, int upLevel, int topLevel, string type, bool isOriginal, string src, string altSrc)
        {
            var parsedContent = string.Empty;

            var contentId = 0;
            //判断是否图片地址由标签属性获得
            if (!isGetPicUrlFromAttribute)
            {
                contentId = contextInfo.ContentId;
            }
            var contextType = contextInfo.ContextType;

            var picUrl = string.Empty;
            if (!string.IsNullOrEmpty(src))
            {
                picUrl = src;
            }
            else
            {
                if (contextType == EContextType.Undefined)
                {
                    contextType = contentId != 0 ? EContextType.Content : EContextType.Channel;
                }

                if (contextType == EContextType.Content)//获取内容图片
                {
                    var contentInfo = contextInfo.ContentInfo;

                    if (isOriginal)
                    {
                        if (contentInfo != null && contentInfo.ReferenceId > 0 && contentInfo.SourceId > 0)
                        {
                            var targetNodeId = contentInfo.SourceId;
                            var targetPublishmentSystemId = DataProvider.NodeDao.GetPublishmentSystemId(targetNodeId);
                            var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
                            var targetNodeInfo = NodeManager.GetNodeInfo(targetPublishmentSystemId, targetNodeId);

                            var tableStyle = NodeManager.GetTableStyle(targetPublishmentSystemInfo, targetNodeInfo);
                            var tableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeInfo);
                            var targetContentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentInfo.ReferenceId);
                            if (targetContentInfo != null && targetContentInfo.NodeId > 0)
                            {
                                contentInfo = targetContentInfo;
                            }
                        }
                    }

                    if (contentInfo == null)
                    {
                        contentInfo = DataProvider.ContentDao.GetContentInfo(ETableStyle.BackgroundContent, pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentId);
                    }

                    if (contentInfo != null)
                    {
                        if (no <= 1)
                        {
                            picUrl = contentInfo.GetExtendedAttribute(type);
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
                                        picUrl = extendValue;
                                        break;
                                    }
                                    index++;
                                }
                            }
                        }
                    }
                }
                else if (contextType == EContextType.Channel)//获取栏目图片
                {
                    var channelId = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelId, upLevel, topLevel);

                    channelId = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelId, channelIndex, channelName);

                    var channel = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelId);

                    picUrl = channel.ImageUrl;
                }
                else if (contextType == EContextType.Each)
                {
                    picUrl = contextInfo.ItemContainer.EachItem.DataItem as string;
                }
            }

            if (string.IsNullOrEmpty(picUrl))
            {
                picUrl = altSrc;
            }

            if (!string.IsNullOrEmpty(picUrl))
            {
                var extension = PathUtils.GetExtension(picUrl);
                if (EFileSystemTypeUtils.IsFlash(extension))
                {
                    parsedContent = StlFlash.Parse(stlElement, node, pageInfo, contextInfo);
                }
                else if (EFileSystemTypeUtils.IsPlayer(extension))
                {
                    parsedContent = StlPlayer.Parse(stlElement, node, pageInfo, contextInfo);
                }
                else
                {
                    stlImage.Src = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, picUrl);
                    parsedContent = ControlUtils.GetControlRenderHtml(stlImage);
                }
            }

            return parsedContent;
        }
	}
}
