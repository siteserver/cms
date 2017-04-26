using System;
using System.Collections.Specialized;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlVideo
	{
        private StlVideo() { }
		public const string ElementName = "stl:video";      //视频播放

		public const string Attribute_ChannelIndex = "channelindex";			//栏目索引
		public const string Attribute_ChannelName = "channelname";				//栏目名称
		public const string Attribute_Parent = "parent";						//显示父栏目
		public const string Attribute_UpLevel = "uplevel";						//上级栏目的级别
        public const string Attribute_TopLevel = "toplevel";					//从首页向下的栏目级别
        public const string Attribute_Type = "type";                            //指定存储媒体的字段

		public const string Attribute_PlayUrl = "playurl";      				    //视频地址
        public const string Attribute_ImageUrl = "imageurl";						//图片地址
        public const string Attribute_Width = "width";							    //宽度
        public const string Attribute_Height = "height";						    //高度
        public const string Attribute_IsAutoPlay = "isautoplay";                    //是否自动播放
        public const string Attribute_IsControls = "iscontrols";                    //是否提供播放控件
        public const string Attribute_IsPreLoad = "ispreload";                      //是否预载入
        public const string Attribute_IsLoop = "isloop";                            //是否循环播放
        public const string Attribute_IsDynamic = "isdynamic";                      //是否动态显示

		public static ListDictionary AttributeList
		{
			get
			{
				var attributes = new ListDictionary();
				attributes.Add(Attribute_ChannelIndex, "栏目索引");
				attributes.Add(Attribute_ChannelName, "栏目名称");
				attributes.Add(Attribute_Parent, "显示父栏目");
				attributes.Add(Attribute_UpLevel, "上级栏目的级别");
                attributes.Add(Attribute_TopLevel, "从首页向下的栏目级别");
                attributes.Add(Attribute_Type, "指定视频的字段");
                attributes.Add(Attribute_PlayUrl, "视频地址");
                attributes.Add(Attribute_ImageUrl, "图片地址");
                attributes.Add(Attribute_Width, "宽度");
                attributes.Add(Attribute_Height, "高度");
                attributes.Add(Attribute_IsAutoPlay, "是否自动播放");
                attributes.Add(Attribute_IsControls, "是否显示播放控件");
                attributes.Add(Attribute_IsPreLoad, "是否预载入");
                attributes.Add(Attribute_IsLoop, "是否循环播放");
                
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
				var isGetUrlFromAttribute = false;
				var channelIndex = string.Empty;
				var channelName = string.Empty;
				var upLevel = 0;
                var topLevel = -1;
                var type = BackgroundContentAttribute.VideoUrl;
				var playUrl = string.Empty;
                var imageUrl = string.Empty;
                var width = pageInfo.PublishmentSystemInfo.Additional.ConfigVideoContentInsertWidth;
                var height = pageInfo.PublishmentSystemInfo.Additional.ConfigVideoContentInsertHeight;
                var isAutoPlay = true;
                var isControls = true;
                var isPreLoad = true;
                var isLoop = false;
                var isDynamic = false;
                var parameters = new NameValueCollection();

				while (ie.MoveNext())
				{
					var attr = (XmlAttribute)ie.Current;
					var attributeName = attr.Name.ToLower();
					if (attributeName.Equals(Attribute_ChannelIndex))
					{
                        channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
						if (!string.IsNullOrEmpty(channelIndex))
						{
                            isGetUrlFromAttribute = true;
						}
					}
                    else if (attributeName.Equals(Attribute_ChannelName))
                    {
                        channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        if (!string.IsNullOrEmpty(channelName))
                        {
                            isGetUrlFromAttribute = true;
                        }
                    }
                    else if (attributeName.Equals(Attribute_Parent))
                    {
                        if (TranslateUtils.ToBool(attr.Value))
                        {
                            upLevel = 1;
                            isGetUrlFromAttribute = true;
                        }
                    }
                    else if (attributeName.Equals(Attribute_UpLevel))
                    {
                        upLevel = TranslateUtils.ToInt(attr.Value);
                        if (upLevel > 0)
                        {
                            isGetUrlFromAttribute = true;
                        }
                    }
                    else if (attributeName.Equals(Attribute_TopLevel))
                    {
                        topLevel = TranslateUtils.ToInt(attr.Value);
                        if (topLevel >= 0)
                        {
                            isGetUrlFromAttribute = true;
                        }
                    }
                    else if (attributeName.Equals(Attribute_Type))
                    {
                        type = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_PlayUrl))
                    {
                        playUrl = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_ImageUrl))
                    {
                        imageUrl = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_Width))
                    {
                        width = TranslateUtils.ToInt(attr.Value, width);
                    }
                    else if (attributeName.Equals(Attribute_Height))
                    {
                        height = TranslateUtils.ToInt(attr.Value, height);
                    }
                    else if (attributeName.Equals(Attribute_IsAutoPlay))
                    {
                        isAutoPlay = TranslateUtils.ToBool(attr.Value, true);
                    }
                    else if (attributeName.Equals(Attribute_IsControls))
                    {
                        isControls = TranslateUtils.ToBool(attr.Value, true);
                    }
                    else if (attributeName.Equals(Attribute_IsPreLoad))
                    {
                        isPreLoad = TranslateUtils.ToBool(attr.Value, true);
                    }
                    else if (attributeName.Equals(Attribute_IsLoop))
                    {
                        isLoop = TranslateUtils.ToBool(attr.Value, false);
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value);
                    }
                    else
                    {
                        parameters.Add(attr.Name, attr.Value);
                    }
				}

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(stlElement, node, pageInfo, contextInfo, isGetUrlFromAttribute, channelIndex, channelName, upLevel, topLevel, type, playUrl, imageUrl, width, height, isAutoPlay, isControls, isPreLoad, isLoop, parameters);
                }
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, bool isGetUrlFromAttribute, string channelIndex, string channelName, int upLevel, int topLevel, string type, string playUrl, string imageUrl, int width, int height, bool isAutoPlay, bool isControls, bool isPreLoad, bool isLoop, NameValueCollection parameters)
        {
            var parsedContent = string.Empty;

            var contentID = 0;
            //判断是否地址由标签属性获得
            if (!isGetUrlFromAttribute)
            {
                contentID = contextInfo.ContentID;
            }

            var videoUrl = string.Empty;
            if (!string.IsNullOrEmpty(playUrl))
            {
                videoUrl = playUrl;
            }
            else
            {
                if (contextInfo.ContextType == EContextType.Content)
                {
                    if (contentID != 0)//获取内容视频
                    {
                        if (contextInfo.ContentInfo == null)
                        {
                            videoUrl = BaiRongDataProvider.ContentDao.GetValue(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentID, type);
                            if (string.IsNullOrEmpty(videoUrl))
                            {
                                if (!StringUtils.EqualsIgnoreCase(type, BackgroundContentAttribute.VideoUrl))
                                {
                                    videoUrl = BaiRongDataProvider.ContentDao.GetValue(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentID, BackgroundContentAttribute.VideoUrl);
                                }
                            }
                            if (string.IsNullOrEmpty(videoUrl))
                            {
                                if (!StringUtils.EqualsIgnoreCase(type, BackgroundContentAttribute.FileUrl))
                                {
                                    videoUrl = BaiRongDataProvider.ContentDao.GetValue(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentID, BackgroundContentAttribute.FileUrl);
                                }
                            }
                        }
                        else
                        {
                            videoUrl = contextInfo.ContentInfo.GetExtendedAttribute(type);
                            if (string.IsNullOrEmpty(videoUrl))
                            {
                                videoUrl = contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.VideoUrl);
                            }
                            if (string.IsNullOrEmpty(videoUrl))
                            {
                                videoUrl = contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.FileUrl);
                            }
                        }
                    }
                }
                else if (contextInfo.ContextType == EContextType.Each)
                {
                    videoUrl = contextInfo.ItemContainer.EachItem.DataItem as string;
                }
            }
            
            if (string.IsNullOrEmpty(imageUrl))
            {
                if (contentID != 0)
                {
                    if (contextInfo.ContentInfo == null)
                    {
                        imageUrl = BaiRongDataProvider.ContentDao.GetValue(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentID, BackgroundContentAttribute.ImageUrl);
                    }
                    else
                    {
                        imageUrl = contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl);
                    }
                }
            }

            if (string.IsNullOrEmpty(imageUrl))
            {
                var channelID = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelID, upLevel, topLevel);
                channelID = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelID, channelIndex, channelName);
                var channel = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelID);
                imageUrl = channel.ImageUrl;
            }

            if (!string.IsNullOrEmpty(videoUrl))
            {
                videoUrl = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, videoUrl);
                imageUrl = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, imageUrl);

                pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAcVideoJs);

                parsedContent =
                    $@"<video class=""video-js vjs-default-skin"" src=""{videoUrl}"" width=""{width}"" height=""{height}"" {(isAutoPlay
                        ? "autoplay"
                        : string.Empty)} {(isControls ? "controls" : string.Empty)} {(isPreLoad
                        ? string.Empty
                        : @"preload=""none""")} {(isLoop ? "loop" : string.Empty)} {(string.IsNullOrEmpty(imageUrl)
                        ? string.Empty
                        : $@"poster=""{imageUrl}""")}></video>";
            }

            return parsedContent;
        }
	}
}
