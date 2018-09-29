using System.Collections.Generic;
using System.Text;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "播放视频", Description = "通过 stl:player 标签在模板中播放视频")]
    public class StlPlayer
	{
        private StlPlayer() { }
		public const string ElementName = "stl:player";

        [StlAttribute(Title = "指定存储媒体的字段")]
        public const string Type = nameof(Type);

	    [StlAttribute(Title = "视频地址")]
	    public const string PlayUrl = nameof(PlayUrl);
        
	    [StlAttribute(Title = "图片地址")]
	    public const string ImageUrl = nameof(ImageUrl);
        
	    [StlAttribute(Title = "指定播放器")]
	    public const string PlayBy = nameof(PlayBy);
        
	    [StlAttribute(Title = "宽度")]
	    public const string Width = nameof(Width);
        
	    [StlAttribute(Title = "高度")]
        public const string Height = nameof(Height);
        
	    [StlAttribute(Title = "是否自动播放")]
	    public const string IsAutoPlay = nameof(IsAutoPlay);

	    private const string PlayByHtml5 = "Html5";
        private const string PlayByFlowPlayer = "FlowPlayer";
	    private const string PlayByJwPlayer = "JWPlayer";

        public static List<string> PlayByList => new List<string>
        {
            PlayByHtml5,
            PlayByFlowPlayer,
            PlayByJwPlayer
        };

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
		{
            var type = BackgroundContentAttribute.VideoUrl;
            var playUrl = string.Empty;
            var imageUrl = string.Empty;
            var playBy = string.Empty;
            var width = 450;
            var height = 350;
            var isAutoPlay = true;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, PlayUrl) || StringUtils.EqualsIgnoreCase(name, "src"))
                {
                    playUrl = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ImageUrl))
                {
                    imageUrl = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, PlayBy))
                {
                    playBy = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Width))
                {
                    width = TranslateUtils.ToInt(value, width);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Height))
                {
                    height = TranslateUtils.ToInt(value, height);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsAutoPlay) || StringUtils.EqualsIgnoreCase(name, "play"))
                {
                    isAutoPlay = TranslateUtils.ToBool(value, true);
                }
            }

            return ParseImpl(pageInfo, contextInfo, playUrl, imageUrl, playBy, width, height, type, isAutoPlay);
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string playUrl, string imageUrl, string playBy, int width, int height, string type, bool isAutoPlay)
        {
            if (string.IsNullOrEmpty(playUrl))
            {
                var contentId = contextInfo.ContentId;
                if (contentId != 0)//获取内容视频
                {
                    if (contextInfo.ContentInfo == null)
                    {
                        playUrl = StlContentCache.GetValue(pageInfo.SiteInfo.TableName, contentId, type);
                        if (string.IsNullOrEmpty(playUrl))
                        {
                            if (!StringUtils.EqualsIgnoreCase(type, BackgroundContentAttribute.VideoUrl))
                            {
                                playUrl = StlContentCache.GetValue(pageInfo.SiteInfo.TableName, contentId, BackgroundContentAttribute.VideoUrl);
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
                    }
                }
            }

            if (string.IsNullOrEmpty(playUrl)) return string.Empty;

            playUrl = PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, playUrl, pageInfo.IsLocal);
            imageUrl = PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, imageUrl, pageInfo.IsLocal);

            var extension = PathUtils.GetExtension(playUrl);
            var uniqueId = pageInfo.UniqueId;

            var fileType = EFileSystemTypeUtils.GetEnumType(extension);

            if (EFileSystemTypeUtils.IsFlash(extension))
            {
                return StlFlash.Parse(pageInfo, contextInfo);
            }

            if (EFileSystemTypeUtils.IsImage(extension))
            {
                return StlImage.Parse(pageInfo, contextInfo);
            }

            if (fileType == EFileSystemType.Avi)
            {
                return ParseAvi(uniqueId, width, height, isAutoPlay, playUrl);
            }

            if (fileType == EFileSystemType.Mpg)
            {
                return ParseMpg(uniqueId, width, height, isAutoPlay, playUrl);
            }

            if (fileType == EFileSystemType.Rm || fileType == EFileSystemType.Rmb || fileType == EFileSystemType.Rmvb)
            {
                return ParseRm(contextInfo, uniqueId, width, height, isAutoPlay, playUrl);
            }

            if (fileType == EFileSystemType.Wmv)
            {
                return ParseWmv(uniqueId, width, height, isAutoPlay, playUrl);
            }

            if (fileType == EFileSystemType.Wma)
            {
                return ParseWma(uniqueId, isAutoPlay, playUrl);
            }

            if (StringUtils.EqualsIgnoreCase(playBy, PlayByJwPlayer))
            {
                pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.JsAcJwPlayer6);
                var ajaxElementId = StlParserUtility.GetAjaxDivId(pageInfo.UniqueId);
                return $@"
<div id='{ajaxElementId}'></div>
<script type='text/javascript'>
	jwplayer('{ajaxElementId}').setup({{
        autostart: {isAutoPlay.ToString().ToLower()},
		file: ""{playUrl}"",
		width: ""{width}"",
		height: ""{height}"",
		image: ""{imageUrl}""
	}});
</script>
";
            }

            if (StringUtils.EqualsIgnoreCase(playBy, PlayByFlowPlayer))
            {
                var ajaxElementId = StlParserUtility.GetAjaxDivId(pageInfo.UniqueId);
                pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.JsAcFlowPlayer);

                var imageHtml = string.Empty;
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    imageHtml = $@"<img src=""{imageUrl}"" style=""{(width > 0 ? $"width:{width}px;" : string.Empty)}{(height > 0 ? $"height:{height}px;" : string.Empty)}"" />";
                }

                var swfUrl = SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.FlowPlayer.Swf);
                return $@"
