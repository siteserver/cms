using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;

namespace SSCMS.Parse
{
    public class ParsePage
    {
        private readonly IPathManager _pathManager;

        public SortedDictionary<string, string> HeadCodes { get; private set; }

        public SortedDictionary<string, string> BodyCodes { get; private set; }

        public SortedDictionary<string, string> FootCodes { get; private set; }

        private Config Config { get; }

        public Site Site { get; private set; }

        public Dictionary<string, string> Parameters { get; set; }

        public Template Template { get; }

        public User User { get; set; }

        public int SiteId { get; private set; }

        public int PageChannelId { get; private set; }

        public int PageContentId { get; private set; }

        public bool IsLocal { get; set; }
        public EditMode EditMode { get; }
        public string IncludeFile { get; set; }
        public int EditableIndex { get; set; }
        public List<Editable> Editables { get; }

        public Stack<KeyValuePair<int, Channel>> ChannelItems { get; }

        public Stack<KeyValuePair<int, Content>> ContentItems { get; }

        public Stack<KeyValuePair<int, IDictionary<string, object>>> SqlItems { get; }

        public Stack<KeyValuePair<int, Site>> SiteItems { get;  }

        public Stack<KeyValuePair<int, object>> EachItems { get; }

        public Dictionary<string, object> PluginItems { get; }

        public ParsePage Clone()
        {
            return new ParsePage(_pathManager, EditMode, Config, PageChannelId, PageContentId, Site, Template, PluginItems)
            {
                HeadCodes = new SortedDictionary<string, string>(HeadCodes),
                BodyCodes = new SortedDictionary<string, string>(BodyCodes),
                FootCodes = new SortedDictionary<string, string>(FootCodes),
                IsLocal = IsLocal
            };
        }

        public ParsePage(IPathManager pathManager, EditMode editMode, Config config, int pageChannelId, int pageContentId, Site site, Template template, Dictionary<string, object> pluginItems)
        {
            _pathManager = pathManager;
            Template = template;
            SiteId = site.Id;
            PageChannelId = pageChannelId;
            PageContentId = pageContentId;
            IsLocal = false;
            EditMode = editMode;
            EditableIndex = 0;
            Editables = new List<Editable>();
            HeadCodes = new SortedDictionary<string, string>();
            BodyCodes = new SortedDictionary<string, string>();
            FootCodes = new SortedDictionary<string, string>();
            Config = config;
            Site = site;
            User = null;
            ChannelItems = new Stack<KeyValuePair<int, Channel>>(5);
            ContentItems = new Stack<KeyValuePair<int, Content>>(5);
            SqlItems = new Stack<KeyValuePair<int, IDictionary<string, object>>>(5);
            SiteItems = new Stack<KeyValuePair<int, Site>>(5);
            EachItems = new Stack<KeyValuePair<int, object>>(5);
            PluginItems = pluginItems;
        }

        public void ChangeSite(Site site, int pageChannelId, int pageContentId, ParseContext contextInfo)
        {
            SiteId = site.Id;
            Site = site;
            PageChannelId = pageChannelId;
            PageContentId = pageContentId;

            contextInfo.Site = site;
            contextInfo.ChannelId = pageChannelId;
            contextInfo.ContentId = pageContentId;
        }

        public async Task AddPageBodyCodeIfNotExistsAsync(string pageJsName)
        {
            if (!BodyCodes.ContainsKey(pageJsName))
            {
                BodyCodes.Add(pageJsName, await GetJsCodeAsync(pageJsName));
            }
        }

        public async Task AddPageAfterHtmlCodeIfNotExistsAsync(string pageJsName)
        {
            if (!FootCodes.ContainsKey(pageJsName))
            {
                FootCodes.Add(pageJsName, await GetJsCodeAsync(pageJsName));
            }
        }

        public async Task AddPageHeadCodeIfNotExistsAsync(string pageJsName)
        {
            if (!HeadCodes.ContainsKey(pageJsName))
            {
                HeadCodes.Add(pageJsName, await GetJsCodeAsync(pageJsName));
            }
        }

        public static class Const
        {
            public const string Jquery = "Jquery";
            public const string JsCookie = "JsCookie";
            public const string StlClient = "StlClient";

