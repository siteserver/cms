using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.StlParser.StlElement
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

        public static async Task<object> ParseAsync(ParseContext parseContext)
        {
            var type = ContentAttribute.VideoUrl;
            var playUrl = string.Empty;
            var imageUrl = string.Empty;
            var playBy = string.Empty;
            var width = 450;
            var height = 350;
            var isAutoPlay = true;

            foreach (var name in parseContext.Attributes.AllKeys)
            {
                var value = parseContext.Attributes[name];

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

            return await ParseImplAsync(parseContext, playUrl, imageUrl, playBy, width, height, type, isAutoPlay);
        }

        private static async Task<object> ParseImplAsync(ParseContext parseContext, string playUrl, string imageUrl, string playBy, int width, int height, string type, bool isAutoPlay)
        {
            if (string.IsNullOrEmpty(playUrl))
            {
                var contentInfo = await parseContext.GetContentInfoAsync();

                if (contentInfo != null)
                {
                    playUrl = contentInfo.Get<string>(type);
                    if (string.IsNullOrEmpty(playUrl))
                    {
                        playUrl = contentInfo.Get<string>(ContentAttribute.VideoUrl);
                    }
                }
            }

            if (string.IsNullOrEmpty(playUrl)) return string.Empty;

            playUrl = parseContext.UrlManager.ParseNavigationUrl(parseContext.SiteInfo, playUrl, parseContext.IsLocal);
            imageUrl = parseContext.UrlManager.ParseNavigationUrl(parseContext.SiteInfo, imageUrl, parseContext.IsLocal);

            var extension = PathUtils.GetExtension(playUrl);
            var uniqueId = parseContext.UniqueId;

            var fileType = EFileSystemTypeUtils.GetEnumType(extension);

            if (EFileSystemTypeUtils.IsFlash(extension))
            {
                return await  StlFlash.ParseAsync(parseContext);
            }

            if (EFileSystemTypeUtils.IsImage(extension))
            {
                return await StlImage.ParseAsync(parseContext);
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
                return ParseRm(parseContext, uniqueId, width, height, isAutoPlay, playUrl);
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
                parseContext.PageInfo.AddPageBodyCodeIfNotExists(parseContext.UrlManager, PageInfo.Const.JsAcJwPlayer6);
                var ajaxElementId = StlParserUtility.GetAjaxDivId(parseContext.UniqueId);
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
                var ajaxElementId = StlParserUtility.GetAjaxDivId(parseContext.UniqueId);
                parseContext.PageInfo.AddPageBodyCodeIfNotExists(parseContext.UrlManager, PageInfo.Const.JsAcFlowPlayer);

                var imageHtml = string.Empty;
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    imageHtml = $@"<img src=""{imageUrl}"" style=""{(width > 0 ? $"width:{width}px;" : string.Empty)}{(height > 0 ? $"height:{height}px;" : string.Empty)}"" />";
                }

                var swfUrl = SiteFilesAssets.GetUrl(SiteFilesAssets.FlowPlayer.Swf);
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

            return await StlVideo.ParseAsync(parseContext);
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

        private static string ParseRm(ParseContext parseContext, int uniqueId, int width, int height, bool isAutoPlay, string playUrl)
        {
            if (string.IsNullOrEmpty(parseContext.Attributes["ShowDisplay"]))
            {
                parseContext.Attributes["ShowDisplay"] = "0";
            }
            if (string.IsNullOrEmpty(parseContext.Attributes["ShowControls"]))
            {
                parseContext.Attributes["ShowControls"] = "1";
            }
            parseContext.Attributes["AutoStart"] = isAutoPlay ? "1" : "0";
            if (string.IsNullOrEmpty(parseContext.Attributes["AutoRewind"]))
            {
                parseContext.Attributes["AutoRewind"] = "0";
            }
            if (string.IsNullOrEmpty(parseContext.Attributes["PlayCount"]))
            {
                parseContext.Attributes["PlayCount"] = "0";
            }
            if (string.IsNullOrEmpty(parseContext.Attributes["Appearance"]))
            {
                parseContext.Attributes["Appearance"] = "0";
            }
            if (string.IsNullOrEmpty(parseContext.Attributes["BorderStyle"]))
            {
                parseContext.Attributes["BorderStyle"] = "0";
            }
            if (string.IsNullOrEmpty(parseContext.Attributes["Controls"]))
            {
                parseContext.Attributes["ImageWindow"] = "0";
            }
            parseContext.Attributes["moviewindowheight"] = height.ToString();
            parseContext.Attributes["moviewindowwidth"] = width.ToString();
            parseContext.Attributes["filename"] = playUrl;
            parseContext.Attributes["src"] = playUrl;

            var paramBuilder = new StringBuilder();
            var embedBuilder = new StringBuilder();
            foreach (string key in parseContext.Attributes.Keys)
            {
                paramBuilder.Append($@"<param name=""{key}"" value=""{parseContext.Attributes[key]}"">").Append(Constants.ReturnAndNewline);
                embedBuilder.Append($@" {key}=""{parseContext.Attributes[key]}""");
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
