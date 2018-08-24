using System.Collections.Generic;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "播放视频", Description = "通过 stl:video 标签在模板中显示视频播放器")]
    public class StlVideo
	{
        private StlVideo() { }
		public const string ElementName = "stl:video";

        private static readonly Attr Type = new Attr("type", "指定视频的字段");

		private static readonly Attr PlayUrl = new Attr("playUrl", "视频地址");
        private static readonly Attr ImageUrl = new Attr("imageUrl", "图片地址");
        private static readonly Attr Width = new Attr("width", "宽度");
        private static readonly Attr Height = new Attr("height", "高度");
        private static readonly Attr IsAutoPlay = new Attr("isAutoPlay", "是否自动播放");
        private static readonly Attr IsControls = new Attr("isControls", "是否显示播放控件");
        private static readonly Attr IsLoop = new Attr("isLoop", "是否循环播放");

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

                if (StringUtils.EqualsIgnoreCase(name, Type.Name))
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
                else if (StringUtils.EqualsIgnoreCase(name, IsLoop.Name))
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
                            videoUrl = Content.GetValue(pageInfo.SiteInfo.TableName, contentId, type);
                            if (string.IsNullOrEmpty(videoUrl))
                            {
                                if (!StringUtils.EqualsIgnoreCase(type, BackgroundContentAttribute.VideoUrl))
                                {
                                    videoUrl = Content.GetValue(pageInfo.SiteInfo.TableName, contentId, BackgroundContentAttribute.VideoUrl);
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
