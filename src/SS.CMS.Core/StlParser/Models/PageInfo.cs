using System.Collections.Generic;
using System.Text;
using SS.CMS.Core.Common;
using SS.CMS.Models;
using SS.CMS.Services;

namespace SS.CMS.Core.StlParser.Models
{
    public class PageInfo
    {
        public SortedDictionary<string, string> HeadCodes { get; }
        public SortedDictionary<string, string> BodyCodes { get; }
        public SortedDictionary<string, string> FootCodes { get; }
        public Site SiteInfo { get; private set; }
        public Dictionary<string, string> Parameters { get; set; }
        public Template TemplateInfo { get; }
        public User UserInfo { get; set; }
        public int SiteId { get; private set; }
        public int PageChannelId { get; private set; }
        public int PageContentId { get; private set; }
        public bool IsLocal { get; set; }
        public Stack<KeyValuePair<int, Site>> SiteItems { get; }
        public Stack<KeyValuePair<int, Channel>> ChannelItems { get; }
        public Stack<KeyValuePair<int, Content>> ContentItems { get; }
        public Stack<KeyValuePair<int, object>> EachItems { get; }
        public Stack<KeyValuePair<int, Dictionary<string, object>>> SqlItems { get; }
        public Dictionary<string, object> PluginItems { get; }
        private int _uniqueId;

        public int UniqueId
        {
            get => _uniqueId++;
            set => _uniqueId = value;
        }

        public PageInfo Clone()
        {
            return new PageInfo(PageChannelId, PageContentId, SiteInfo, TemplateInfo, PluginItems);
        }

        public PageInfo(int pageChannelId, int pageContentId, Site siteInfo, Template templateInfo, Dictionary<string, object> pluginItems)
        {
            TemplateInfo = templateInfo;
            SiteId = siteInfo.Id;
            PageChannelId = pageChannelId;
            PageContentId = pageContentId;
            IsLocal = false;
            HeadCodes = new SortedDictionary<string, string>();
            BodyCodes = new SortedDictionary<string, string>();
            FootCodes = new SortedDictionary<string, string>();
            SiteInfo = siteInfo;
            UserInfo = null;
            _uniqueId = 1;

            SiteItems = new Stack<KeyValuePair<int, Site>>(5);
            ChannelItems = new Stack<KeyValuePair<int, Channel>>(5);
            ContentItems = new Stack<KeyValuePair<int, Content>>(5);
            EachItems = new Stack<KeyValuePair<int, object>>(5);
            SqlItems = new Stack<KeyValuePair<int, Dictionary<string, object>>>(5);
            PluginItems = pluginItems;
        }

        public void ChangeSite(Site siteInfo, int pageChannelId, int pageContentId, ParseContext parseContext)
        {
            SiteId = siteInfo.Id;
            SiteInfo = siteInfo;
            PageChannelId = pageChannelId;
            PageContentId = pageContentId;

            parseContext.ChannelId = pageChannelId;
            parseContext.ContentId = pageContentId;
        }

        public void AddPageBodyCodeIfNotExists(IUrlManager urlManager, string pageJsName)
        {
            if (!BodyCodes.ContainsKey(pageJsName))
            {
                BodyCodes.Add(pageJsName, GetJsCode(urlManager, pageJsName));
            }
        }

        public void AddPageAfterHtmlCodeIfNotExists(IUrlManager urlManager, string pageJsName)
        {
            if (!FootCodes.ContainsKey(pageJsName))
            {
                FootCodes.Add(pageJsName, GetJsCode(urlManager, pageJsName));
            }
        }

        public void AddPageHeadCodeIfNotExists(IUrlManager urlManager, string pageJsName)
        {
            if (!HeadCodes.ContainsKey(pageJsName))
            {
                HeadCodes.Add(pageJsName, GetJsCode(urlManager, pageJsName));
            }
        }

