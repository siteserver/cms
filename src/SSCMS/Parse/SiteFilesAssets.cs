using System;

namespace SSCMS.Parse
{
    public static class SiteFilesAssets
    {
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

            return $"{apiUrl}/sitefiles/assets/{relatedUrl}";// PageUtils.Combine(apiUrl, "sitefiles/assets", relatedUrl);
        }

        //public static string GetPath(params string[] paths) => WebUtils.GetSiteFilesPath("assets", PathUtils.Combine(paths));

        public static string GetPath(params string[] paths) => string.Empty;

        public static class Search
        {
            public static string LoadingTemplatePath => GetPath("search/loading.html");

            public static string YesTemplatePath => GetPath("search/yes.html");

            public static string NoTemplatePath => GetPath("search/no.html");
        }

        public static class BaiRongFlash
        {
            public const string Js = "scripts/bairongflash.js";
        }

        public static class SwfObject
        {
            public const string Js = "scripts/swfobject.js";
        }

        public static class BrPlayer
        {
            public const string Swf = "flashes/brplayer/player.swf";
        }

        public static class JwPlayer6
        {
            public const string Js = "flashes/jwplayer6/jwplayer.js";
        }

        public static class FlowPlayer
        {
            public const string Js = "flashes/flowplayer/flowplayer-3.2.12.min.js";
            public const string Swf = "flashes/flowplayer/flowplayer-3.2.16.swf";
        }

        public static class MediaElement
        {
            public const string Js = "flashes/mediaelement/mediaelement-and-player.min.js";
            public const string Css = "flashes/mediaelement/mediaelementplayer.min.css";
            public const string Swf = "flashes/mediaelement/mediaelement-flash-audio.swf";
        }

        public static class AudioJs
        {
            public const string Js = "flashes/audiojs/audio.min.js";
        }

        public static class VideoJs
        {
            public const string Css = "flashes/videojs/video-js.min.css";
            public const string Js = "flashes/videojs/video.min.js";
        }

        public static class Stl
        {
            public const string JsPageScript = "scripts/stl/pagescript.js";
            public const string JsUserScript = "scripts/stl/userscript.js";
        }

        public static class Static
        {
            public const string JsStaticAdFloating = "scripts/static/adFloating.js";
        }

        public static class Components
        {
            public const string Jquery = "components/jquery-1.9.1.min.js";
            public const string Lodash = "components/lodash-4.17.4.min.js";
            public const string JsCookie = "components/js.cookie.js";
            public const string StlClient = "components/stlClient.js";
        }

        public static class VueJs
        {
            public const string Vue = "vuejs/vue-2.5.16.min.js";
            public const string ElementCss = "vuejs/element-ui/theme-chalk/index.css";
            public const string ElementJs = "vuejs/element-ui/index.js";
        }

        public static class JQuery
        {
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
        }

        public static class DateString
        {
            public const string Js = "scripts/datestring.js";
            public const string Charset = "utf-8";
        }

        public static class Flashes
        {
            public const string Vcastr = "flashes/vcastr3.swf";
            public const string FocusViewer = "flashes/focusviewer.swf";
            public const string Bcastr = "flashes/bcastr31.swf";
            public const string Ali = "flashes/focusali.swf";
        }

        public static class DatePicker
        {
            public const string Js = "scripts/datepicker/wdatepicker.js";
        }

        public static class Print
        {
            public const string Js = "scripts/print_uft8.js";
            public const string IconUrl = "Icons/print";
        }

        public static class TwCn
        {
            public const string Js = "scripts/independent/tw_cn.js";
            public const string Charset = "utf-8";
        }
    }
}
