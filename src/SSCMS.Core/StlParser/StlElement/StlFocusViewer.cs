using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Enums;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "滚动焦点图")]
    public static class StlFocusViewer
    {
        public const string ElementName = "stl:focusViewer";

        public const string AttributeChannelIndex = "channelIndex";
        public const string AttributeIndex = "index";
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

        public const string ThemeStyle1 = "Style1";
        public const string ThemeStyle2 = "Style2";
        public const string ThemeStyle3 = "Style3";
        public const string ThemeStyle4 = "Style4";

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            // 如果是实体标签则返回空
            if (parseManager.ContextInfo.IsStlEntity)
            {
                return string.Empty;
            }

            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var scopeType = ScopeType.Self;
            var groupChannel = string.Empty;
            var groupChannelNot = string.Empty;
            var groupContent = string.Empty;
            var groupContentNot = string.Empty;
            var tags = string.Empty;
            var orderByString = parseManager.DatabaseManager.GetContentOrderByString(TaxisType.OrderByTaxisDesc);
            var startNum = 1;
            var totalNum = 0;
            var isShowText = true;
            var isTopText = string.Empty;
            var titleWordNum = 0;

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
            var attributes = new NameValueCollection();

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeChannelIndex) || StringUtils.EqualsIgnoreCase(name, AttributeIndex))
                {
                    channelIndex = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeChannelName))
                {
                    channelName = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeScope))
                {
                    scopeType = TranslateUtils.ToEnum(value, ScopeType.Self);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeGroupChannel))
                {
                    groupChannel = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeGroupChannelNot))
                {
                    groupChannelNot = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeGroupContent))
                {
                    groupContent = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeGroupContentNot))
                {
                    groupContentNot = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTags))
                {
                    tags = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeOrder))
                {
                    var dataManager = new StlDataManager(parseManager.DatabaseManager);
                    orderByString = dataManager.GetContentOrderByString(parseManager.PageInfo.SiteId, value, TaxisType.OrderByTaxisDesc);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeStartNum))
                {
                    startNum = TranslateUtils.ToInt(value, 1);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTotalNum))
                {
                    totalNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTitleWordNum))
                {
                    titleWordNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsTop))
                {
                    isTopExists = true;
                    isTop = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsRecommend))
                {
                    isRecommendExists = true;
                    isRecommend = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsHot))
                {
                    isHotExists = true;
                    isHot = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsColor))
                {
                    isColorExists = true;
                    isColor = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTheme))
                {
                    theme = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeWidth))
                {
                    if (StringUtils.EndsWithIgnoreCase(value, "px"))
                    {
                        value = value.Substring(0, value.Length - 2);
                    }
                    imageWidth = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeHeight))
                {
                    if (StringUtils.EndsWithIgnoreCase(value, "px"))
                    {
                        value = value.Substring(0, value.Length - 2);
                    }
                    imageHeight = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeBgColor))
                {
                    bgColor = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsShowText))
                {
                    isShowText = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsTopText))
                {
                    isTopText = value;
                }
                else
                {
                    attributes[name] = value;
                }
            }

            return await ParseAsync(parseManager, attributes, channelIndex, channelName, scopeType, groupChannel, groupChannelNot, groupContent, groupContentNot, tags, orderByString, startNum, totalNum, isShowText, isTopText, titleWordNum, isTop, isTopExists, isRecommend, isRecommendExists, isHot, isHotExists, isColor, isColorExists, theme, imageWidth, imageHeight, textHeight, bgColor);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, NameValueCollection attributes, string channelIndex, string channelName, ScopeType scopeType, string groupChannel, string groupChannelNot, string groupContent, string groupContentNot, string tags, string orderByString, int startNum, int totalNum, bool isShowText, string isTopText, int titleWordNum, bool isTop, bool isTopExists, bool isRecommend, bool isRecommendExists, bool isHot, bool isHotExists, bool isColor, bool isColorExists, string theme, int imageWidth, int imageHeight, int textHeight, string bgColor)
        {
            var databaseManager = parseManager.DatabaseManager;
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var parsedContent = string.Empty;

            var dataManager = new StlDataManager(parseManager.DatabaseManager);
            var channelId = await dataManager.GetChannelIdByChannelIdOrChannelIndexOrChannelNameAsync(pageInfo.SiteId, contextInfo.ChannelId, channelIndex, channelName);

            var minContentInfoList = await databaseManager.ContentRepository.GetSummariesAsync(parseManager.DatabaseManager, pageInfo.Site, channelId, 0, groupContent, groupContentNot, tags, true, true, false, false, false, false, false, startNum, totalNum, orderByString, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, scopeType, groupChannel, groupChannelNot, null);

            if (minContentInfoList != null)
            {
                if (StringUtils.EqualsIgnoreCase(theme, ThemeStyle2))
                {
                    await pageInfo.AddPageBodyCodeIfNotExistsAsync(ParsePage.Const.JsAcSwfObject);

                    var imageUrls = new List<string>();
                    var navigationUrls = new List<string>();
                    var titleCollection = new List<string>();

                    foreach (var minContentInfo in minContentInfoList)
                    {
                        var contentInfo = await databaseManager.ContentRepository.GetAsync(pageInfo.Site, minContentInfo.ChannelId, minContentInfo.Id);
                        var imageUrl = contentInfo.ImageUrl;

                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            if (StringUtils.EndsWithIgnoreCase(imageUrl, ".jpg") || StringUtils.EndsWithIgnoreCase(imageUrl, ".jpeg") || StringUtils.EndsWithIgnoreCase(imageUrl, ".png") || StringUtils.EndsWithIgnoreCase(imageUrl, ".pneg"))
                            {
                                titleCollection.Add(StringUtils.ToJsString(PageUtils.UrlEncode(StringUtils.MaxLengthText(StringUtils.StripTags(contentInfo.Title), titleWordNum))));
                                navigationUrls.Add(PageUtils.UrlEncode(await parseManager.PathManager.GetContentUrlAsync(pageInfo.Site, contentInfo, pageInfo.IsLocal)));
                                imageUrls.Add(PageUtils.UrlEncode(await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site, imageUrl, pageInfo.IsLocal)));
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

                    var elementId = StringUtils.GetElementId();
                    var paramBuilder = new StringBuilder();
                    paramBuilder.Append(
                        $@"so_{elementId}.addParam(""quality"", ""high"");").Append(Constants.ReturnAndNewline);
                    paramBuilder.Append(
                        $@"so_{elementId}.addParam(""wmode"", ""transparent"");").Append(Constants.ReturnAndNewline);
                    paramBuilder.Append(
                        $@"so_{elementId}.addParam(""menu"", ""false"");").Append(Constants.ReturnAndNewline);
                    paramBuilder.Append(
                        $@"so_{elementId}.addParam(""FlashVars"", ""bcastr_file=""+files_uniqueID+""&bcastr_link=""+links_uniqueID+""&bcastr_title=""+texts_uniqueID+""&AutoPlayTime=5&TitleBgPosition={isTopText}&TitleBgColor={bgColor}&BtnDefaultColor={bgColor}"");").Append(Constants.ReturnAndNewline);

                    var bcastrUrl = parseManager.PathManager.GetSiteFilesUrl(pageInfo.Site, Resources.Flashes.Bcastr);

                    string scriptHtml = $@"
<div id=""flashcontent_{elementId}""></div>
<script type=""text/javascript"">
var files_uniqueID='{ListUtils.ToString(imageUrls, "|")}';
var links_uniqueID='{ListUtils.ToString(navigationUrls, "|")}';
var texts_uniqueID='{ListUtils.ToString(titleCollection, "|")}';

var so_{elementId} = new SWFObject(""{bcastrUrl}"", ""flash_{elementId}"", ""{imageWidth}"", ""{imageHeight}"", ""7"", """");
{paramBuilder}
so_{elementId}.write(""flashcontent_{elementId}"");
</script>
";
                    scriptHtml = scriptHtml.Replace("uniqueID", elementId);

                    parsedContent = scriptHtml;
                }
                else if (StringUtils.EqualsIgnoreCase(theme, ThemeStyle3))
                {
                    await pageInfo.AddPageBodyCodeIfNotExistsAsync(ParsePage.Const.JsAcSwfObject);

                    var imageUrls = new List<string>();
                    var navigationUrls = new List<string>();
                    var titleCollection = new List<string>();

                    foreach (var minContentInfo in minContentInfoList)
                    {
                        var contentInfo = await databaseManager.ContentRepository.GetAsync(pageInfo.Site, minContentInfo.ChannelId, minContentInfo.Id);
                        var imageUrl = contentInfo.ImageUrl;

                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            if (StringUtils.EndsWithIgnoreCase(imageUrl, ".jpg") || StringUtils.EndsWithIgnoreCase(imageUrl, ".jpeg") || StringUtils.EndsWithIgnoreCase(imageUrl, ".png") || StringUtils.EndsWithIgnoreCase(imageUrl, ".pneg"))
                            {
                                titleCollection.Add(StringUtils.ToJsString(PageUtils.UrlEncode(StringUtils.MaxLengthText(StringUtils.StripTags(contentInfo.Title), titleWordNum))));
                                navigationUrls.Add(PageUtils.UrlEncode(await parseManager.PathManager.GetContentUrlAsync(pageInfo.Site, contentInfo, pageInfo.IsLocal)));
                                imageUrls.Add(PageUtils.UrlEncode(await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site, imageUrl, pageInfo.IsLocal)));
                            }
                        }
                    }

                    var elementId = StringUtils.GetElementId();
                    var paramBuilder = new StringBuilder();
                    paramBuilder.Append($@"so_{elementId}.addParam(""quality"", ""high"");").Append(Constants.ReturnAndNewline);
                    paramBuilder.Append($@"so_{elementId}.addParam(""wmode"", ""transparent"");").Append(Constants.ReturnAndNewline);
                    paramBuilder.Append($@"so_{elementId}.addParam(""allowFullScreen"", ""true"");").Append(Constants.ReturnAndNewline);
                    paramBuilder.Append($@"so_{elementId}.addParam(""allowScriptAccess"", ""always"");").Append(Constants.ReturnAndNewline);
                    paramBuilder.Append($@"so_{elementId}.addParam(""menu"", ""false"");").Append(Constants.ReturnAndNewline);
                    paramBuilder.Append(
                        $@"so_{elementId}.addParam(""flashvars"", ""pw={imageWidth}&ph={imageHeight}&Times=4000&sizes=14&umcolor=16777215&btnbg=12189697&txtcolor=16777215&urls=""+urls_uniqueID+""&imgs=""+imgs_uniqueID+""&titles=""+titles_uniqueID);").Append(Constants.ReturnAndNewline);

                    var aliUrl = parseManager.PathManager.GetSiteFilesUrl(pageInfo.Site, Resources.Flashes.Ali);

                    string scriptHtml = $@"
<div id=""flashcontent_{elementId}""></div>
<script type=""text/javascript"">
var urls_uniqueID='{ListUtils.ToString(navigationUrls, "|")}';
var imgs_uniqueID='{ListUtils.ToString(imageUrls, "|")}';
var titles_uniqueID='{ListUtils.ToString(titleCollection, "|")}';

var so_{elementId} = new SWFObject(""{aliUrl}"", ""flash_{elementId}"", ""{imageWidth}"", ""{imageHeight}"", ""7"", """");
{paramBuilder}
so_{elementId}.write(""flashcontent_{elementId}"");
</script>
";
                    scriptHtml = scriptHtml.Replace("uniqueID", elementId);

                    parsedContent = scriptHtml;
                }
                else if (StringUtils.EqualsIgnoreCase(theme, ThemeStyle4))
                {
                    var imageUrls = new StringCollection();
                    var navigationUrls = new StringCollection();

                    foreach (var minContentInfo in minContentInfoList)
                    {
                        var contentInfo = await databaseManager.ContentRepository.GetAsync(pageInfo.Site, minContentInfo.ChannelId, minContentInfo.Id);
                        var imageUrl = contentInfo.ImageUrl;

                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            navigationUrls.Add(await parseManager.PathManager.GetContentUrlAsync(pageInfo.Site, contentInfo, pageInfo.IsLocal));
                            imageUrls.Add(await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site, imageUrl, pageInfo.IsLocal));
                        }
                    }

                    //foreach (var dataItem in dataSource)
                    //{
                    //    var contentInfo = new BackgroundContentInfo(dataItem);
                    //    if (!string.IsNullOrEmpty(contentInfo?.ImageUrl))
                    //    {
                    //        navigationUrls.Add(PageUtility.GetContentUrl(pageInfo.Site, contentInfo));
                    //        imageUrls.Add(PageUtility.ParseNavigationUrl(pageInfo.Site, contentInfo.ImageUrl));
                    //    }
                    //}

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

                    var bgUrl = await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site,
                        "@/images/focusviewerbg.png", pageInfo.IsLocal);

                    var scriptHtml = $@"
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

                    parsedContent = scriptHtml.Replace("fv_", $"fv{StringUtils.GetElementId()}_");
                }
                else
                {
                    var imageUrls = new List<string>();
                    var navigationUrls = new List<string>();
                    var titleCollection = new List<string>();

                    foreach (var minContentInfo in minContentInfoList)
                    {
                        var contentInfo = await databaseManager.ContentRepository.GetAsync(pageInfo.Site, minContentInfo.ChannelId, minContentInfo.Id);
                        var imageUrl = contentInfo.ImageUrl;

                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            //这里使用png图片不管用
                            if (StringUtils.EndsWithIgnoreCase(imageUrl, ".jpg") || StringUtils.EndsWithIgnoreCase(imageUrl, ".jpeg"))
                            {
                                titleCollection.Add(StringUtils.ToJsString(PageUtils.UrlEncode(StringUtils.MaxLengthText(StringUtils.StripTags(contentInfo.Title), titleWordNum))));
                                navigationUrls.Add(PageUtils.UrlEncode(await parseManager.PathManager.GetContentUrlAsync(pageInfo.Site, contentInfo, pageInfo.IsLocal)));
                                imageUrls.Add(PageUtils.UrlEncode(await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site, imageUrl, pageInfo.IsLocal)));
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
                        titles = ListUtils.ToString(titleCollection, "|");
                    }
                    var elementId = StringUtils.GetElementId();
                    attributes["id"] = elementId;
                    var divHtml = $@"<div {TranslateUtils.ToAttributesString(attributes)}>&nbsp;</div>";

                    var jsUrl = parseManager.PathManager.GetSiteFilesUrl(pageInfo.Site, Resources.BaiRongFlash.Js);
                    var focusViewerUrl = parseManager.PathManager.GetSiteFilesUrl(pageInfo.Site, Resources.Flashes.FocusViewer);

                    var scriptHtml = $@"
<script type=""text/javascript"" src=""{jsUrl}""></script>
<script type=""text/javascript"">
	var uniqueID_focus_width={imageWidth}
	var uniqueID_focus_height={imageHeight}
	var uniqueID_text_height={textHeight}
	var uniqueID_swf_height = uniqueID_focus_height + uniqueID_text_height
	
	var uniqueID_pics='{ListUtils.ToString(imageUrls, "|")}'
	var uniqueID_links='{ListUtils.ToString(navigationUrls, "|")}'
	var uniqueID_texts='{titles}'
	
	var uniqueID_FocusFlash = new bairongFlash(""{focusViewerUrl}"", ""focusflash"", uniqueID_focus_width, uniqueID_swf_height, ""7"", ""{bgColor}"", false, ""High"");
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
</script>
";
                    scriptHtml = scriptHtml.Replace("uniqueID", elementId);

                    parsedContent = divHtml + scriptHtml;
                }
            }

            return parsedContent;
        }
    }
}