        // public void AddLastPageScript(PageInfo lastPageInfo)
        // {
        //     foreach (var key in lastPageInfo.BodyCodes.Keys)
        //     {
        //         if (!BodyCodes.ContainsKey(key))
        //         {
        //             BodyCodes.Add(key, lastPageInfo.BodyCodes[key]);
        //         }
        //     }
        //     foreach (var key in lastPageInfo.FootCodes.Keys)
        //     {
        //         if (!FootCodes.ContainsKey(key))
        //         {
        //             FootCodes.Add(key, lastPageInfo.FootCodes[key]);
        //         }
        //     }
        //     foreach (var key in lastPageInfo.HeadCodes.Keys)
        //     {
        //         if (!HeadCodes.ContainsKey(key))
        //         {
        //             HeadCodes.Add(key, lastPageInfo.HeadCodes[key]);
        //         }
        //     }
        // }

        // public void ClearLastPageScript(PageInfo lastPageInfo)
        // {
        //     foreach (var key in lastPageInfo.BodyCodes.Keys)
        //     {
        //         BodyCodes.Remove(key);
        //     }
        //     foreach (var key in lastPageInfo.FootCodes.Keys)
        //     {
        //         FootCodes.Remove(key);
        //     }
        //     foreach (var key in lastPageInfo.HeadCodes.Keys)
        //     {
        //         HeadCodes.Remove(key);
        //     }
        // }

        // /// <summary>
        // /// 清理本页面的js
        // /// </summary>
        // public void ClearLastPageScript()
        // {
        //     HeadCodes.Clear();
        //     BodyCodes.Clear();
        //     FootCodes.Clear();
        // }

        public class Const
        {
            public const string Jquery = "Jquery";
            public const string Vue = "Vue";
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
            public const string JsAgStlTreeNotAjax = "Js_Ag_StlTreeNotAjax";                    //树状导航
            public const string JsAgStlTreeAjax = "Js_Ag_StlTreeAjax";                    //树状导航
            public const string JsAhTranslate = "Js_Ah_Translate";                //繁体/简体转换

            public const string JsPageOpenWindow = "Js_Page_OpenWindow";
            public const string JsUserScript = "Js_User_Script";
            public const string JsInnerCalendar = "Js_Inner_Calendar";

            public const string JsStaticAdFloating = "Js_Static_AdFloating";      //漂浮广告
        }