            public const string BAjaxUpload = "B_AjaxUpload";              //AjaxUpload
            public const string BQueryString = "B_QueryString";            //QueryString
            public const string BjQueryForm = "B_JQueryForm";              //JQueryForm
            public const string BShowLoading = "B_ShowLoading";              //ShowLoading
            public const string BjTemplates = "B_JTemplates";              //JTemplates
            public const string BValidate = "B_Validate";                  //Validate
            public const string BBootstrap = "B_Bootstrap";                  //Bootstrap

            public const string JsAcSwfObject = "Js_Ac_SWFObject";                //SWFObject

            public const string JsAcJwPlayer6 = "Js_Ac_JWPlayer6";                //JWPlayer6
            public const string JsAcFlowPlayer = "Js_Ac_FlowPlayer";              //flowPlayer
            public const string JsAcMediaElement = "Js_Ac_MediaElement";          //mediaelement
            public const string JsAcAudioJs = "Js_Ac_AudioJs";                    //audio.js
            public const string JsAcVideoJs = "Js_Ac_VideoJs";                    //video.js

            public const string JsAeStlZoom = "Js_Ae_StlZoom";                    //文字缩放
            public const string JsAfStlPrinter = "Js_Af_StlPrinter";              //打印
            public const string JsAhTranslate = "Js_Ah_Translate";                //繁体/简体转换

            public const string JsPageOpenWindow = "Js_Page_OpenWindow";
            public const string JsUserScript = "Js_User_Script";
            public const string JsInnerCalendar = "Js_Inner_Calendar";

            public const string Vue = nameof(Vue);
            public const string VueElement = nameof(VueElement);
            public const string Layer = nameof(Layer);
            public const string PdfObject = nameof(PdfObject);
        }

