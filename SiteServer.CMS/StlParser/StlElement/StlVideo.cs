using System.Collections.Generic;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "播放视频", Description = "通过 stl:video 标签在模板中显示视频播放器")]
    public class StlVideo
	{
        private StlVideo() { }
		public const string ElementName = "stl:video";

        [StlAttribute(Title = "指定视频的字段")]
        private const string Type = nameof(Type);
         
		[StlAttribute(Title = "视频地址")]
        private const string PlayUrl = nameof(PlayUrl);
         
        [StlAttribute(Title = "图片地址")]
        private const string ImageUrl = nameof(ImageUrl);
         
        [StlAttribute(Title = "宽度")]
        private const string Width = nameof(Width);
         
        [StlAttribute(Title = "高度")]
        private const string Height = nameof(Height);
         
        [StlAttribute(Title = "是否自动播放")]
        private const string IsAutoPlay = nameof(IsAutoPlay);
         
        [StlAttribute(Title = "是否显示播放控件")]
        private const string IsControls = nameof(IsControls);
         
        [StlAttribute(Title = "是否循环播放")]
        private const string IsLoop = nameof(IsLoop);

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
		{
            var type = BackgroundContentAttribute.VideoUrl;
            var playUrl = string.Empty;
            var imageUrl = string.Empty;
            var width = 0;
            var height = 280;
            var isAutoPlay = true;
            var isControls = true;
            var isLoop = false;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, PlayUrl))
                {
                    playUrl = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ImageUrl))
                {
                    imageUrl = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Width))
                {
                    width = TranslateUtils.ToInt(value, width);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Height))
                {
                    height = TranslateUtils.ToInt(value, height);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsAutoPlay))
                {
                    isAutoPlay = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsControls))
                {
                    isControls = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsLoop))
                {
                    isLoop = TranslateUtils.ToBool(value, false);
                }
            }

            return ParseImpl(pageInfo, contextInfo, type, playUrl, imageUrl, width, height, isAutoPlay, isControls, isLoop);
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string type, string playUrl, string imageUrl, int width, int height, bool isAutoPlay, bool isControls, bool isLoop)
        {
            

            var videoUrl = string.Empty;
            if (!string.IsNullOrEmpty(playUrl))
            {
                videoUrl = playUrl;
            }
            else
            {
                var contentId = contextInfo.ContentId;
                if (contextInfo.ContextType == EContextType.Content)
                {
                    if (contentId != 0)//获取内容视频
                    {
                        if (contextInfo.ContentInfo == null)
                        {
                            videoUrl = StlContentCache.GetValue(pageInfo.SiteInfo.TableName, contentId, type);
                            if (string.IsNullOrEmpty(videoUrl))
                            {
                                if (!StringUtils.EqualsIgnoreCase(type, BackgroundContentAttribute.VideoUrl))
                                {
                                    videoUrl = StlContentCache.GetValue(pageInfo.SiteInfo.TableName, contentId, BackgroundContentAttribute.VideoUrl);
                                }
                            }
                        }
                        else
                        {
                            videoUrl = contextInfo.ContentInfo.GetString(type);
                            if (string.IsNullOrEmpty(videoUrl))
                            {
                                videoUrl = contextInfo.ContentInfo.GetString(BackgroundContentAttribute.VideoUrl);
                            }
                        }
                    }
                }
                else if (contextInfo.ContextType == EContextType.Each)
                {
                    videoUrl = contextInfo.ItemContainer.EachItem.DataItem as string;
                }
            }

            if (string.IsNullOrEmpty(videoUrl)) return string.Empty;

            videoUrl = PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, videoUrl, pageInfo.IsLocal);
            imageUrl = PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, imageUrl, pageInfo.IsLocal);

            pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.JsAcVideoJs);

            var dict = new Dictionary<string, string>
            {
                {"class", "video-js vjs-default-skin"},
                {"src", videoUrl}
            };
            if (isAutoPlay)
            {
                dict.Add("autoplay", null);
            }
            if (isControls)
            {
                dict.Add("controls", null);
            }
            if (isLoop)
            {
                dict.Add("loop", null);
            }
            if (!string.IsNullOrEmpty(imageUrl))
            {
                dict.Add("poster", imageUrl);
            }
            if (width > 0)
            {
                dict.Add("width", width.ToString());
            }
            if (height > 0)
            {
                dict.Add("height", height.ToString());
            }

            return $@"<video {TranslateUtils.ToAttributesString(dict)}></video>";
        }
	}
}
