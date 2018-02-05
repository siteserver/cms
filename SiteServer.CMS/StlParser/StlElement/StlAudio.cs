using System.Collections.Generic;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "播放音频", Description = "通过 stl:audio 标签在模板中显示并播放音频文件")]
    public class StlAudio
	{
        private StlAudio() { }
		public const string ElementName = "stl:audio";

        public const string AttributeType = "type";
        public const string AttributePlayUrl = "playUrl";
        public const string AttributeIsAutoPlay = "isAutoPlay";
        public const string AttributeIsPreLoad = "isPreload";
        public const string AttributeIsLoop = "isLoop";

		public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
		    {AttributeType, "指定存储音频的内容字段，默认为VideoUrl"},
		    {AttributePlayUrl, "音频地址"},
		    {AttributeIsAutoPlay, "是否自动播放"},
		    {AttributeIsPreLoad, "是否预载入"},
		    {AttributeIsLoop, "是否循环播放"},
		};

	    public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
		{
            var type = BackgroundContentAttribute.VideoUrl;
            var playUrl = string.Empty;
            var isAutoPlay = false;
            var isPreLoad = true;
            var isLoop = false;

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeType))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributePlayUrl))
                {
                    playUrl = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsAutoPlay))
                {
                    isAutoPlay = TranslateUtils.ToBool(value, false);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsPreLoad))
                {
                    isPreLoad = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsLoop))
                {
                    isLoop = TranslateUtils.ToBool(value, false);
                }
            }

            return ParseImpl(pageInfo, contextInfo, type, playUrl, isAutoPlay, isPreLoad, isLoop);
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string type, string playUrl, bool isAutoPlay, bool isPreLoad, bool isLoop)
        {
            var contentId = contextInfo.ContentId;

            if (string.IsNullOrEmpty(playUrl))
            {
                if (contentId != 0)//获取内容视频
                {
                    if (contextInfo.ContentInfo == null)
                    {
                        //playUrl = DataProvider.ContentDao.GetValue(pageInfo.SiteInfo.AuxiliaryTableForContent, contentId, type);
                        playUrl = Content.GetValue(pageInfo.SiteInfo.TableName, contentId, type);
                        if (string.IsNullOrEmpty(playUrl))
                        {
                            if (!StringUtils.EqualsIgnoreCase(type, BackgroundContentAttribute.VideoUrl))
                            {
                                //playUrl = DataProvider.ContentDao.GetValue(pageInfo.SiteInfo.AuxiliaryTableForContent, contentId, BackgroundContentAttribute.VideoUrl);
                                playUrl = Content.GetValue(pageInfo.SiteInfo.TableName, contentId, BackgroundContentAttribute.VideoUrl);
                            }
                        }
                        if (string.IsNullOrEmpty(playUrl))
                        {
                            if (!StringUtils.EqualsIgnoreCase(type, BackgroundContentAttribute.FileUrl))
                            {
                                //playUrl = DataProvider.ContentDao.GetValue(pageInfo.SiteInfo.AuxiliaryTableForContent, contentId, BackgroundContentAttribute.FileUrl);
                                playUrl = Content.GetValue(pageInfo.SiteInfo.TableName, contentId, BackgroundContentAttribute.FileUrl);
                            }
                        }
                    }
                    else
                    {
                        playUrl = contextInfo.ContentInfo.GetString(type);
                        if (string.IsNullOrEmpty(playUrl))
                        {
                            playUrl = contextInfo.ContentInfo.GetString(BackgroundContentAttribute.VideoUrl);
                        }
                        if (string.IsNullOrEmpty(playUrl))
                        {
                            playUrl = contextInfo.ContentInfo.GetString(BackgroundContentAttribute.FileUrl);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(playUrl)) return string.Empty;

            playUrl = PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, playUrl, pageInfo.IsLocal);

            // 如果是实体标签，则只返回数字
            if (contextInfo.IsCurlyBrace)
            {
                return playUrl;
            }
            else
            {
                pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.Jquery);
                pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.JsAcMediaElement);

                return $@"
<audio src=""{playUrl}"" {(isAutoPlay ? "autoplay" : string.Empty)} {(isPreLoad ? string.Empty : @"preload=""none""")} {(isLoop ? "loop" : string.Empty)}>
    <object width=""460"" height=""40"" type=""application/x-shockwave-flash"" data=""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.MediaElement.Swf)}"">
        <param name=""movie"" value=""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.MediaElement.Swf)}"" />
        <param name=""flashvars"" value=""controls=true&file={playUrl}"" />
    </object>
</audio>
<script>$('audio').mediaelementplayer();</script>
";
            }
        }
    }
}
