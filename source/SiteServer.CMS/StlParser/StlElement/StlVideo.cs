using System;
using System.Collections.Generic;
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
    [Stl(Usage = "播放视频", Description = "通过 stl:video 标签在模板中显示视频播放器")]
    public class StlVideo
	{
        private StlVideo() { }
		public const string ElementName = "stl:video";

		public const string AttributeChannelIndex = "channelIndex";
		public const string AttributeChannelName = "channelName";
		public const string AttributeParent = "parent";
		public const string AttributeUpLevel = "upLevel";
        public const string AttributeTopLevel = "topLevel";
        public const string AttributeType = "type";
		public const string AttributePlayUrl = "playUrl";
        public const string AttributeImageUrl = "imageUrl";
        public const string AttributeWidth = "width";
        public const string AttributeHeight = "height";
        public const string AttributeIsAutoPlay = "isAutoPlay";
        public const string AttributeIsControls = "isControls";
        public const string AttributeIsPreLoad = "isPreload";
        public const string AttributeIsLoop = "isLoop";
        public const string AttributeIsDynamic = "isDynamic";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
	    {
	        {AttributeChannelIndex, "栏目索引"},
	        {AttributeChannelName, "栏目名称"},
	        {AttributeParent, "显示父栏目"},
	        {AttributeUpLevel, "上级栏目的级别"},
	        {AttributeTopLevel, "从首页向下的栏目级别"},
	        {AttributeType, "指定视频的字段"},
	        {AttributePlayUrl, "视频地址"},
	        {AttributeImageUrl, "图片地址"},
	        {AttributeWidth, "宽度"},
	        {AttributeHeight, "高度"},
	        {AttributeIsAutoPlay, "是否自动播放"},
	        {AttributeIsControls, "是否显示播放控件"},
	        {AttributeIsPreLoad, "是否预载入"},
	        {AttributeIsLoop, "是否循环播放"},
	        {AttributeIsDynamic, "是否动态显示"}
	    };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			string parsedContent;
			try
			{
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

                var ie = node.Attributes?.GetEnumerator();
			    if (ie != null)
			    {
			        while (ie.MoveNext())
			        {
			            var attr = (XmlAttribute) ie.Current;

			            if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChannelIndex))
			            {
			                channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
			                if (!string.IsNullOrEmpty(channelIndex))
			                {
			                    isGetUrlFromAttribute = true;
			                }
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChannelName))
			            {
			                channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
			                if (!string.IsNullOrEmpty(channelName))
			                {
			                    isGetUrlFromAttribute = true;
			                }
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeParent))
			            {
			                if (TranslateUtils.ToBool(attr.Value))
			                {
			                    upLevel = 1;
			                    isGetUrlFromAttribute = true;
			                }
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeUpLevel))
			            {
			                upLevel = TranslateUtils.ToInt(attr.Value);
			                if (upLevel > 0)
			                {
			                    isGetUrlFromAttribute = true;
			                }
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTopLevel))
			            {
			                topLevel = TranslateUtils.ToInt(attr.Value);
			                if (topLevel >= 0)
			                {
			                    isGetUrlFromAttribute = true;
			                }
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeType))
			            {
			                type = attr.Value;
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributePlayUrl))
			            {
			                playUrl = attr.Value;
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeImageUrl))
			            {
			                imageUrl = attr.Value;
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeWidth))
			            {
			                width = TranslateUtils.ToInt(attr.Value, width);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeHeight))
			            {
			                height = TranslateUtils.ToInt(attr.Value, height);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsAutoPlay))
			            {
			                isAutoPlay = TranslateUtils.ToBool(attr.Value, true);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsControls))
			            {
			                isControls = TranslateUtils.ToBool(attr.Value, true);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsPreLoad))
			            {
			                isPreLoad = TranslateUtils.ToBool(attr.Value, true);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsLoop))
			            {
			                isLoop = TranslateUtils.ToBool(attr.Value, false);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
			            {
			                isDynamic = TranslateUtils.ToBool(attr.Value);
			            }
			        }
			    }

			    if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(pageInfo, contextInfo, isGetUrlFromAttribute, channelIndex, channelName, upLevel, topLevel, type, playUrl, imageUrl, width, height, isAutoPlay, isControls, isPreLoad, isLoop);
                }
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, bool isGetUrlFromAttribute, string channelIndex, string channelName, int upLevel, int topLevel, string type, string playUrl, string imageUrl, int width, int height, bool isAutoPlay, bool isControls, bool isPreLoad, bool isLoop)
        {
            var parsedContent = string.Empty;

            var contentId = 0;
            //判断是否地址由标签属性获得
            if (!isGetUrlFromAttribute)
            {
                contentId = contextInfo.ContentId;
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
                    if (contentId != 0)//获取内容视频
                    {
                        if (contextInfo.ContentInfo == null)
                        {
                            videoUrl = BaiRongDataProvider.ContentDao.GetValue(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentId, type);
                            if (string.IsNullOrEmpty(videoUrl))
                            {
                                if (!StringUtils.EqualsIgnoreCase(type, BackgroundContentAttribute.VideoUrl))
                                {
                                    videoUrl = BaiRongDataProvider.ContentDao.GetValue(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentId, BackgroundContentAttribute.VideoUrl);
                                }
                            }
                            if (string.IsNullOrEmpty(videoUrl))
                            {
                                if (!StringUtils.EqualsIgnoreCase(type, BackgroundContentAttribute.FileUrl))
                                {
                                    videoUrl = BaiRongDataProvider.ContentDao.GetValue(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentId, BackgroundContentAttribute.FileUrl);
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
                if (contentId != 0)
                {
                    if (contextInfo.ContentInfo == null)
                    {
                        imageUrl = BaiRongDataProvider.ContentDao.GetValue(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentId, BackgroundContentAttribute.ImageUrl);
                    }
                    else
                    {
                        imageUrl = contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl);
                    }
                }
            }

            if (string.IsNullOrEmpty(imageUrl))
            {
                var channelId = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelId, upLevel, topLevel);
                channelId = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelId, channelIndex, channelName);
                var channel = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelId);
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
