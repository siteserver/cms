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
    [Stl(Usage = "滚动焦点图", Description = "通过 stl:focusviewer 标签在模板中实现由 FLASH 显示的图片轮播效果")]
    public class StlFocusViewer
    {
        private StlFocusViewer() { }
        public const string ElementName = "stl:focusViewer";

        public const string AttributeChannelIndex = "channelIndex";
        public const string AttributeChannelName = "channelName";
        public const string AttributeScope = "scope";
        public const string AttributeGroup = "group";
        public const string AttributeGroupNot = "groupNot";
        public const string AttributeGroupChannel = "groupChannel";
        public const string AttributeGroupChannelNot = "groupChannelNot";
        public const string AttributeGroupContent = "groupContent";
        public const string AttributeGroupContentNot = "groupContentNot";
        public const string AttributeTags = "tags";
        public const string AttributeOrder = "order";
        public const string AttributeStartNum = "startNum";
        public const string AttributeTotalNum = "totalNum";
        public const string AttributeTitleWordNum = "titleWordNum";
        public const string AttributeWhere = "where";
        public const string AttributeIsTop = "isTop";
        public const string AttributeIsRecommend = "isRecommend";
        public const string AttributeIsHot = "isHot";
        public const string AttributeIsColor = "isColor";
        public const string AttributeTheme = "theme";
        public const string AttributeWidth = "width";
        public const string AttributeHeight = "height";
        public const string AttributeBgColor = "bgColor";
        public const string AttributeIsShowText = "isShowText";
        public const string AttributeIsTopText = "isTopText";
        public const string AttributeIsDynamic = "isDynamic";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeChannelIndex, "栏目索引"},
            {AttributeChannelName, "栏目名称"},
            {AttributeScope, "范围"},
            {AttributeGroupChannel, "指定显示的栏目组"},
            {AttributeGroupChannelNot, "指定不显示的栏目组"},
            {AttributeGroupContent, "指定显示的内容组"},
            {AttributeGroupContentNot, "指定不显示的内容组"},
            {AttributeGroup, "指定显示的内容组"},
            {AttributeGroupNot, "指定不显示的内容组"},
            {AttributeTags, "指定标签"},
            {AttributeOrder, "排序"},
            {AttributeStartNum, "从第几条信息开始显示"},
            {AttributeTotalNum, "标题文字数量"},
            {AttributeTitleWordNum, "标题文字数量"},
            {AttributeWhere, "获取滚动焦点图的条件判断"},
            {AttributeIsTop, "仅显示置顶内容"},
            {AttributeIsRecommend, "仅显示推荐内容"},
            {AttributeIsHot, "仅显示热点内容"},
            {AttributeIsColor, "仅显示醒目内容"},
            {AttributeTheme, StringUtils.SortedListToAttributeValueString("主题样式", ThemeList)},
            {AttributeWidth, "图片宽度"},
            {AttributeHeight, "图片高度"},
            {AttributeBgColor, "背景色"},
            {AttributeIsShowText, "是否显示文字标题"},
            {AttributeIsTopText, "是否文字显示在顶端"},
            {AttributeIsDynamic, "是否动态显示"}
        };

        public const string ThemeStyle1 = "Style1";
        public const string ThemeStyle2 = "Style2";
        public const string ThemeStyle3 = "Style3";
        public const string ThemeStyle4 = "Style4";

        public static SortedList<string, string> ThemeList => new SortedList<string, string>
        {
            {ThemeStyle1, "样式1"},
            {ThemeStyle2, "样式2"},
            {ThemeStyle3, "样式3"},
            {ThemeStyle4, "样式4"},
        };

        //对“flash滚动焦点图”（stl:focusViewer）元素进行解析
        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent;
            try
            {
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
                var genericControl = new HtmlGenericControl("div");

                var ie = node.Attributes?.GetEnumerator();
                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChannelIndex))
                        {
                            channelIndex = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChannelName))
                        {
                            channelName = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeScope))
                        {
                            scopeType = EScopeTypeUtils.GetEnumType(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeGroupChannel))
                        {
                            groupChannel = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeGroupChannelNot))
                        {
                            groupChannelNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeGroupContent))
                        {
                            groupContent = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeGroupContentNot))
                        {
                            groupContentNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTags))
                        {
                            tags = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeOrder))
                        {
                            orderByString = StlDataUtility.GetOrderByString(pageInfo.PublishmentSystemId, attr.Value, ETableStyle.BackgroundContent, ETaxisType.OrderByTaxisDesc);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeStartNum))
                        {
                            startNum = TranslateUtils.ToInt(attr.Value, 1);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTotalNum))
                        {
                            totalNum = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTitleWordNum))
                        {
                            titleWordNum = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeWhere))
                        {
                            where = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsTop))
                        {
                            isTopExists = true;
                            isTop = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsRecommend))
                        {
                            isRecommendExists = true;
                            isRecommend = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsHot))
                        {
                            isHotExists = true;
                            isHot = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsColor))
                        {
                            isColorExists = true;
                            isColor = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTheme))
                        {
                            theme = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeWidth))
                        {
                            if (StringUtils.EndsWithIgnoreCase(attr.Value, "px"))
                            {
                                attr.Value = attr.Value.Substring(0, attr.Value.Length - 2);
                            }
                            imageWidth = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeHeight))
                        {
                            if (StringUtils.EndsWithIgnoreCase(attr.Value, "px"))
                            {
                                attr.Value = attr.Value.Substring(0, attr.Value.Length - 2);
                            }
                            imageHeight = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeBgColor))
                        {
                            bgColor = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsShowText))
                        {
                            isShowText = TranslateUtils.ToBool(attr.Value, true);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsTopText))
                        {
                            isTopText = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                        else
                        {
                            genericControl.Attributes[attr.Name] = attr.Value;
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(pageInfo, contextInfo, genericControl, channelIndex, channelName, scopeType, groupChannel, groupChannelNot, groupContent, groupContentNot, tags, orderByString, startNum, totalNum, isShowText, isTopText, titleWordNum, where, isTop, isTopExists, isRecommend, isRecommendExists, isHot, isHotExists, isColor, isColorExists, theme, imageWidth, imageHeight, textHeight, bgColor);
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

            var channelId = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, contextInfo.ChannelId, channelIndex, channelName);

            var dataSource = StlDataUtility.GetContentsDataSource(pageInfo.PublishmentSystemInfo, channelId, 0, groupContent, groupContentNot, tags, true, true, false, false, false, false, false, false, startNum, totalNum, orderByString, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where, scopeType, groupChannel, groupChannelNot, null);

            if (dataSource != null)
            {
                if (StringUtils.EqualsIgnoreCase(theme, ThemeStyle2))
                {
                    pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAcSwfObject);

                    var imageUrls = new StringCollection();
                    var navigationUrls = new StringCollection();
                    var titleCollection = new StringCollection();

                    foreach (var dataItem in dataSource)
                    {
                        var contentInfo = new BackgroundContentInfo(dataItem);
                        if (!string.IsNullOrEmpty(contentInfo?.ImageUrl))
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

                    var uniqueId = "FocusViewer_" + pageInfo.UniqueId;
                    var paramBuilder = new StringBuilder();
                    paramBuilder.Append(
                        $@"so_{uniqueId}.addParam(""quality"", ""high"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append(
                        $@"so_{uniqueId}.addParam(""wmode"", ""transparent"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append(
                        $@"so_{uniqueId}.addParam(""menu"", ""false"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append(
                        $@"so_{uniqueId}.addParam(""FlashVars"", ""bcastr_file=""+files_uniqueID+""&bcastr_link=""+links_uniqueID+""&bcastr_title=""+texts_uniqueID+""&AutoPlayTime=5&TitleBgPosition={isTopText}&TitleBgColor={bgColor}&BtnDefaultColor={bgColor}"");").Append(StringUtils.Constants.ReturnAndNewline);

                    string scriptHtml = $@"
<div id=""flashcontent_{uniqueId}""></div>
<script type=""text/javascript"">
var files_uniqueID='{TranslateUtils.ObjectCollectionToString(imageUrls, "|")}';
var links_uniqueID='{TranslateUtils.ObjectCollectionToString(navigationUrls, "|")}';
var texts_uniqueID='{TranslateUtils.ObjectCollectionToString(titleCollection, "|")}';

var so_{uniqueId} = new SWFObject(""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.Flashes.Bcastr)}"", ""flash_{uniqueId}"", ""{imageWidth}"", ""{imageHeight}"", ""7"", """");
{paramBuilder}
so_{uniqueId}.write(""flashcontent_{uniqueId}"");
</script>
";
                    scriptHtml = scriptHtml.Replace("uniqueID", uniqueId);

                    parsedContent = scriptHtml;
                }
                else if (StringUtils.EqualsIgnoreCase(theme, ThemeStyle3))
                {
                    pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAcSwfObject);

                    var imageUrls = new StringCollection();
                    var navigationUrls = new StringCollection();
                    var titleCollection = new StringCollection();

                    foreach (var dataItem in dataSource)
                    {
                        var contentInfo = new BackgroundContentInfo(dataItem);
                        if (!string.IsNullOrEmpty(contentInfo?.ImageUrl))
                        {
                            if (contentInfo.ImageUrl.ToLower().EndsWith(".jpg") || contentInfo.ImageUrl.ToLower().EndsWith(".jpeg") || contentInfo.ImageUrl.ToLower().EndsWith(".png") || contentInfo.ImageUrl.ToLower().EndsWith(".pneg"))
                            {
                                titleCollection.Add(StringUtils.ToJsString(PageUtils.UrlEncode(StringUtils.MaxLengthText(StringUtils.StripTags(contentInfo.Title), titleWordNum))));
                                navigationUrls.Add(PageUtils.UrlEncode(PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, contentInfo)));
                                imageUrls.Add(PageUtils.UrlEncode(PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, contentInfo.ImageUrl)));
                            }
                        }
                    }

                    var uniqueId = "FocusViewer_" + pageInfo.UniqueId;
                    var paramBuilder = new StringBuilder();
                    paramBuilder.Append($@"so_{uniqueId}.addParam(""quality"", ""high"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append($@"so_{uniqueId}.addParam(""wmode"", ""transparent"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append($@"so_{uniqueId}.addParam(""allowFullScreen"", ""true"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append($@"so_{uniqueId}.addParam(""allowScriptAccess"", ""always"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append($@"so_{uniqueId}.addParam(""menu"", ""false"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append(
                        $@"so_{uniqueId}.addParam(""flashvars"", ""pw={imageWidth}&ph={imageHeight}&Times=4000&sizes=14&umcolor=16777215&btnbg=12189697&txtcolor=16777215&urls=""+urls_uniqueID+""&imgs=""+imgs_uniqueID+""&titles=""+titles_uniqueID);").Append(StringUtils.Constants.ReturnAndNewline);

                    string scriptHtml = $@"
<div id=""flashcontent_{uniqueId}""></div>
<script type=""text/javascript"">
var urls_uniqueID='{TranslateUtils.ObjectCollectionToString(navigationUrls, "|")}';
var imgs_uniqueID='{TranslateUtils.ObjectCollectionToString(imageUrls, "|")}';
var titles_uniqueID='{TranslateUtils.ObjectCollectionToString(titleCollection, "|")}';

var so_{uniqueId} = new SWFObject(""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.Flashes.Ali)}"", ""flash_{uniqueId}"", ""{imageWidth}"", ""{imageHeight}"", ""7"", """");
{paramBuilder}
so_{uniqueId}.write(""flashcontent_{uniqueId}"");
</script>
";
                    scriptHtml = scriptHtml.Replace("uniqueID", uniqueId);

                    parsedContent = scriptHtml;
                }
                else if (StringUtils.EqualsIgnoreCase(theme, ThemeStyle4))
                {
                    var imageUrls = new StringCollection();
                    var navigationUrls = new StringCollection();

                    foreach (var dataItem in dataSource)
                    {
                        var contentInfo = new BackgroundContentInfo(dataItem);
                        if (!string.IsNullOrEmpty(contentInfo?.ImageUrl))
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
                        if (!string.IsNullOrEmpty(contentInfo?.ImageUrl))
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
                    var uniqueId = "FocusViewer_" + pageInfo.UniqueId;
                    genericControl.ID = uniqueId;
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
                    scriptHtml = scriptHtml.Replace("uniqueID", uniqueId);

                    parsedContent = divHtml + scriptHtml;
                }
            }

            return parsedContent;
        }
    }
}