<a href=""{playUrl}"" style=""display:block;{(width > 0 ? $"width:{width}px;" : string.Empty)}{(height > 0 ? $"height:{height}px;" : string.Empty)}"" id=""player_{ajaxElementId}"">{imageHtml}</a>
<script language=""javascript"">
    flowplayer(""player_{ajaxElementId}"", ""{swfUrl}"", {{
        clip:  {{
            autoPlay: {isAutoPlay.ToString().ToLower()}
        }}
    }});
</script>
";
            }

            return StlVideo.Parse(pageInfo, contextInfo);
        }

        private static string ParseAvi(int uniqueId, int width, int height, bool isAutoPlay, string playUrl)
        {
            return $@"
<object id=""palyer_{uniqueId}"" width=""{width}"" height=""{height}"" border=""0"" classid=""clsid:CFCDAA03-8BE4-11cf-B84B-0020AFBBCCFA"">
<param name=""ShowDisplay"" value=""0"">
<param name=""ShowControls"" value=""1"">
<param name=""AutoStart"" value=""{(isAutoPlay ? "1" : "0")}"">
<param name=""AutoRewind"" value=""0"">
<param name=""PlayCount"" value=""0"">
<param name=""Appearance"" value=""0"">
<param name=""BorderStyle"" value=""0"">
<param name=""MovieWindowHeight"" value=""240"">
<param name=""MovieWindowWidth"" value=""320"">
<param name=""FileName"" value=""{playUrl}"">
<embed width=""{width}"" height=""{height}"" border=""0"" showdisplay=""0"" showcontrols=""1"" autostart=""{(isAutoPlay
                ? "1"
                : "0")}"" autorewind=""0"" playcount=""0"" moviewindowheight=""240"" moviewindowwidth=""320"" filename=""{playUrl}"" src=""{playUrl}"">
</embed>
</object>
";
        }

        private static string ParseMpg(int uniqueId, int width, int height, bool isAutoPlay, string playUrl)
        {
            return $@"
<object classid=""clsid:05589FA1-C356-11CE-BF01-00AA0055595A"" id=""palyer_{uniqueId}"" width=""{width}"" height=""{height}"">
<param name=""Appearance"" value=""0"">
<param name=""AutoStart"" value=""{(isAutoPlay ? "true" : "false")}"">
<param name=""AllowChangeDisplayMode"" value=""-1"">
<param name=""AllowHideDisplay"" value=""0"">
<param name=""AllowHideControls"" value=""-1"">
<param name=""AutoRewind"" value=""-1"">
<param name=""Balance"" value=""0"">
<param name=""CurrentPosition"" value=""0"">
<param name=""DisplayBackColor"" value=""0"">
<param name=""DisplayForeColor"" value=""16777215"">
<param name=""DisplayMode"" value=""0"">
<param name=""Enabled"" value=""-1"">
<param name=""EnableContextMenu"" value=""-1"">
<param name=""EnablePositionControls"" value=""-1"">
<param name=""EnableSelectionControls"" value=""0"">
<param name=""EnableTracker"" value=""-1"">
<param name=""Filename"" value=""{playUrl}"" valuetype=""ref"">
<param name=""FullScreenMode"" value=""0"">
<param name=""MovieWindowSize"" value=""0"">
<param name=""PlayCount"" value=""1"">
<param name=""Rate"" value=""1"">
<param name=""SelectionStart"" value=""-1"">
<param name=""SelectionEnd"" value=""-1"">
<param name=""ShowControls"" value=""-1"">
<param name=""ShowDisplay"" value=""-1"">
<param name=""ShowPositionControls"" value=""0"">
<param name=""ShowTracker"" value=""-1"">
<param name=""Volume"" value=""-480"">
</object>
";
        }

        private static string ParseRm(ContextInfo contextInfo, int uniqueId, int width, int height, bool isAutoPlay, string playUrl)
        {
            if (string.IsNullOrEmpty(contextInfo.Attributes["ShowDisplay"]))
            {
                contextInfo.Attributes["ShowDisplay"] = "0";
            }
            if (string.IsNullOrEmpty(contextInfo.Attributes["ShowControls"]))
            {
                contextInfo.Attributes["ShowControls"] = "1";
            }
            contextInfo.Attributes["AutoStart"] = isAutoPlay ? "1" : "0";
            if (string.IsNullOrEmpty(contextInfo.Attributes["AutoRewind"]))
            {
                contextInfo.Attributes["AutoRewind"] = "0";
            }
            if (string.IsNullOrEmpty(contextInfo.Attributes["PlayCount"]))
            {
                contextInfo.Attributes["PlayCount"] = "0";
            }
            if (string.IsNullOrEmpty(contextInfo.Attributes["Appearance"]))
            {
                contextInfo.Attributes["Appearance"] = "0";
            }
            if (string.IsNullOrEmpty(contextInfo.Attributes["BorderStyle"]))
            {
                contextInfo.Attributes["BorderStyle"] = "0";
            }
            if (string.IsNullOrEmpty(contextInfo.Attributes["Controls"]))
            {
                contextInfo.Attributes["ImageWindow"] = "0";
            }
            contextInfo.Attributes["moviewindowheight"] = height.ToString();
            contextInfo.Attributes["moviewindowwidth"] = width.ToString();
            contextInfo.Attributes["filename"] = playUrl;
            contextInfo.Attributes["src"] = playUrl;

            var paramBuilder = new StringBuilder();
            var embedBuilder = new StringBuilder();
            foreach (string key in contextInfo.Attributes.Keys)
            {
                paramBuilder.Append($@"<param name=""{key}"" value=""{contextInfo.Attributes[key]}"">").Append(StringUtils.Constants.ReturnAndNewline);
                embedBuilder.Append($@" {key}=""{contextInfo.Attributes[key]}""");
            }

            return $@"
<object id=""video_{uniqueId}"" width=""{width}"" height=""{height}"" border=""0"" classid=""clsid:CFCDAA03-8BE4-11cf-B84B-0020AFBBCCFA"">
{paramBuilder}
<embed{embedBuilder}>
</embed>
</object>
";
        }

        private static string ParseWmv(int uniqueId, int width, int height, bool isAutoPlay, string playUrl)
        {
            return $@"
<object id=""palyer_{uniqueId}"" WIDTH=""{width}"" HEIGHT=""{height}"" classid=""CLSID:22d6f312-b0f6-11d0-94ab-0080c74c7e95"" codebase=""http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=6,4,5,715"" standby=""Loading Microsoft Windows Media Player components..."" type=""application/x-oleobject"" align=""right"" hspace=""5"">
<param name=""AutoRewind"" value=""1"">
<param name=""ShowControls"" value=""1"">
<param name=""ShowPositionControls"" value=""0"">
<param name=""ShowAudioControls"" value=""1"">
<param name=""ShowTracker"" value=""0"">
<param name=""ShowDisplay"" value=""0"">
<param name=""ShowStatusBar"" value=""0"">
<param name=""ShowGotoBar"" value=""0"">
<param name=""ShowCaptioning"" value=""0"">
<param name=""AutoStart"" value=""{(isAutoPlay ? "1" : "0")}"">
<param name=""FileName"" value=""{playUrl}"">
<param name=""Volume"" value=""-2500"">
<param name=""AnimationAtStart"" value=""0"">
<param name=""TransparentAtStart"" value=""0"">
<param name=""AllowChangeDisplaySize"" value=""0"">
<param name=""AllowScan"" value=""0"">
<param name=""EnableContextMenu"" value=""0"">
<param name=""ClickToPlay"" value=""0"">
</object>
";
        }

        private static string ParseWma(int uniqueId, bool isAutoPlay, string playUrl)
        {
            return $@"
<object classid=""clsid:22D6F312-B0F6-11D0-94AB-0080C74C7E95"" id=""palyer_{uniqueId}"">
<param name=""Filename"" value=""{playUrl}"">
<param name=""PlayCount"" value=""1"">
<param name=""AutoStart"" value=""{(isAutoPlay ? "1" : "0")}"">
<param name=""ClickToPlay"" value=""1"">
<param name=""DisplaySize"" value=""0"">
<param name=""EnableFullScreen Controls"" value=""1"">
<param name=""ShowAudio Controls"" value=""1"">
<param name=""EnableContext Menu"" value=""1"">
<param name=""ShowDisplay"" value=""1"">
</object>
";
        }
    }
}