        private async Task<string> GetJsCodeAsync(string pageJsName)
        {
            var retVal = string.Empty;

            if (pageJsName == Const.Jquery)
            {
                if (Site.IsCreateWithJQuery)
                {
                    var jsUrl = _pathManager.GetSiteFilesUrl(Site, Libraries.JqueryJs);

                    retVal =
                        $@"<script src=""{jsUrl}"" type=""text/javascript""></script>";
                }
            }
            else if (pageJsName == Const.JsCookie)
            {
                var jsUrl = _pathManager.GetSiteFilesUrl(Site, Resources.Components.JsCookie);

                retVal =
                    $@"<script src=""{jsUrl}"" type=""text/javascript""></script>";
            }
            else if (pageJsName == Const.StlClient)
            {
                var lodashUrl = _pathManager.GetSiteFilesUrl(Site, Resources.Components.Lodash);
                var clientUrl = _pathManager.GetSiteFilesUrl(Site, Resources.Components.StlClient);

                retVal =
                    $@"<script src=""{lodashUrl}"" type=""text/javascript""></script><script src=""{clientUrl}"" type=""text/javascript""></script>";
            }
            else if (pageJsName == Const.BAjaxUpload)
            {
                var jsUrl = _pathManager.GetSiteFilesUrl(Site, Resources.JQuery.AjaxUpload.Js);

                retVal =
                    $@"<script src=""{jsUrl}"" type=""text/javascript""></script>";
            }
            else if (pageJsName == Const.BQueryString)
            {
                var jsUrl = _pathManager.GetSiteFilesUrl(Site, Resources.JQuery.QueryString.Js);

                retVal =
                    $@"<script src=""{jsUrl}"" type=""text/javascript""></script>";
            }
            else if (pageJsName == Const.BjQueryForm)
            {
                var jsUrl = _pathManager.GetSiteFilesUrl(Site, Resources.JQuery.JQueryForm.Js);

                retVal =
                    $@"<script src=""{jsUrl}"" type=""text/javascript""></script>";
            }
            else if (pageJsName == Const.BShowLoading)
            {
                var cssUrl = _pathManager.GetSiteFilesUrl(Site, Resources.JQuery.ShowLoading.Css);
                var jsUrl = _pathManager.GetSiteFilesUrl(Site, Resources.JQuery.ShowLoading.Js);

                retVal =
                    $@"<link href=""{cssUrl}"" rel=""stylesheet"" media=""screen"" /><script type=""text/javascript"" charset=""{Resources
                        .JQuery.ShowLoading.Charset}"" src=""{jsUrl}""></script>";
            }
            else if (pageJsName == Const.BjTemplates)
            {
                var jsUrl =
                    _pathManager.GetSiteFilesUrl(Site, Resources.JQuery.JTemplates.Js);

                retVal =
                    $@"<script type=""text/javascript"" charset=""{Resources.JQuery.JTemplates.Charset}"" src=""{jsUrl}""></script>";
            }
            else if (pageJsName == Const.BValidate)
            {
                var jsUrl =
                    _pathManager.GetSiteFilesUrl(Site, Resources.JQuery.ValidateJs.Js);

                retVal =
                    $@"<script type=""text/javascript"" charset=""{Resources.JQuery.ValidateJs.Charset}"" src=""{jsUrl}""></script>";
            }
            else if (pageJsName == Const.BBootstrap)
            {
                var cssUrl = _pathManager.GetSiteFilesUrl(Site, Resources.JQuery.Bootstrap.Css);
                var jsUrl = _pathManager.GetSiteFilesUrl(Site, Resources.JQuery.Bootstrap.Js);

                retVal = $@"
<link rel=""stylesheet"" type=""text/css"" href=""{cssUrl}"">
<script language=""javascript"" src=""{jsUrl}""></script>
";
            }
            else if (pageJsName == Const.JsAcSwfObject)
            {
                var jsUrl = _pathManager.GetSiteFilesUrl(Site, Resources.SwfObject.Js);

                retVal =
                    $@"<script type=""text/javascript"" src=""{jsUrl}""></script>";
            }
            else if (pageJsName == Const.JsAcJwPlayer6)
            {
                var jsUrl = _pathManager.GetSiteFilesUrl(Site, Resources.JwPlayer6.Js);

                retVal =
                    $@"<script type=""text/javascript"" src=""{jsUrl}""></script><script type=""text/javascript"">jwplayer.key=""ABCDEFGHIJKLMOPQ"";</script>";
            }
            else if (pageJsName == Const.JsAcFlowPlayer)
            {
                var jsUrl = _pathManager.GetSiteFilesUrl(Site, Resources.FlowPlayer.Js);

                retVal =
                    $@"<script type=""text/javascript"" src=""{jsUrl}""></script>";
            }
            else if (pageJsName == Const.JsAcMediaElement)
            {
                var jsUrl = _pathManager.GetSiteFilesUrl(Site, Resources.MediaElement.Js);
                var cssUrl = _pathManager.GetSiteFilesUrl(Site, Resources.MediaElement.Css);

                retVal =
                    $@"<script type=""text/javascript"" src=""{jsUrl}""></script><link rel=""stylesheet"" href=""{cssUrl}"" />";
            }
            else if (pageJsName == Const.JsAcAudioJs)
            {
                var jsUrl = _pathManager.GetSiteFilesUrl(Site, Resources.AudioJs.Js);

                retVal =
                    $@"<script type=""text/javascript"" src=""{jsUrl}""></script>
<script type='text/javascript'>
audiojs.events.ready(function() {{
    audiojs.createAll();
}});
</script>
";
            }
            else if (pageJsName == Const.JsAcVideoJs)
            {
                var cssUrl = _pathManager.GetSiteFilesUrl(Site, Resources.VideoJs.Css);
                var jsUrl = _pathManager.GetSiteFilesUrl(Site, Resources.VideoJs.Js);

                retVal = $@"
<link href=""{cssUrl}"" rel=""stylesheet"">
<script type=""text/javascript"" src=""{jsUrl}""></script>
";
            }
            else if (pageJsName == Const.JsPageOpenWindow)
            {
                retVal = @"
<div id=""stl_wnd_board"" style=""position:absolute;top:100px;left:0px;width:100%;z-index:65531;height:100%;display:none"" align=""center"">
    <div id=""stl_wnd_div"" style=""display:none; width:400px; height:330px; padding:0; margin:0px;"" align=""center"">
    <iframe id=""stl_wnd_frame"" frameborder=""0"" scrolling=""auto"" width=""100%"" height=""100%"" src=""""></iframe>
    </div>
</div>
<script>
function stlCloseWindow()
{document.getElementById('stl_wnd_div').style.display='none';document.getElementById('stl_wnd_board').style.display='none';}
function stlOpenWindow(pageUrl,width,height)
{var stl_wnd=document.getElementById('stl_wnd_div');var stl_board=document.getElementById('stl_wnd_board');var wnd_frame=document.getElementById('stl_wnd_frame');if(stl_wnd){stl_wnd.style.width=width+'px';stl_wnd.style.height=height+'px';stl_board.style.display='block';stl_board.style.top=(100+document.documentElement.scrollTop)+'px';stl_wnd.style.visible='hidden';stl_wnd.style.display='block';var url;if(pageUrl.indexOf('?')==-1){url=pageUrl+'?_r='+Math.random();}else{url=pageUrl+'&_r='+Math.random();}
wnd_frame.src=url;}}
</script>
";
            }
            else if (pageJsName == Const.JsUserScript)
            {
                var jsPageScriptUrl = _pathManager.GetSiteFilesUrl(Site, Resources.Stl.JsPageScript);
                var assetsUrl = _pathManager.GetSiteFilesUrl(Site);
                var jsUserScriptUrl = _pathManager.GetSiteFilesUrl(Site, Resources.Stl.JsUserScript);

                retVal = $@"
<script type=""text/javascript"" src=""{jsPageScriptUrl}""></script>
<script type=""text/javascript"">stlInit('{assetsUrl}', '{Site.Id}', {(await _pathManager.GetWebUrlAsync(Site)).TrimEnd('/')}');</script>
<script type=""text/javascript"" src=""{jsUserScriptUrl}""></script>";
            }
            else if (pageJsName == Const.JsInnerCalendar)
            {
                var jsUrl = _pathManager.GetSiteFilesUrl(Site, Resources.DatePicker.Js);

                retVal = $@"<script type=""text/javascript"" src=""{jsUrl}""></script>";
            }
            else if (pageJsName == Const.JsAhTranslate)
            {
                var jsUrl = _pathManager.GetSiteFilesUrl(Site, Resources.TwCn.Js);

                retVal =
                    $@"<script src=""{jsUrl}"" charset=""{Resources.TwCn.Charset}"" type=""text/javascript""></script>";
            }
            else if (pageJsName == Const.Vue)
            {
                var jsUrl = _pathManager.GetSiteFilesUrl(Site, Libraries.VueJs);

                retVal =
                    $@"<script src=""{jsUrl}"" type=""text/javascript""></script>";
            }
            else if (pageJsName == Const.VueElement)
            {
                var cssUrl = _pathManager.GetSiteFilesUrl(Site, Libraries.ElementCss);
                var jsUrl = _pathManager.GetSiteFilesUrl(Site, Libraries.ElementJs);

                retVal =
                    $@"<link href=""{cssUrl}"" rel=""stylesheet"" /><script type=""text/javascript"" src=""{jsUrl}""></script>";
            }
            else if (pageJsName == Const.Layer)
            {
                var jsUrl = _pathManager.GetSiteFilesUrl(Site, Libraries.LayerJs);

                retVal =
                    $@"<script src=""{jsUrl}"" type=""text/javascript""></script>";
            }
            else if (pageJsName == Const.PdfObject)
            {
                var jsUrl = _pathManager.GetSiteFilesUrl(Site, Libraries.PdfObjectJs);
                retVal =
                    $@"<script src=""{jsUrl}"" type=""text/javascript""></script>";
            }

            return retVal;
        }

        public string HeadCodesHtml
        {
            get
            {
                var builder = new StringBuilder();

                //builder.Append(
                //$@"<script>var $pageInfo = {{siteId : {SiteId}, channelId : {PageChannelId}, contentId : {PageContentId}, siteUrl : ""{Site.WebUrl.TrimEnd('/')}"", rootUrl : ""{PageUtils.GetRootUrl(string.Empty).TrimEnd('/')}"", apiUrl : ""{ApiUrl.TrimEnd('/')}""}};</script>").AppendLine();

                foreach (var key in HeadCodes.Keys)
                {
                    builder.Append(HeadCodes[key]);
                }
                return builder.ToString();
            }
        }

        public string BodyCodesHtml
        {
            get
            {
                var builder = new StringBuilder();
                foreach (var key in BodyCodes.Keys)
                {
                    builder.Append(BodyCodes[key]);
                }
                return builder.ToString();
            }   
        }

        public string FootCodesHtml
        {
            get
            {
                var builder = new StringBuilder();
                foreach (var key in FootCodes.Keys)
                {
                    builder.Append(FootCodes[key]);
                }
                return builder.ToString();
            }
        }
    }
}