        private string GetJsCode(IUrlManager urlManager, string pageJsName)
        {
            var retval = string.Empty;

            if (pageJsName == Const.Jquery)
            {
                if (SiteInfo.IsCreateWithJQuery)
                {
                    retval =
                        $@"<script src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.Components.Jquery)}"" type=""text/javascript""></script>";
                }
            }
            else if (pageJsName == Const.Vue)
            {
                retval =
                    $@"<script src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.Components.Vue)}"" type=""text/javascript""></script>";
            }
            else if (pageJsName == Const.JsCookie)
            {
                retval =
                    $@"<script src=""{ SiteFilesAssets.GetUrl(SiteFilesAssets.Components.JsCookie)}"" type=""text/javascript""></script>";
            }
            else if (pageJsName == Const.StlClient)
            {
                retval =
                    $@"<script src=""{ SiteFilesAssets.GetUrl(SiteFilesAssets.Components.Lodash)}"" type=""text/javascript""></script><script src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.Components.StlClient)}"" type=""text/javascript""></script>";
            }
            else if (pageJsName == Const.BAjaxUpload)
            {
                retval =
                    $@"<script src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.JQuery.AjaxUpload.Js)}"" type=""text/javascript""></script>";
            }
            else if (pageJsName == Const.BQueryString)
            {
                retval =
                    $@"<script src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.JQuery.QueryString.Js)}"" type=""text/javascript""></script>";
            }
            else if (pageJsName == Const.BjQueryForm)
            {
                retval =
                    $@"<script src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.JQuery.JQueryForm.Js)}"" type=""text/javascript""></script>";
            }
            else if (pageJsName == Const.BShowLoading)
            {
                retval =
                    $@"<link href=""{SiteFilesAssets.GetUrl(SiteFilesAssets.JQuery.ShowLoading.Css)}"" rel=""stylesheet"" media=""screen"" /><script type=""text/javascript"" charset=""{SiteFilesAssets
                        .JQuery.ShowLoading.Charset}"" src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.JQuery.ShowLoading.Js)}""></script>";
            }
            else if (pageJsName == Const.BjTemplates)
            {
                retval =
                    $@"<script type=""text/javascript"" charset=""{SiteFilesAssets.JQuery.JTemplates.Charset}"" src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.JQuery.JTemplates.Js)}""></script>";
            }
            else if (pageJsName == Const.BValidate)
            {
                retval =
                    $@"<script type=""text/javascript"" charset=""{SiteFilesAssets.JQuery.ValidateJs.Charset}"" src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.JQuery.ValidateJs.Js)}""></script>";
            }
            else if (pageJsName == Const.BBootstrap)
            {
                var cssUrl = SiteFilesAssets.GetUrl(SiteFilesAssets.JQuery.Bootstrap.Css);
                var jsUrl = SiteFilesAssets.GetUrl(SiteFilesAssets.JQuery.Bootstrap.Js);
                retval = $@"
<link rel=""stylesheet"" type=""text/css"" href=""{cssUrl}"">
<script language=""javascript"" src=""{jsUrl}""></script>
";
            }
            else if (pageJsName == Const.JsAcSwfObject)
            {
                retval =
                    $@"<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.SwfObject.Js)}""></script>";
            }
            else if (pageJsName == Const.JsAcJwPlayer6)
            {
                retval =
                    $@"<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.JwPlayer6.Js)}""></script><script type=""text/javascript"">jwplayer.key=""ABCDEFGHIJKLMOPQ"";</script>";
            }
            else if (pageJsName == Const.JsAcFlowPlayer)
            {
                retval =
                    $@"<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.FlowPlayer.Js)}""></script>";
            }
            else if (pageJsName == Const.JsAcMediaElement)
            {
                retval =
                    $@"<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.MediaElement.Js)}""></script><link rel=""stylesheet"" href=""{SiteFilesAssets.GetUrl(SiteFilesAssets.MediaElement.Css)}"" />";
            }
            else if (pageJsName == Const.JsAcAudioJs)
            {
                retval =
                    $@"<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.AudioJs.Js)}""></script>
<script type='text/javascript'>
audiojs.events.ready(function() {{
    audiojs.createAll();
}});
</script>
";
            }
            else if (pageJsName == Const.JsAcVideoJs)
            {
                retval = $@"
<link href=""{SiteFilesAssets.GetUrl(SiteFilesAssets.VideoJs.Css)}"" rel=""stylesheet"">
<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.VideoJs.Js)}""></script>
";
            }
            else if (pageJsName == Const.JsPageOpenWindow)
            {
                retval = @"
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
                retval = $@"
<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.Stl.JsPageScript)}""></script>
<script type=""text/javascript"">stlInit('{SiteFilesAssets.GetUrl(string.Empty)}', '{SiteInfo.Id}', {urlManager.GetWebUrl(SiteInfo).TrimEnd('/')}');</script>
<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.Stl.JsUserScript)}""></script>";
            }
            else if (pageJsName == Const.JsInnerCalendar)
            {
                retval = $@"<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.DatePicker.Js)}""></script>";
            }
            else if (pageJsName == Const.JsStaticAdFloating)
            {
                retval =
                    $@"<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.Static.JsStaticAdFloating)}""></script>";
            }
            else if (pageJsName == Const.JsAhTranslate)
            {
                retval =
                    $@"<script src=""{SiteFilesAssets.GetUrl(SiteFilesAssets.TwCn.Js)}"" charset=""{SiteFilesAssets.TwCn.Charset}"" type=""text/javascript""></script>";
            }
            return retval;
        }

        public string HeadCodesHtml
        {
            get
            {
                var builder = new StringBuilder();

                //builder.Append(
                //$@"<script>var $pageInfo = {{siteId : {SiteId}, channelId : {PageChannelId}, contentId : {PageContentId}, siteUrl : ""{SiteInfo.Additional.WebUrl.TrimEnd('/')}"", rootUrl : ""{PageUtils.GetRootUrl(string.Empty).TrimEnd('/')}"", apiUrl : ""{ApiUrl.TrimEnd('/')}""}};</script>").AppendLine();

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
