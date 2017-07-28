using System;

namespace BaiRong.Core
{
    public class SiteFilesAssets
    {
        public const string FileLoading = "loading.gif";
        public const string FileS = "s.gif";
        public const string FileWaiting = "waiting.gif";

        public static string GetUrl(string apiUrl, string relatedUrl)
        {
            if (string.IsNullOrEmpty(apiUrl))
            {
                apiUrl = "/api";
            }
            apiUrl = apiUrl.Trim().ToLower();

            if (apiUrl == "/api")
            {
                apiUrl = "/";
            }
            else if (apiUrl.EndsWith("/api"))
            {
                apiUrl = apiUrl.Substring(0, apiUrl.LastIndexOf("/api", StringComparison.Ordinal));
            }
            else if (apiUrl.EndsWith("/api/"))
            {
                apiUrl = apiUrl.Substring(0, apiUrl.LastIndexOf("/api/", StringComparison.Ordinal));
            }
            if (string.IsNullOrEmpty(apiUrl))
            {
                apiUrl = "/";
            }
            return PageUtils.Combine(apiUrl, "sitefiles/assets", relatedUrl);
        }

        public static string GetPath(params string[] paths) => PathUtils.GetSiteFilesPath("assets", PathUtils.Combine(paths));

        public class CommentInput
        {
            public static string CommentsTemplatePath => GetPath("commentInput/template.html");

            public static string GetScriptUrl(string apiUrl) => GetUrl(apiUrl, "commentInput/script.js");

            public static string GetStyleUrl(string apiUrl) => GetUrl(apiUrl, "commentInput/style.css");
        }

        public class Digg
        {
            public static string GetScriptUrl(string apiUrl) => GetUrl(apiUrl, "digg/script.js");

            public static string GetStyleUrl(string apiUrl, string theme) => GetUrl(apiUrl, $"digg/{theme}.css");
        }

        public class Input
        {
            public static string GetScriptUrl(string apiUrl) => GetUrl(apiUrl, "input/script.js");
        }

        public class Resume
        {
            public static string GetScriptUrl(string apiUrl) => GetUrl(apiUrl, "resume/js/resume.js");

            public static string GetStyleUrl(string apiUrl) => GetUrl(apiUrl, "resume/css/resume.css");
        }

        public class Search
        {
            public static string LoadingTemplatePath => GetPath("search/loading.html");

            public static string YesTemplatePath => GetPath("search/yes.html");

            public static string NoTemplatePath => GetPath("search/no.html");
        }

        public class Star
        {
            public static string GetScriptUrl(string apiUrl) => GetUrl(apiUrl, "star/script.js");

            public static string GetStyleUrl(string apiUrl, string theme) => GetUrl(apiUrl, $"star/{theme}.css");
        }

        public class Vote
        {
            public static string GetStyleUrl(string apiUrl) => GetUrl(apiUrl, "vote/css/vote.css");
        }

        public class Tags
        {
            public static string GetStyleUrl(string apiUrl, string theme) => GetUrl(apiUrl, $"tags/{theme}.css");
        }

        public class Tracker
        {
            public static string DirectoryName = "tracker";
            public static string GetScriptUrl(string apiUrl) => GetUrl(apiUrl, "tracker/script.js");
        }

        public class GovPublicApply
        {
            public static string GetScriptUrl(string apiUrl) => GetUrl(apiUrl, "govpublicapply/js/apply.js");

            public static string GetStyleUrl(string apiUrl) => GetUrl(apiUrl, "govpublicapply/css/apply.css");
        }

        public class GovPublicQuery
        {
            public static string GetScriptUrl(string apiUrl) => GetUrl(apiUrl, "govpublicquery/js/query.js");

            public static string GetStyleUrl(string apiUrl) => GetUrl(apiUrl, "govpublicquery/css/query.css");
        }

        public class GovInteractApply
        {
            public static string GetStyleUrl(string apiUrl) => GetUrl(apiUrl, "govinteractapply/css/apply.css");
        }

        public class GovInteractQuery
        {
            public static string GetStyleUrl(string apiUrl) => GetUrl(apiUrl, "govinteractquery/css/query.css");
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

        public class Components
        {
            public const string Jquery = "components/jquery-1.9.1.min.js";
            public const string Lodash = "components/lodash-4.17.4.min.js";
            public const string Vue = "components/vue-2.1.10.min.js";
            public const string JsCookie = "components/js.cookie.js";
            public const string StlClient = "components/stlClient.js";
        }

        public class JQuery
        {
            public class FancyBox
            {
                public const string Js = "jquery/fancybox/jquery.fancybox-1.3.4.pack.js";
                public const string Css = "jquery/fancybox/jquery.fancybox-1.3.4.css";
            }

            public class AjaxUpload
            {
                public const string Js = "jquery/ajaxUpload.js";
            }

            public class QueryString
            {
                public const string Js = "jquery/queryString.js";
            }

            public class JQueryForm
            {
                public const string Js = "jquery/jquery.form.js";
            }

            public class ShowLoading
            {
                public const string Js = "jquery/showLoading/js/jquery.showLoading.min.js";
                public const string Css = "jquery/showLoading/css/showLoading.css";
                public const string Charset = "utf-8";
            }

            public class JTemplates
            {
                public const string Js = "jquery/jquery-jtemplates.js";
                public const string Charset = "utf-8";
            }

            public class ValidateJs
            {
                public const string Js = "jquery/validate.js";
                public const string Charset = "utf-8";
            }

            public class Bootstrap
            {
                public const string Css = "jquery/bootstrap/css/bootstrap.min.css";
                public const string Js = "jquery/bootstrap/js/bootstrap.min.js";
            }

            public class Highcharts
            {
                public const string HighchartsJs = "scripts/highcharts/js/highcharts.js";
                public const string ExportingJs = "scripts/highcharts/js/modules/exporting.js";
            }

            public class Toastr
            {
                public const string Js = "jquery/toastr/toastr.min.js";
                public const string Css = "jquery/toastr/toastr.min.css";
            }

            public class Layer
            {
                public const string Js = "jquery/layer/layer.min.js";
            }
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
            public const string Js = "scripts/datepicker/wdatepicker.js";

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
