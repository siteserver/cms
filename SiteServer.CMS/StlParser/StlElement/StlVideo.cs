using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "播放视频", Description = "通过 stl:video 标签在模板中显示视频播放器")]
    public class StlVideo
	{
        private StlVideo() { }
		public const string ElementName = "stl:video";

		private static readonly Attr ChannelIndex = new Attr("channelIndex", "栏目索引");
		private static readonly Attr ChannelName = new Attr("channelName", "栏目名称");
		private static readonly Attr Parent = new Attr("parent", "显示父栏目");
		private static readonly Attr UpLevel = new Attr("upLevel", "上级栏目的级别");
        private static readonly Attr TopLevel = new Attr("topLevel", "从首页向下的栏目级别");
        private static readonly Attr Type = new Attr("type", "指定视频的字段");
		private static readonly Attr PlayUrl = new Attr("playUrl", "视频地址");
        private static readonly Attr ImageUrl = new Attr("imageUrl", "图片地址");
        private static readonly Attr Width = new Attr("width", "宽度");
        private static readonly Attr Height = new Attr("height", "高度");
        private static readonly Attr IsAutoPlay = new Attr("isAutoPlay", "是否自动播放");
        private static readonly Attr IsControls = new Attr("isControls", "是否显示播放控件");
        private static readonly Attr IsPreload = new Attr("isPreload", "是否预载入");
        private static readonly Attr IsLoop = new Attr("isLoop", "是否循环播放");

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
		{
		    var isGetUrlFromAttribute = false;
            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var upLevel = 0;
            var topLevel = -1;
            var type = BackgroundContentAttribute.VideoUrl;
            var playUrl = string.Empty;
            var imageUrl = string.Empty;
            var width = pageInfo.SiteInfo.Additional.ConfigVideoContentInsertWidth;
            var height = pageInfo.SiteInfo.Additional.ConfigVideoContentInsertHeight;
            var isAutoPlay = true;
            var isControls = true;
            var isPreLoad = true;
            var isLoop = false;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, ChannelIndex.Name))
                {
                    channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (!string.IsNullOrEmpty(channelIndex))
                    {
                        isGetUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName.Name))
                {
                    channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (!string.IsNullOrEmpty(channelName))
                    {
                        isGetUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Parent.Name))
                {
                    if (TranslateUtils.ToBool(value))
                    {
                        upLevel = 1;
                        isGetUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, UpLevel.Name))
                {
                    upLevel = TranslateUtils.ToInt(value);
                    if (upLevel > 0)
                    {
                        isGetUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, TopLevel.Name))
                {
                    topLevel = TranslateUtils.ToInt(value);
                    if (topLevel >= 0)
                    {
                        isGetUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Type.Name))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, PlayUrl.Name))
                {
                    playUrl = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ImageUrl.Name))
                {
                    imageUrl = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Width.Name))
                {
                    width = TranslateUtils.ToInt(value, width);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Height.Name))
                {
                    height = TranslateUtils.ToInt(value, height);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsAutoPlay.Name))
                {
                    isAutoPlay = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsControls.Name))
                {
                    isControls = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsPreload.Name))
                {
                    isPreLoad = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsLoop.Name))
                {
                    isLoop = TranslateUtils.ToBool(value, false);
                }
            }

            return ParseImpl(pageInfo, contextInfo, isGetUrlFromAttribute, channelIndex, channelName, upLevel, topLevel, type, playUrl, imageUrl, width, height, isAutoPlay, isControls, isPreLoad, isLoop);
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
                            //videoUrl = DataProvider.ContentDao.GetValue(pageInfo.SiteInfo.AuxiliaryTableForContent, contentId, type);
                            videoUrl = Content.GetValue(pageInfo.SiteInfo.TableName, contentId, type);
                            if (string.IsNullOrEmpty(videoUrl))
                            {
                                if (!StringUtils.EqualsIgnoreCase(type, BackgroundContentAttribute.VideoUrl))
                                {
                                    //videoUrl = DataProvider.ContentDao.GetValue(pageInfo.SiteInfo.AuxiliaryTableForContent, contentId, BackgroundContentAttribute.VideoUrl);
                                    videoUrl = Content.GetValue(pageInfo.SiteInfo.TableName, contentId, BackgroundContentAttribute.VideoUrl);
                                }
                            }
                            //if (string.IsNullOrEmpty(videoUrl))
                            //{
                            //    if (!StringUtils.EqualsIgnoreCase(type, BackgroundContentAttribute.FileUrl))
                            //    {
                            //        //videoUrl = DataProvider.ContentDao.GetValue(pageInfo.SiteInfo.AuxiliaryTableForContent, contentId, BackgroundContentAttribute.FileUrl);
                            //        videoUrl = Content.GetValue(pageInfo.SiteInfo.TableName, contentId, BackgroundContentAttribute.FileUrl);
                            //    }
                            //}
                        }
                        else
                        {
                            videoUrl = contextInfo.ContentInfo.GetString(type);
                            if (string.IsNullOrEmpty(videoUrl))
                            {
                                videoUrl = contextInfo.ContentInfo.GetString(BackgroundContentAttribute.VideoUrl);
                            }
                            //if (string.IsNullOrEmpty(videoUrl))
                            //{
                            //    videoUrl = contextInfo.ContentInfo.GetString(BackgroundContentAttribute.FileUrl);
                            //}
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
                    //imageUrl = contextInfo.ContentInfo == null ? DataProvider.ContentDao.GetValue(pageInfo.SiteInfo.AuxiliaryTableForContent, contentId, BackgroundContentAttribute.ImageUrl) : contextInfo.ContentInfo.GetString(BackgroundContentAttribute.ImageUrl);
                    imageUrl = contextInfo.ContentInfo == null ? Content.GetValue(pageInfo.SiteInfo.TableName, contentId, BackgroundContentAttribute.ImageUrl) : contextInfo.ContentInfo.GetString(BackgroundContentAttribute.ImageUrl);
                }
            }

            if (string.IsNullOrEmpty(imageUrl))
            {
                var channelId = StlDataUtility.GetChannelIdByLevel(pageInfo.SiteId, contextInfo.ChannelId, upLevel, topLevel);
                channelId = StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelName(pageInfo.SiteId, channelId, channelIndex, channelName);
                var channel = ChannelManager.GetChannelInfo(pageInfo.SiteId, channelId);
                imageUrl = channel.ImageUrl;
            }

            if (!string.IsNullOrEmpty(videoUrl))
            {
                videoUrl = PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, videoUrl, pageInfo.IsLocal);
                imageUrl = PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, imageUrl, pageInfo.IsLocal);

                pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.JsAcVideoJs);

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
