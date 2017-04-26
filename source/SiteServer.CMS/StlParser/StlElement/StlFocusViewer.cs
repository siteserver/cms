using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlFocusViewer
    {
        private StlFocusViewer() { }
        public const string ElementName = "stl:focusviewer";//显示滚动焦点图

        public const string Attribute_ChannelIndex = "channelindex";			//栏目索引
        public const string Attribute_ChannelName = "channelname";				//栏目名称
        public const string Attribute_Scope = "scope";							//范围
        public const string Attribute_Group = "group";		                    //指定显示的内容组
        public const string Attribute_GroupNot = "groupnot";	                //指定不显示的内容组
        public const string Attribute_GroupChannel = "groupchannel";		    //指定显示的栏目组
        public const string Attribute_GroupChannelNot = "groupchannelnot";	    //指定不显示的栏目组
        public const string Attribute_GroupContent = "groupcontent";		    //指定显示的内容组
        public const string Attribute_GroupContentNot = "groupcontentnot";	    //指定不显示的内容组
        public const string Attribute_Tags = "tags";	                        //指定标签
        public const string Attribute_Order = "order";							//排序
        public const string Attribute_StartNum = "startnum";					//从第几条信息开始显示
        public const string Attribute_TotalNum = "totalnum";					//显示图片数目
        public const string Attribute_TitleWordNum = "titlewordnum";			//标题文字数量
        public const string Attribute_Where = "where";                          //获取滚动焦点图的条件判断

        public const string Attribute_IsTop = "istop";                       //仅显示置顶内容
        public const string Attribute_IsRecommend = "isrecommend";           //仅显示推荐内容
        public const string Attribute_IsHot = "ishot";                       //仅显示热点内容
        public const string Attribute_IsColor = "iscolor";                   //仅显示醒目内容

        public const string Attribute_Theme = "theme";                      //主题样式
        public const string Attribute_Width = "width";						//图片宽度
        public const string Attribute_Height = "height";					//图片高度
        public const string Attribute_BgColor = "bgcolor";					//背景色
        public const string Attribute_IsShowText = "isshowtext";			//是否显示文字标题
        public const string Attribute_IsTopText = "istoptext";				//是否文字显示在顶端
        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

        public const string Theme_Style1 = "Style1";
        public const string Theme_Style2 = "Style2";
        public const string Theme_Style3 = "Style3";
        public const string Theme_Style4 = "Style4";

        public static List<string> AttributeValuesTheme
        {
            get
            {
                var list = new List<string>();

                list.Add(Theme_Style1);
                list.Add(Theme_Style2);
                list.Add(Theme_Style3);
                list.Add(Theme_Style4);

                return list;
            }
        }

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary();
                attributes.Add(Attribute_ChannelIndex, "栏目索引");
                attributes.Add(Attribute_ChannelName, "栏目名称");
                attributes.Add(Attribute_Scope, "范围");
                attributes.Add(Attribute_GroupChannel, "指定显示的栏目组");
                attributes.Add(Attribute_GroupChannelNot, "指定不显示的栏目组");
                attributes.Add(Attribute_GroupContent, "指定显示的内容组");
                attributes.Add(Attribute_GroupContentNot, "指定不显示的内容组");
                attributes.Add(Attribute_Tags, "指定标签");
                attributes.Add(Attribute_Order, "排序");
                attributes.Add(Attribute_StartNum, "从第几条信息开始显示");
                attributes.Add(Attribute_TotalNum, "标题文字数量");
                attributes.Add(Attribute_TitleWordNum, "标题文字数量");
                attributes.Add(Attribute_Where, "获取滚动焦点图的条件判断");

                attributes.Add(Attribute_IsTop, "仅显示置顶内容");
                attributes.Add(Attribute_IsRecommend, "仅显示推荐内容");
                attributes.Add(Attribute_IsHot, "仅显示热点内容");
                attributes.Add(Attribute_IsColor, "仅显示醒目内容");

                attributes.Add(Attribute_Theme, "主题样式");
                attributes.Add(Attribute_Width, "图片宽度");
                attributes.Add(Attribute_Height, "图片高度");
                attributes.Add(Attribute_BgColor, "背景色");
                attributes.Add(Attribute_IsShowText, "是否显示文字标题");
                attributes.Add(Attribute_IsTopText, "是否文字显示在顶端");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
                return attributes;
            }
        }

        public static Dictionary<string, string> BooleanAttributeList
        {
            get
            {
                var attributes = new Dictionary<string, string>();

                attributes.Add(Attribute_IsTop, "仅显示置顶内容");
                attributes.Add(Attribute_IsRecommend, "仅显示推荐内容");
                attributes.Add(Attribute_IsHot, "仅显示热点内容");
                attributes.Add(Attribute_IsColor, "仅显示醒目内容");

                attributes.Add(Attribute_IsDynamic, "是否动态显示");

                return attributes;
            }
        }

        //对“flash滚动焦点图”（stl:focusViewer）元素进行解析
        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;
            try
            {
                var genericControl = new HtmlGenericControl("div");
                var ie = node.Attributes.GetEnumerator();

                var channelIndex = string.Empty;
                var channelName = string.Empty;
                var scopeType = EScopeType.Self;
                var groupChannel = string.Empty;
                var groupChannelNot = string.Empty;
                var groupContent = string.Empty;
                var groupContentNot = string.Empty;
                var tags = string.Empty;
                var orderByString = ETaxisTypeUtils.GetOrderByString(ETableStyle.BackgroundContent, ETaxisType.OrderByTaxisDesc, string.Empty, null);
                var startNum = 1;
                var totalNum = 0;
                var isShowText = true;
                var isTopText = string.Empty;
                var titleWordNum = 0;
                var where = string.Empty;

                var isTop = false;
                var isTopExists = false;
                var isRecommend = false;
                var isRecommendExists = false;
                var isHot = false;
                var isHotExists = false;
                var isColor = false;
                var isColorExists = false;

                var theme = string.Empty;
                var imageWidth = 260;
                var imageHeight = 182;
                var textHeight = 25;
                var bgColor = string.Empty;
                var isDynamic = false;

                while (ie.MoveNext())
                {
                    var attr = (XmlAttribute)ie.Current;
                    var attributeName = attr.Name.ToLower();
                    if (attributeName.Equals(Attribute_ChannelIndex))
                    {
                        channelIndex = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_ChannelName))
                    {
                        channelName = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_Scope))
                    {
                        scopeType = EScopeTypeUtils.GetEnumType(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_GroupChannel))
                    {
                        groupChannel = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_GroupChannelNot))
                    {
                        groupChannelNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_GroupContent))
                    {
                        groupContent = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_GroupContentNot))
                    {
                        groupContentNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_Tags))
                    {
                        tags = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_Order))
                    {
                        orderByString = StlDataUtility.GetOrderByString(pageInfo.PublishmentSystemId, attr.Value, ETableStyle.BackgroundContent, ETaxisType.OrderByTaxisDesc);
                    }
                    else if (attributeName.Equals(Attribute_StartNum))
                    {
                        startNum = TranslateUtils.ToInt(attr.Value, 1);
                    }
                    else if (attributeName.Equals(Attribute_TotalNum))
                    {
                        totalNum = TranslateUtils.ToInt(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_TitleWordNum))
                    {
                        titleWordNum = TranslateUtils.ToInt(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_Where))
                    {
                        where = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_IsTop))
                    {
                        isTopExists = true;
                        isTop = TranslateUtils.ToBool(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_IsRecommend))
                    {
                        isRecommendExists = true;
                        isRecommend = TranslateUtils.ToBool(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_IsHot))
                    {
                        isHotExists = true;
                        isHot = TranslateUtils.ToBool(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_IsColor))
                    {
                        isColorExists = true;
                        isColor = TranslateUtils.ToBool(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_Theme))
                    {
                        theme = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_Width))
                    {
                        if (StringUtils.EndsWithIgnoreCase(attr.Value, "px"))
                        {
                            attr.Value = attr.Value.Substring(0, attr.Value.Length - 2);
                        }
                        imageWidth = TranslateUtils.ToInt(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_Height))
                    {
                        if (StringUtils.EndsWithIgnoreCase(attr.Value, "px"))
                        {
                            attr.Value = attr.Value.Substring(0, attr.Value.Length - 2);
                        }
                        imageHeight = TranslateUtils.ToInt(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_BgColor))
                    {
                        bgColor = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_IsShowText))
                    {
                        isShowText = TranslateUtils.ToBool(attr.Value, true);
                    }
                    else if (attributeName.Equals(Attribute_IsTopText))
                    {
                        isTopText = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value);
                    }
                    else
                    {
                        genericControl.Attributes.Remove(attributeName);
                        genericControl.Attributes.Add(attributeName, attr.Value);
                    }
                }

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(pageInfo, contextInfo, genericControl, channelIndex, channelName, scopeType, groupChannel, groupChannelNot, groupContent, groupContentNot, tags, orderByString, startNum, totalNum, isShowText, isTopText, titleWordNum, where, isTop, isTopExists, isRecommend, isRecommendExists, isHot, isHotExists, isColor, isColorExists, theme, imageWidth, imageHeight, textHeight, bgColor);
                }
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, HtmlGenericControl genericControl, string channelIndex, string channelName, EScopeType scopeType, string groupChannel, string groupChannelNot, string groupContent, string groupContentNot, string tags, string orderByString, int startNum, int totalNum, bool isShowText, string isTopText, int titleWordNum, string where, bool isTop, bool isTopExists, bool isRecommend, bool isRecommendExists, bool isHot, bool isHotExists, bool isColor, bool isColorExists, string theme, int imageWidth, int imageHeight, int textHeight, string bgColor)
        {
            var parsedContent = string.Empty;

            var channelID = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, contextInfo.ChannelID, channelIndex, channelName);

            var dataSource = StlDataUtility.GetContentsDataSource(pageInfo.PublishmentSystemInfo, channelID, 0, groupContent, groupContentNot, tags, true, true, false, false, false, false, false, false, startNum, totalNum, orderByString, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where, scopeType, groupChannel, groupChannelNot, null);

            if (dataSource != null)
            {
                if (StringUtils.EqualsIgnoreCase(theme, Theme_Style2))
                {
                    pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAcSwfObject);

                    var imageUrls = new StringCollection();
                    var navigationUrls = new StringCollection();
                    var titleCollection = new StringCollection();

                    foreach (var dataItem in dataSource)
                    {
                        var contentInfo = new BackgroundContentInfo(dataItem);
                        if (contentInfo != null && !string.IsNullOrEmpty(contentInfo.ImageUrl))
                        {
                            if (contentInfo.ImageUrl.ToLower().EndsWith(".jpg") || contentInfo.ImageUrl.ToLower().EndsWith(".jpeg") || contentInfo.ImageUrl.ToLower().EndsWith(".png") || contentInfo.ImageUrl.ToLower().EndsWith(".pneg"))
                            {
                                titleCollection.Add(StringUtils.ToJsString(PageUtils.UrlEncode(StringUtils.MaxLengthText(StringUtils.StripTags(contentInfo.Title), titleWordNum))));
                                navigationUrls.Add(PageUtils.UrlEncode(PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, contentInfo)));
                                imageUrls.Add(PageUtils.UrlEncode(PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, contentInfo.ImageUrl)));
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(bgColor))
                    {
                        bgColor = "0xDADADA";
                    }
                    else
                    {
                        bgColor = bgColor.TrimStart('#');
                        if (!bgColor.StartsWith("0x"))
                        {
                            bgColor = "0x" + bgColor;
                        }
                    }

                    if (string.IsNullOrEmpty(isTopText))
                    {
                        isTopText = "0";
                    }
                    else
                    {
                        isTopText = (TranslateUtils.ToBool(isTopText)) ? "0" : "1";
                    }

                    var uniqueID = "FocusViewer_" + pageInfo.UniqueId;
                    var paramBuilder = new StringBuilder();
                    paramBuilder.Append(
                        $@"so_{uniqueID}.addParam(""quality"", ""high"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append(
                        $@"so_{uniqueID}.addParam(""wmode"", ""transparent"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append(
                        $@"so_{uniqueID}.addParam(""menu"", ""false"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append(
                        $@"so_{uniqueID}.addParam(""FlashVars"", ""bcastr_file=""+files_uniqueID+""&bcastr_link=""+links_uniqueID+""&bcastr_title=""+texts_uniqueID+""&AutoPlayTime=5&TitleBgPosition={isTopText}&TitleBgColor={bgColor}&BtnDefaultColor={bgColor}"");").Append(StringUtils.Constants.ReturnAndNewline);

                    string scriptHtml = $@"
<div id=""flashcontent_{uniqueID}""></div>
<script type=""text/javascript"">
var files_uniqueID='{TranslateUtils.ObjectCollectionToString(imageUrls, "|")}';
var links_uniqueID='{TranslateUtils.ObjectCollectionToString(navigationUrls, "|")}';
var texts_uniqueID='{TranslateUtils.ObjectCollectionToString(titleCollection, "|")}';

var so_{uniqueID} = new SWFObject(""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.Flashes.Bcastr)}"", ""flash_{uniqueID}"", ""{imageWidth}"", ""{imageHeight}"", ""7"", """");
{paramBuilder}
so_{uniqueID}.write(""flashcontent_{uniqueID}"");
</script>
";
                    scriptHtml = scriptHtml.Replace("uniqueID", uniqueID);

                    parsedContent = scriptHtml;
                }
                else if (StringUtils.EqualsIgnoreCase(theme, Theme_Style3))
                {
                    pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAcSwfObject);

                    var imageUrls = new StringCollection();
                    var navigationUrls = new StringCollection();
                    var titleCollection = new StringCollection();

                    foreach (var dataItem in dataSource)
                    {
                        var contentInfo = new BackgroundContentInfo(dataItem);
                        if (contentInfo != null && !string.IsNullOrEmpty(contentInfo.ImageUrl))
                        {
                            if (contentInfo.ImageUrl.ToLower().EndsWith(".jpg") || contentInfo.ImageUrl.ToLower().EndsWith(".jpeg") || contentInfo.ImageUrl.ToLower().EndsWith(".png") || contentInfo.ImageUrl.ToLower().EndsWith(".pneg"))
                            {
                                titleCollection.Add(StringUtils.ToJsString(PageUtils.UrlEncode(StringUtils.MaxLengthText(StringUtils.StripTags(contentInfo.Title), titleWordNum))));
                                navigationUrls.Add(PageUtils.UrlEncode(PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, contentInfo)));
                                imageUrls.Add(PageUtils.UrlEncode(PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, contentInfo.ImageUrl)));
                            }
                        }
                    }

                    var uniqueID = "FocusViewer_" + pageInfo.UniqueId;
                    var paramBuilder = new StringBuilder();
                    paramBuilder.Append($@"so_{uniqueID}.addParam(""quality"", ""high"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append($@"so_{uniqueID}.addParam(""wmode"", ""transparent"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append($@"so_{uniqueID}.addParam(""allowFullScreen"", ""true"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append($@"so_{uniqueID}.addParam(""allowScriptAccess"", ""always"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append($@"so_{uniqueID}.addParam(""menu"", ""false"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append(
                        $@"so_{uniqueID}.addParam(""flashvars"", ""pw={imageWidth}&ph={imageHeight}&Times=4000&sizes=14&umcolor=16777215&btnbg=12189697&txtcolor=16777215&urls=""+urls_uniqueID+""&imgs=""+imgs_uniqueID+""&titles=""+titles_uniqueID);").Append(StringUtils.Constants.ReturnAndNewline);

                    string scriptHtml = $@"
<div id=""flashcontent_{uniqueID}""></div>
<script type=""text/javascript"">
var urls_uniqueID='{TranslateUtils.ObjectCollectionToString(navigationUrls, "|")}';
var imgs_uniqueID='{TranslateUtils.ObjectCollectionToString(imageUrls, "|")}';
var titles_uniqueID='{TranslateUtils.ObjectCollectionToString(titleCollection, "|")}';

var so_{uniqueID} = new SWFObject(""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.Flashes.Ali)}"", ""flash_{uniqueID}"", ""{imageWidth}"", ""{imageHeight}"", ""7"", """");
{paramBuilder}
so_{uniqueID}.write(""flashcontent_{uniqueID}"");
</script>
";
                    scriptHtml = scriptHtml.Replace("uniqueID", uniqueID);

                    parsedContent = scriptHtml;
                }
                else if (StringUtils.EqualsIgnoreCase(theme, Theme_Style4))
                {
                    var imageUrls = new StringCollection();
                    var navigationUrls = new StringCollection();

                    foreach (var dataItem in dataSource)
                    {
                        var contentInfo = new BackgroundContentInfo(dataItem);
                        if (contentInfo != null && !string.IsNullOrEmpty(contentInfo.ImageUrl))
                        {
                            navigationUrls.Add(PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, contentInfo));
                            imageUrls.Add(PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, contentInfo.ImageUrl));
                        }
                    }

                    var imageBuilder = new StringBuilder();
                    var numBuilder = new StringBuilder();

                    imageBuilder.Append($@"
<div style=""display:block""><a href=""{navigationUrls[0]}"" target=""_blank""><img src=""{imageUrls[0]}"" width=""{imageWidth}"" height=""{imageHeight}"" border=""0"" onMouseOver=""fv_clearAuto();"" onMouseOut=""fv_setAuto()"" /></a></div>
");

                    if (navigationUrls.Count > 1)
                    {
                        for (var i = 1; i < navigationUrls.Count; i++)
                        {
                            imageBuilder.Append($@"
<div style=""display:none""><a href=""{navigationUrls[i]}"" target=""_blank""><img src=""{imageUrls[i]}"" width=""{imageWidth}"" height=""{imageHeight}"" border=""0"" onMouseOver=""fv_clearAuto();"" onMouseOut=""fv_setAuto()"" /></a></div>
");
                        }
                    }

                    numBuilder.Append(@"
<td width=""18"" height=""16"" class=""fv_bigon"" onmouseover=""fv_clearAuto();fv_Mea(0);"" onmouseout=""fv_setAuto()"">1</td>
");

                    if (navigationUrls.Count > 1)
                    {
                        for (var i = 1; i < navigationUrls.Count; i++)
                        {
                            numBuilder.Append($@"
<td width=""18"" class=""fv_bigoff"" onmouseover=""fv_clearAuto();fv_Mea({i});"" onmouseout=""fv_setAuto()"">{i + 1}</td>
");
                        }
                    }

                    var bgUrl = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo,
                        "@/images/focusviewerbg.png");
                    string scriptHtml = $@"
<style type=""text/css"">
.fv_adnum{{ position:absolute; z-index:1005; width:{imageWidth - 24}px; height:24px; padding:5px 3px 0 0; left:21px; top:{imageHeight -
                                                                                                                                                                                                  29}px; }}
.fv_bigon{{ background:#E59948; font-family:Arial; color:#fff; font-size:12px; text-align:center; cursor:pointer}}
.fv_bigoff{{ background:#7DCABD; font-family:Arial; color:#fff; font-size:12px; text-align:center; cursor:pointer}}
</style>
<div style=""position:relative; left:0; top:0; width:{imageWidth}px; height:{imageHeight}px; background:#000"">
	<div id=""fv_filbox"" style=""position:absolute; z-index:999; left:0; top:0; width:{imageWidth}px; height:{imageHeight}px; filter:progid:DXImageTransform.Microsoft.Fade( duration=0.5,overlap=1.0 );"">
		{imageBuilder}
    </div>
    <div class=""fv_adnum"" style=""background:url({bgUrl}) no-repeat !important; background:none ;filter: progid:DXImageTransform.Microsoft.AlphaImageLoader(enabled=true, src='{bgUrl}')""></div>
    <div class=""fv_adnum"">
        <table border=""0"" cellspacing=""1"" cellpadding=""0"" align=""right"" id=""fv_num"">
          <tr>
            {numBuilder}
          </tr>
        </table>
    </div>
</div>
<script language=""javascript""> 
    var fv_n=0;
    var fv_nums={navigationUrls.Count};
    var fv_showNum = document.getElementById(""fv_num"");
    var is_IE=(navigator.appName==""Microsoft Internet Explorer"");
    function fv_Mea(value){{
        fv_n=value;
        for(var i=0;i<fv_nums;i++)
            if(value==i){{
                fv_showNum.getElementsByTagName(""td"")[i].setAttribute('className', 'fv_bigon');
                fv_showNum.getElementsByTagName(""td"")[i].setAttribute('class', 'fv_bigon');
            }}
            else{{	
                fv_showNum.getElementsByTagName(""td"")[i].setAttribute('className', 'fv_bigoff');
                fv_showNum.getElementsByTagName(""td"")[i].setAttribute('class', 'fv_bigoff');
            }}
        var divs = document.getElementById(""fv_filbox"").getElementsByTagName(""div""); 
		if (is_IE){{
        	document.getElementById(""fv_filbox"").filters[0].Apply();
		}}
		for(i=0;i<fv_nums;i++)i==value?divs[i].style.display=""block"":divs[i].style.display=""none"";
		if (is_IE){{
			document.getElementById(""fv_filbox"").filters[0].play();
		}}
    }}
    function fv_clearAuto(){{clearInterval(autoStart)}}
    function fv_setAuto(){{autoStart=setInterval(""auto(fv_n)"", 5000)}}
    function auto(){{
        fv_n++;
        if(fv_n>(fv_nums-1))fv_n=0;
        fv_Mea(fv_n);
    }}
    fv_setAuto(); 
</script>
";

                    parsedContent = scriptHtml.Replace("fv_", $"fv{pageInfo.UniqueId}_");
                }
                else
                {
                    var imageUrls = new StringCollection();
                    var navigationUrls = new StringCollection();
                    var titleCollection = new StringCollection();

                    foreach (var dataItem in dataSource)
                    {
                        var contentInfo = new BackgroundContentInfo(dataItem);
                        if (contentInfo != null && !string.IsNullOrEmpty(contentInfo.ImageUrl))
                        {
                            //这里使用png图片不管用
                            //||contentInfo.ImageUrl.ToLower().EndsWith(".png")||contentInfo.ImageUrl.ToLower().EndsWith(".pneg")
                            if (contentInfo.ImageUrl.ToLower().EndsWith(".jpg") || contentInfo.ImageUrl.ToLower().EndsWith(".jpeg"))
                            {
                                titleCollection.Add(StringUtils.ToJsString(PageUtils.UrlEncode(StringUtils.MaxLengthText(StringUtils.StripTags(contentInfo.Title), titleWordNum))));
                                navigationUrls.Add(PageUtils.UrlEncode(PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, contentInfo)));
                                imageUrls.Add(PageUtils.UrlEncode(PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, contentInfo.ImageUrl)));
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(bgColor))
                    {
                        bgColor = "#DADADA";
                    }
                    var titles = string.Empty;
                    if (isShowText == false)
                    {
                        textHeight = 0;
                    }
                    else
                    {
                        titles = TranslateUtils.ObjectCollectionToString(titleCollection, "|");
                    }
                    var uniqueID = "FocusViewer_" + pageInfo.UniqueId;
                    genericControl.ID = uniqueID;
                    genericControl.InnerHtml = "&nbsp;";
                    var divHtml = ControlUtils.GetControlRenderHtml(genericControl);
                    string scriptHtml = $@"
<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.BaiRongFlash.Js)}""></script>
<SCRIPT language=javascript type=""text/javascript"">
	<!--
	
	var uniqueID_focus_width={imageWidth}
	var uniqueID_focus_height={imageHeight}
	var uniqueID_text_height={textHeight}
	var uniqueID_swf_height = uniqueID_focus_height + uniqueID_text_height
	
	var uniqueID_pics='{TranslateUtils.ObjectCollectionToString(imageUrls, "|")}'
	var uniqueID_links='{TranslateUtils.ObjectCollectionToString(navigationUrls, "|")}'
	var uniqueID_texts='{titles}'
	
	var uniqueID_FocusFlash = new bairongFlash(""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.Flashes.FocusViewer)}"", ""focusflash"", uniqueID_focus_width, uniqueID_swf_height, ""7"", ""{bgColor}"", false, ""High"");
	uniqueID_FocusFlash.addParam(""allowScriptAccess"", ""sameDomain"");
	uniqueID_FocusFlash.addParam(""menu"", ""false"");
	uniqueID_FocusFlash.addParam(""wmode"", ""transparent"");

	uniqueID_FocusFlash.addVariable(""pics"", uniqueID_pics);
	uniqueID_FocusFlash.addVariable(""links"", uniqueID_links);
	uniqueID_FocusFlash.addVariable(""texts"", uniqueID_texts);
	uniqueID_FocusFlash.addVariable(""borderwidth"", uniqueID_focus_width);
	uniqueID_FocusFlash.addVariable(""borderheight"", uniqueID_focus_height);
	uniqueID_FocusFlash.addVariable(""textheight"", uniqueID_text_height);
	uniqueID_FocusFlash.write(""uniqueID"");
	
	//-->
</SCRIPT>
";
                    scriptHtml = scriptHtml.Replace("uniqueID", uniqueID);

                    parsedContent = divHtml + scriptHtml;
                }
            }

            return parsedContent;
        }
    }
}
