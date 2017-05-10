using System.Collections;
using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Model
{
    public class PageInfo
    {
        private readonly SortedDictionary<string, string> _pageHeadScripts;
        private readonly SortedDictionary<string, string> _pageAfterBodyScripts;
        private readonly SortedDictionary<string, string> _pageBeforeBodyScripts;
        private readonly SortedDictionary<string, string> _pageEndScripts;
        
        public PublishmentSystemInfo PublishmentSystemInfo { get; private set; }

        public string ApiUrl => PublishmentSystemInfo.Additional.ApiUrl;

        public string HomeUrl => PublishmentSystemInfo.Additional.HomeUrl;

        private int _uniqueId;

        public int UniqueId => _uniqueId++;

        public void SetUniqueId(int uniqueId)
        {
            _uniqueId = uniqueId;
        }

        public Stack ChannelItems { get; }

        public Stack ContentItems { get; }

        public Stack CommentItems { get; }

        public Stack InputItems { get; }

        public Stack SqlItems { get; }

        public Stack SiteItems { get; }

        public Stack PhotoItems { get; }

        public Stack EachItems { get; }

        public class JQuery
        {
            public const string BAjaxUpload = "B_AjaxUpload";              //AjaxUpload
            public const string BQueryString = "B_QueryString";            //QueryString
            public const string BjQueryForm = "B_JQueryForm";              //JQueryForm
            public const string BShowLoading = "B_ShowLoading";              //ShowLoading
            public const string BjTemplates = "B_JTemplates";              //JTemplates
            public const string BValidate = "B_Validate";                  //Validate
            public const string BBootstrap = "B_Bootstrap";                  //Bootstrap
        }

        public const string JsAcSwfObject = "Js_Ac_SWFObject";                //SWFObject

        public const string JsAcJwPlayer6 = "Js_Ac_JWPlayer6";                //JWPlayer6
        public const string JsAcFlowPlayer = "Js_Ac_FlowPlayer";              //flowPlayer
        public const string JsAcMediaElement = "Js_Ac_MediaElement";          //mediaelement
        public const string JsAcAudioJs = "Js_Ac_AudioJs";                    //audio.js
        public const string JsAcVideoJs = "Js_Ac_VideoJs";                    //video.js

        public const string JsAcMenuScripts = "Js_Ac_MenuScripts";            //下拉菜单
        public const string JsAdStlCountHits = "Js_Ad_StlCountHits";          //统计访问量
        public const string JsAdAddTracker = "Js_Ad_AddTracker";              //添加流量统计代码
        public const string JsAeStlZoom = "Js_Ae_StlZoom";                    //文字缩放
        public const string JsAfStlPrinter = "Js_Af_StlPrinter";              //打印
        public const string JsAgStlTreeNotAjax = "Js_Ag_StlTreeNotAjax";                    //树状导航
        public const string JsAgStlTreeAjax = "Js_Ag_StlTreeAjax";                    //树状导航
        public const string JsAhTranslate = "Js_Ah_Translate";                //繁体/简体转换

        public const string JsPageOpenWindow = "Js_Page_OpenWindow";
        public const string JsUserScript = "Js_User_Script";
        public const string JsInnerCalendar = "Js_Inner_Calendar";

        public const string JsStaticAdFloating = "Js_Static_AdFloating";      //漂浮广告

        public class Components
        {
            public const string Jquery = "Jquery";
            public const string Vue = "Vue";
            public const string JsCookie = "JsCookie";
            public const string StlClient = "StlClient";
        }

        public PageInfo(int pageNodeId, int pageContentId, PublishmentSystemInfo publishmentSystemInfo, TemplateInfo templateInfo, UserInfo userInfo)
        {
            TemplateInfo = templateInfo;
            PublishmentSystemId = publishmentSystemInfo.PublishmentSystemId;
            PageNodeId = pageNodeId;
            PageContentId = pageContentId;
            _pageAfterBodyScripts = new SortedDictionary<string, string>();
            _pageBeforeBodyScripts = new SortedDictionary<string, string>();
            _pageEndScripts = new SortedDictionary<string, string>();
            _pageHeadScripts = new SortedDictionary<string, string>();
            PublishmentSystemInfo = publishmentSystemInfo;
            UserInfo = userInfo;
            _uniqueId = 1;

            ChannelItems = new Stack(5);
            ContentItems = new Stack(5);
            CommentItems = new Stack(5);
            InputItems = new Stack(5);
            SqlItems = new Stack(5);
            SiteItems = new Stack(5);
            PhotoItems = new Stack(5);
            EachItems = new Stack(5);
        }

        public TemplateInfo TemplateInfo { get; }

        public UserInfo UserInfo { get; }

        public int PublishmentSystemId { get; private set; }

        public int PageNodeId { get; private set; }

        public int PageContentId { get; private set; }

        public void ChangeSite(PublishmentSystemInfo publishmentSystemInfo, int pageNodeId, int pageContentId, ContextInfo contextInfo)
        {
            PublishmentSystemId = publishmentSystemInfo.PublishmentSystemId;
            PublishmentSystemInfo = publishmentSystemInfo;
            PageNodeId = pageNodeId;
            PageContentId = pageContentId;

            contextInfo.PublishmentSystemInfo = publishmentSystemInfo;
            contextInfo.ChannelId = pageNodeId;
            contextInfo.ContentId = pageContentId;
        }

        private string GetJsCode(string pageJsName)
        {
            var retval = string.Empty;

            if (pageJsName == Components.Jquery)
            {
                if (PublishmentSystemInfo.Additional.IsCreateWithJQuery)
                {
                    retval =
                        $"<script type=\"text/javascript\">!window.jQuery&&document.write('<script src=\"{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.Components.Jquery)}\" language=\"javascript\"></'+'script>');</script>";
                }
            }
            else if (pageJsName == Components.Vue)
            {
                retval =
                    $"<script src=\"{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.Components.Vue)}\" type=\"text/javascript\"></script>";
            }
            else if (pageJsName == Components.JsCookie)
            {
                retval =
                    $@"<script src=""{ SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.Components.JsCookie)}"" type=""text/javascript""></script>";
            }
            else if (pageJsName == Components.StlClient)
            {
                retval =
                    $@"<script src=""{ SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.Components.Lodash)}"" type=""text/javascript""></script><script src=""{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.Components.StlClient)}"" type=""text/javascript""></script>";
            }
            else if (pageJsName == JQuery.BAjaxUpload)
            {
                retval =
                    $"<script src=\"{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.JQuery.AjaxUpload.Js)}\" type=\"text/javascript\"></script>";
            }
            else if (pageJsName == JQuery.BQueryString)
            {
                retval =
                    $"<script src=\"{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.JQuery.QueryString.Js)}\" type=\"text/javascript\"></script>";
            }
            else if (pageJsName == JQuery.BjQueryForm)
            {
                retval =
                    $"<script src=\"{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.JQuery.JQueryForm.Js)}\" type=\"text/javascript\"></script>";
            }
            else if (pageJsName == JQuery.BShowLoading)
            {
                retval =
                    $@"<link href=""{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.JQuery.ShowLoading.Css)}"" rel=""stylesheet"" media=""screen"" /><script type=""text/javascript"" charset=""{SiteFilesAssets
                        .JQuery.ShowLoading.Charset}"" src=""{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.JQuery.ShowLoading.Js)}""></script>";
            }
            else if (pageJsName == JQuery.BjTemplates)
            {
                retval =
                    $@"<script type=""text/javascript"" charset=""{SiteFilesAssets.JQuery.JTemplates.Charset}"" src=""{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.JQuery.JTemplates.Js)}""></script>";
            }
            else if (pageJsName == JQuery.BValidate)
            {
                retval =
                    $@"<script type=""text/javascript"" charset=""{SiteFilesAssets.JQuery.ValidateJs.Charset}"" src=""{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.JQuery.ValidateJs.Js)}""></script>";
            }
            else if (pageJsName == JQuery.BBootstrap)
            {
                var cssUrl = SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.JQuery.Bootstrap.Css);
                var jsUrl = SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.JQuery.Bootstrap.Js);
                retval = $@"
