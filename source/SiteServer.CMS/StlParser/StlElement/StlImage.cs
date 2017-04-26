using System;
using System.Collections.Specialized;
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
	public class StlImage
	{
		private StlImage(){}
		public const string ElementName = "stl:image";//显示图片

		public const string Attribute_ChannelIndex = "channelindex";			//栏目索引
		public const string Attribute_ChannelName = "channelname";				//栏目名称
        public const string Attribute_NO = "no";                            //显示字段的顺序
		public const string Attribute_Parent = "parent";						//显示父栏目
		public const string Attribute_UpLevel = "uplevel";						//上级栏目的级别
        public const string Attribute_TopLevel = "toplevel";					//从首页向下的栏目级别
        public const string Attribute_Type = "type";							//指定存储图片的字段
        public const string Attribute_IsOriginal = "isoriginal";            //如果是引用内容，是否获取所引用内容的值
		public const string Attribute_Src = "src";								//显示的图片地址
        public const string Attribute_AltSrc = "altsrc";						//当指定的图片不存在时显示的图片地址
        public const string Attribute_Width = "width";							//宽度
        public const string Attribute_Height = "height";						//高度
        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

		public static ListDictionary AttributeList
		{
			get
			{
				var attributes = new ListDictionary();
				attributes.Add(Attribute_ChannelIndex, "栏目索引");
				attributes.Add(Attribute_ChannelName, "栏目名称");
                attributes.Add(Attribute_NO, "显示字段的顺序");
				attributes.Add(Attribute_Parent, "显示父栏目");
				attributes.Add(Attribute_UpLevel, "上级栏目的级别");
                attributes.Add(Attribute_TopLevel, "从首页向下的栏目级别");
                attributes.Add(Attribute_Type, "指定存储图片的字段");
                attributes.Add(Attribute_IsOriginal, "如果是引用内容，是否获取所引用内容的值");
                attributes.Add(Attribute_Src, "显示的图片地址");
                attributes.Add(Attribute_AltSrc, "当指定的图片不存在时显示的图片地址");
                attributes.Add(Attribute_Width, "宽度");
                attributes.Add(Attribute_Height, "高度");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
				return attributes;
			}
		}


		//对“栏目链接”（stl:image）元素进行解析
        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			var parsedContent = string.Empty;
			try
			{
				var stlImage = new HtmlImage();
				var ie = node.Attributes.GetEnumerator();
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

				while (ie.MoveNext())
				{
					var attr = (XmlAttribute)ie.Current;
					var attributeName = attr.Name.ToLower();
					if (attributeName.Equals(Attribute_ChannelIndex))
					{
                        channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
						if (!string.IsNullOrEmpty(channelIndex))
						{
							isGetPicUrlFromAttribute = true;
						}
					}
					else if (attributeName.Equals(Attribute_ChannelName))
					{
                        channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
						if (!string.IsNullOrEmpty(channelName))
						{
							isGetPicUrlFromAttribute = true;
						}
                    }
                    else if (attributeName.Equals(Attribute_NO))
                    {
                        no = TranslateUtils.ToInt(attr.Value);
                    }
					else if (attributeName.Equals(Attribute_Parent))
					{
						if (TranslateUtils.ToBool(attr.Value))
						{
							upLevel = 1;
							isGetPicUrlFromAttribute = true;
						}
					}
					else if (attributeName.Equals(Attribute_UpLevel))
					{
						upLevel = TranslateUtils.ToInt(attr.Value);
						if (upLevel > 0)
						{
							isGetPicUrlFromAttribute = true;
						}
                    }
                    else if (attributeName.Equals(Attribute_TopLevel))
                    {
                        topLevel = TranslateUtils.ToInt(attr.Value);
                        if (topLevel >= 0)
                        {
                            isGetPicUrlFromAttribute = true;
                        }
                    }
                    else if (attributeName.Equals(Attribute_Type))
                    {
                        type = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_IsOriginal))
                    {
                        isOriginal = TranslateUtils.ToBool(attr.Value, true);
                    }
					else if (attributeName.Equals(Attribute_Src))
					{
                        src = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_AltSrc))
                    {
                        altSrc = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value);
                    }
					else
					{
						stlImage.Attributes.Remove(attributeName);
						stlImage.Attributes.Add(attributeName, attr.Value);
					}
				}

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(stlElement, node, pageInfo, contextInfo, stlImage, isGetPicUrlFromAttribute, channelIndex, channelName, no, upLevel, topLevel, type, isOriginal, src, altSrc);
                }
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

            var contentID = 0;
            //判断是否图片地址由标签属性获得
            if (!isGetPicUrlFromAttribute)
            {
                contentID = contextInfo.ContentID;
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
                    if (contentID != 0)
                    {
                        contextType = EContextType.Content;
                    }
                    else
                    {
                        contextType = EContextType.Channel;
                    }
                }

                if (contextType == EContextType.Content)//获取内容图片
                {
                    var contentInfo = contextInfo.ContentInfo;

                    if (isOriginal)
                    {
                        if (contentInfo != null && contentInfo.ReferenceId > 0 && contentInfo.SourceId > 0)
                        {
                            var targetNodeID = contentInfo.SourceId;
                            var targetPublishmentSystemID = DataProvider.NodeDao.GetPublishmentSystemId(targetNodeID);
                            var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemID);
                            var targetNodeInfo = NodeManager.GetNodeInfo(targetPublishmentSystemID, targetNodeID);

                            var tableStyle = NodeManager.GetTableStyle(targetPublishmentSystemInfo, targetNodeInfo);
                            var tableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeInfo);
                            var targetContentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentInfo.ReferenceId);
                            if (targetContentInfo != null || targetContentInfo.NodeId > 0)
                            {
                                contentInfo = targetContentInfo;
                            }
                        }
                    }

                    if (contentInfo == null)
                    {
                        contentInfo = DataProvider.ContentDao.GetContentInfo(ETableStyle.BackgroundContent, pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentID);
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
                                foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
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
                    var channelID = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelID, upLevel, topLevel);

                    channelID = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelID, channelIndex, channelName);

                    var channel = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelID);

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
