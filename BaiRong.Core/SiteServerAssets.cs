using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core
{
	public class SiteServerAssets
	{
        public const string DirectoryName = "assets";

        public static string GetUrl(string relatedUrl)
        {
            return PageUtils.Combine(PageUtils.GetAdminDirectoryUrl(DirectoryName), relatedUrl);
        }

        public static string GetPath(params string[] paths)
        {
            return PathUtils.Combine(PathUtils.PhysicalSiteServerPath, DirectoryName, PathUtils.Combine(paths));
        }

        public const string DirectoryIcons = "icons";
        public const string DirectoryFileSystem = "icons/filesystem";

        public static string GetFileSystemIconUrl(EFileSystemType fileSystemType, bool isLargeIcon)
        {
            string urlFormat =
                $"{DirectoryFileSystem}/{(isLargeIcon ? "large" : "small")}{(fileSystemType != EFileSystemType.Directory ? EFileSystemTypeUtils.GetValue(fileSystemType) : ".directory")}.gif";
            return GetUrl(urlFormat);
        }

        public static string GetIconUrl(string iconName)
        {
            return GetUrl(PageUtils.Combine(DirectoryIcons, iconName));
        }

        public class Default
        {
            public const string AccessMdb = "default/access.mdb";
        }

        public class Arrow
        {
            public const string First = "icons/arrow/arrow_first.gif";
            public const string FirstDisabled = "icons/arrow/arrow_first_d.gif";
            public const string Last = "icons/arrow/arrow_last.gif";
            public const string LastDisabled = "icons/arrow/arrow_last_d.gif";
            public const string Next = "icons/arrow/arrow_next.gif";
            public const string NextDisabled = "icons/arrow/arrow_next_d.gif";
            public const string Previous = "icons/arrow/arrow_prev.gif";
            public const string PreviousDisabled = "icons/arrow/arrow_prev_d.gif";
        }

        public class BaiRongFlash
        {
            public const string Js = "scripts/bairongflash.js";
        }

        public class SwfObject
        {
            public const string Js = "scripts/swfobject.js";
        }

        public class BrPlayer
        {
            public const string Swf = "flashes/brplayer/player.swf";
        }

        public class JwPlayer6
        {
            public const string Js = "flashes/jwplayer6/jwplayer.js";
        }

        public class FlowPlayer
        {
            public const string Js = "flashes/flowplayer/flowplayer-3.2.12.min.js";
            public const string Swf = "flashes/flowplayer/flowplayer-3.2.16.swf";
        }

        public class MediaElement
        {
            public const string Js = "flashes/mediaelement/mediaelement-and-player.min.js";
            public const string Css = "flashes/mediaelement/mediaelementplayer.min.css";
            public const string Swf = "flashes/mediaelement/flashmediaelement.swf";
        }

        public class AudioJs
        {
            public const string Js = "flashes/audiojs/audio.min.js";
        }

        public class VideoJs
        {
            public const string Css = "flashes/videojs/video-js.min.css";
            public const string Js = "flashes/videojs/video.min.js";
        }

        public class Stl
        {
            public const string JsPageScript = "scripts/stl/pagescript.js";
            public const string JsUserScript = "scripts/stl/userscript.js";
        }

        public class Static
        {
            public const string JsStaticAdFloating = "scripts/static/adFloating.js";
        }

        public class Bootstrap
        {
            public const string Css = "bootstrap/css/bootstrap.min.css";
            public const string Js = "bootstrap/js/bootstrap.min.js";
        }

        public class Toastr
        {
            public const string Js = "toastr/toastr.min.js";
            public const string Css = "toastr/toastr.min.css";
        }

        public class Layer
        {
            public const string Js = "layer/layer.min.js";
        }

        public class AjaxUpload
        {
            public const string Js = "ajaxUpload.js";
        }

        public class JQuery
        {
            public const string Js = "jquery/jquery-1.9.1.min.js";
        }

        public class GlobalScript
        {
            public const string Js = "scripts/global.v1.3.5.js";
            public const string Charset = "gb2312";
        }

        public class Json
        {
            public const string Js = "scripts/json.v1.1.js";
        }

        public class Validate
        {
            public const string Js = "scripts/independent/validate.js";
            public const string Charset = "utf-8";
        }

        public class DateString
        {
            public const string Js = "scripts/datestring.js";
            public const string Charset = "utf-8";
        }

        public class Lightbox
        {
            public const string Js = "scripts/lightbox/lightbox.js";
            public const string Css = "scripts/lightbox/lightbox.css";
        }

        public class Tabstrip
        {
            public const string Js = "scripts/tabstrip.js";
            public const string Css = "styles/tabstrip.css";
        }

        public class Global
        {
            public const string Css = "styles/global.v1.0.css";
        }

        public class Flashes
        {
            public const string Vcastr = "flashes/vcastr3.swf";
            public const string FocusViewer = "flashes/focusviewer.swf";
            public const string Bcastr = "flashes/bcastr31.swf";
            public const string Ali = "flashes/focusali.swf";
        }

        public class MmMenu
        {
            public const string Js = "scripts/mm_menu.js";
        }

        public class DatePicker
        {
            public const string Js = "datepicker/wdatepicker.js";

            public const string OnFocus = "WdatePicker({isShowClear:false,readOnly:true,dateFmt:'yyyy-MM-dd HH:mm:ss'});";
            public const string FormatString = "yyyy-MM-dd HH:mm:ss";

            public const string OnFocusDateOnly = "WdatePicker({isShowClear:false,readOnly:true,dateFmt:'yyyy-MM-dd'});";
            public const string FormatStringDateOnly = "yyyy-MM-dd";
        }

        public class Slide
        {
            public const string FullScreenSwf = "scripts/slide/fullscreen.swf";
            public const string Js = "scripts/slide/script.js";
            public const string Css = "scripts/slide/style.css";
            public const string Template = "scripts/slide/template.html";
        }

        public class Print
        {
            public const string JsUtf8 = "scripts/print_uft8.js";
            public const string JsGb2312 = "scripts/print.js";
            public const string IconUrl = "Icons/print";
        }

        public class TwCn
        {
            public const string Js = "scripts/independent/tw_cn.js";
            public const string Charset = "utf-8";
        }
    }
}