<link rel=""stylesheet"" type=""text/css"" href=""{cssUrl}"">
<script language=""javascript"" src=""{jsUrl}""></script>
";
            }
            else if (pageJsName == JsAcSwfObject)
            {
                retval =
                    $@"<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.SwfObject.Js)}""></script>";
            }
            else if (pageJsName == JsAcJwPlayer6)
            {
                retval =
                    $@"<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.JwPlayer6.Js)}""></script><script type=""text/javascript"">jwplayer.key=""ABCDEFGHIJKLMOPQ"";</script>";
            }
            else if (pageJsName == JsAcFlowPlayer)
            {
                retval =
                    $@"<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.FlowPlayer.Js)}""></script>";
            }
            else if (pageJsName == JsAcMediaElement)
            {
                retval =
                    $@"<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.MediaElement.Js)}""></script><link rel=""stylesheet"" href=""{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.MediaElement.Css)}"" />";
            }
            else if (pageJsName == JsAcAudioJs)
            {
                retval =
                    $@"<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.AudioJs.Js)}""></script>
<script type='text/javascript'>
audiojs.events.ready(function() {{
    audiojs.createAll();
}});
</script>
";
            }
            else if (pageJsName == JsAcVideoJs)
            {
                retval = $@"
<link href=""{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.VideoJs.Css)}"" rel=""stylesheet"">
<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.VideoJs.Js)}""></script>
";
            }
            else if (pageJsName == JsPageOpenWindow)
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
            else if (pageJsName == JsUserScript)
            {
                retval = $@"
<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.Stl.JsPageScript)}""></script>
<script type=""text/javascript"">stlInit('{SiteFilesAssets.GetUrl(ApiUrl, string.Empty)}', '{PublishmentSystemInfo.PublishmentSystemId}', {PublishmentSystemInfo.PublishmentSystemUrl.TrimEnd('/')}');</script>
<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.Stl.JsUserScript)}""></script>";
            }
            else if (pageJsName == JsInnerCalendar)
            {
                retval = $@"<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.DatePicker.Js)}""></script>";
            }
            else if (pageJsName == JsStaticAdFloating)
            {
                retval =
                    $@"<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.Static.JsStaticAdFloating)}""></script>";
            }
            else if (pageJsName == JsAhTranslate)
            {
                retval =
                    $@"<script src=""{SiteFilesAssets.GetUrl(ApiUrl, SiteFilesAssets.TwCn.Js)}"" charset=""{SiteFilesAssets.TwCn.Charset}"" type=""text/javascript""></script>";
            }
            return retval;
        }

        public ICollection PageAfterBodyScriptKeys => _pageAfterBodyScripts.Keys;

        public ICollection PageBeforeBodyScriptKeys => _pageBeforeBodyScripts.Keys;

        public bool IsPageScriptsExists(string pageJsName)
        {
            return IsPageScriptsExists(pageJsName, true);
        }

        public bool IsPageScriptsExists(string pageJsName, bool isAfterBody)
        {
            if (isAfterBody)
            {
                return _pageAfterBodyScripts.ContainsKey(pageJsName);
            }
            else
            {
                return _pageBeforeBodyScripts.ContainsKey(pageJsName);
            }

        }

        public void AddPageScriptsIfNotExists(string pageJsName)
        {
            AddPageScriptsIfNotExists(pageJsName, true);
        }

        public void AddPageScriptsIfNotExists(string pageJsName, bool isAfterBody)
        {
            if (isAfterBody)
            {
                if (!_pageAfterBodyScripts.ContainsKey(pageJsName))
                {
                    _pageAfterBodyScripts.Add(pageJsName, GetJsCode(pageJsName));
                }
            }
            else
            {
                if (!_pageBeforeBodyScripts.ContainsKey(pageJsName))
                {
                    _pageBeforeBodyScripts.Add(pageJsName, GetJsCode(pageJsName));
                }
            }
        }

        public void AddPageScriptsIfNotExists(string pageJsName, string value)
        {
            AddPageScriptsIfNotExists(pageJsName, value, true);
        }

        public void AddPageScriptsIfNotExists(string pageJsName, string value, bool isAfterBody)
        {
            if (isAfterBody)
            {
                if (!_pageAfterBodyScripts.ContainsKey(pageJsName))
                {
                    _pageAfterBodyScripts.Add(pageJsName, value);
                }
            }
            else
            {
                if (!_pageBeforeBodyScripts.ContainsKey(pageJsName))
                {
                    _pageBeforeBodyScripts.Add(pageJsName, value);
                }
            }
        }

        public void SetPageScripts(string pageJsName, string value, bool isAfterBody)
        {
            if (isAfterBody)
            {
                _pageAfterBodyScripts[pageJsName] = value;
            }
            else
            {
                _pageBeforeBodyScripts[pageJsName] = value;
            }
        }

        public string GetPageScripts(string pageJsName, bool isAfterBody)
        {
            if (isAfterBody)
            {
                return _pageAfterBodyScripts[pageJsName];
            }
            else
            {
                return _pageBeforeBodyScripts[pageJsName];
            }
        }

        public ICollection PageEndScriptKeys => _pageEndScripts.Keys;

        public bool IsPageEndScriptsExists(string pageJsName)
        {
            return _pageEndScripts.ContainsKey(pageJsName);
        }

        public void AddPageEndScriptsIfNotExists(string pageJsName)
        {
            if (!_pageEndScripts.ContainsKey(pageJsName))
            {
                _pageEndScripts.Add(pageJsName, GetJsCode(pageJsName));
            }
        }

        public void AddPageEndScriptsIfNotExists(string pageJsName, string value)
        {
            if (!_pageEndScripts.ContainsKey(pageJsName))
            {
                _pageEndScripts.Add(pageJsName, value);
            }
        }

        public void SetPageEndScripts(string pageJsName, string value)
        {
            _pageEndScripts[pageJsName] = value;
        }

        public string GetPageEndScripts(string pageJsName)
        {
            return _pageEndScripts[pageJsName];
        }

        public ICollection PageHeadScriptKeys => _pageHeadScripts.Keys;

        public bool IsPageHeadScriptsExists(string pageJsName)
        {
            return _pageHeadScripts.ContainsKey(pageJsName);
        }

        public void AddPageHeadScriptsIfNotExists(string pageJsName)
        {
            if (!_pageHeadScripts.ContainsKey(pageJsName))
            {
                _pageHeadScripts.Add(pageJsName, GetJsCode(pageJsName));
            }
        }

        public void AddPageHeadScriptsIfNotExists(string pageJsName, string value)
        {
            if (!_pageHeadScripts.ContainsKey(pageJsName))
            {
                _pageHeadScripts.Add(pageJsName, value);
            }
        }

        public void SetPageHeadScripts(string pageJsName, string value)
        {
            _pageHeadScripts[pageJsName] = value;
        }

        public string GetPageHeadScripts(string pageJsName)
        {
            return _pageHeadScripts[pageJsName];
        }

        /// <summary>
        /// 将一个页面的js复制给本页面，提供给分页时使用
        /// add by sessionliang at 20151209
        /// </summary>
        /// <param name="lastPageInfo"></param>
        public void AddLastPageScript(PageInfo lastPageInfo)
        {
            foreach (string key in lastPageInfo.PageAfterBodyScriptKeys)
            {
                AddPageScriptsIfNotExists(key, lastPageInfo.GetPageScripts(key, true), true);
            }
            foreach (string key in lastPageInfo.PageBeforeBodyScriptKeys)
            {
                AddPageScriptsIfNotExists(key, lastPageInfo.GetPageScripts(key, false), false);
            }
            foreach (string key in lastPageInfo.PageEndScriptKeys)
            {
                AddPageEndScriptsIfNotExists(key, lastPageInfo.GetPageEndScripts(key));
            }
            foreach (string key in lastPageInfo.PageHeadScriptKeys)
            {
                AddPageHeadScriptsIfNotExists(key, lastPageInfo.GetPageHeadScripts(key));
            }
        }

        /// <summary>
        /// 将一个页面的js从本页面去除，提供给分页时使用
        ///  add by sessionliang at 20151209
        /// </summary>
        /// <param name="lastPageInfo"></param>
        public void ClearLastPageScript(PageInfo lastPageInfo)
        {
            foreach (string key in lastPageInfo.PageAfterBodyScriptKeys)
            {
                _pageAfterBodyScripts.Remove(key);
            }
            foreach (string key in lastPageInfo.PageBeforeBodyScriptKeys)
            {
                _pageBeforeBodyScripts.Remove(key);
            }
            foreach (string key in lastPageInfo.PageEndScriptKeys)
            {
                _pageEndScripts.Remove(key);
            }
            foreach (string key in lastPageInfo.PageHeadScriptKeys)
            {
                _pageHeadScripts.Remove(key);
            }
        }

        /// <summary>
        /// 清理本页面的js
        /// </summary>
        public void ClearLastPageScript()
        {
            _pageAfterBodyScripts.Clear();
            _pageBeforeBodyScripts.Clear();
            _pageEndScripts.Clear();
            _pageHeadScripts.Clear();
        }

        public List<InnerLinkInfo> CacheOfInnerLinkInfoList
        {
            get
            {
                var list = CacheUtils.Get("PageInfo_InnerLinkInfoList_" + PublishmentSystemId) as List<InnerLinkInfo>;
                if (list != null) return list;

                list = DataProvider.InnerLinkDao.GetInnerLinkInfoList(PublishmentSystemId);
                var innerLinkNameArrayList = new ArrayList();
                foreach (InnerLinkInfo innerLinkInfo in innerLinkNameArrayList)
                {
                    innerLinkNameArrayList.Add(innerLinkInfo.InnerLinkName);
                }
                if (PublishmentSystemInfo.Additional.IsInnerLinkByChannelName)
                {
                    var dic = NodeManager.GetNodeInfoHashtableByPublishmentSystemId(PublishmentSystemId);
                    foreach (NodeInfo nodeInfo in dic.Values)
                    {
                        if (!innerLinkNameArrayList.Contains(nodeInfo.NodeName))
                        {
                            var innerLinkInfo = new InnerLinkInfo(nodeInfo.NodeName, PublishmentSystemId, PageUtility.GetChannelUrl(PublishmentSystemInfo, nodeInfo));
                            list.Add(innerLinkInfo);
                            innerLinkNameArrayList.Add(nodeInfo.NodeName);
                        }
                    }
                }
                foreach (InnerLinkInfo innerLinkInfo in list)
                {
                    innerLinkInfo.InnerString = string.Format(PublishmentSystemInfo.Additional.InnerLinkFormatString, PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(PublishmentSystemInfo, innerLinkInfo.LinkUrl)), innerLinkInfo.InnerLinkName);
                }
                CacheUtils.Insert("PageInfo_InnerLinkInfoList_" + PublishmentSystemId, list, 10);
                return list;
            }
        }
    }
}
