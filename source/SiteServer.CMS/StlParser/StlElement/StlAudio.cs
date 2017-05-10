using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

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
        public const string AttributeIsDynamic = "isDynamic";

		public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
		    {AttributeType, "指定存储音频的内容字段，默认为VideoUrl"},
		    {AttributePlayUrl, "音频地址"},
		    {AttributeIsAutoPlay, "是否自动播放"},
		    {AttributeIsPreLoad, "是否预载入"},
		    {AttributeIsLoop, "是否循环播放"},
		    {AttributeIsDynamic, "是否动态显示"}
		};

	    public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			string parsedContent;
			try
			{
				var ie = node.Attributes?.GetEnumerator();
                var type = BackgroundContentAttribute.VideoUrl;
				var playUrl = string.Empty;
                var isAutoPlay = false;
                var isPreLoad = true;
                var isLoop = false;
                var isDynamic = false;
                var parameters = new NameValueCollection();

			    if (ie != null)
			    {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeType))
                        {
                            type = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributePlayUrl))
                        {
                            playUrl = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsAutoPlay))
                        {
                            isAutoPlay = TranslateUtils.ToBool(attr.Value, false);
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
                        else
                        {
                            parameters.Add(attr.Name, attr.Value);
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(pageInfo, contextInfo, type, playUrl, isAutoPlay, isPreLoad, isLoop);
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
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
                        playUrl = BaiRongDataProvider.ContentDao.GetValue(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentId, type);
                        if (string.IsNullOrEmpty(playUrl))
                        {
                            if (!StringUtils.EqualsIgnoreCase(type, BackgroundContentAttribute.VideoUrl))
                            {
                                playUrl = BaiRongDataProvider.ContentDao.GetValue(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentId, BackgroundContentAttribute.VideoUrl);
                            }
                        }
                        if (string.IsNullOrEmpty(playUrl))
                        {
                            if (!StringUtils.EqualsIgnoreCase(type, BackgroundContentAttribute.FileUrl))
                            {
                                playUrl = BaiRongDataProvider.ContentDao.GetValue(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentId, BackgroundContentAttribute.FileUrl);
                            }
                        }
                    }
                    else
                    {
                        playUrl = contextInfo.ContentInfo.GetExtendedAttribute(type);
                        if (string.IsNullOrEmpty(playUrl))
                        {
                            playUrl = contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.VideoUrl);
                        }
                        if (string.IsNullOrEmpty(playUrl))
                        {
                            playUrl = contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.FileUrl);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(playUrl)) return string.Empty;

            playUrl = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, playUrl);

            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAcMediaElement);

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
